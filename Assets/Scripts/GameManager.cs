using UnityEngine;
using UnityEngine.UI;
using Nakama;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int maxPlayers = 4;
    public float gameTime = 180f; // 3 minutes
    
    [Header("UI")]
    public Text roleText;
    public Text timerText;
    public Text playersAliveText;
    public Text scoreText;
    
    [Header("Spawn Points")]
    public Transform[] hiderSpawnPoints;
    public Transform[] seekerSpawnPoints;
    
    private List<GameObject> players = new List<GameObject>();
    private bool isSeeker = false;
    private int score = 0;
    private float timeLeft;
    private bool gameActive = false;
    
    void Start()
    {
        timeLeft = gameTime;
    }
    
    public void StartGame(bool seeker)
    {
        isSeeker = seeker;
        gameActive = true;
        
        // Set role text
        roleText.text = isSeeker ? "🔍 SEEKER" : "🫥 HIDER";
        roleText.color = isSeeker ? Color.red : Color.green;
        
        // Spawn player
        SpawnPlayer();
        
        // Start timer
        StartCoroutine(GameTimer());
    }
    
    void SpawnPlayer()
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        if (playerPrefab == null)
        {
            // Create simple player if prefab not found
            playerPrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerPrefab.name = "Player";
            playerPrefab.AddComponent<PlayerController>();
            playerPrefab.AddComponent<CharacterController>();
        }
        
        // Choose spawn point
        Transform spawnPoint = isSeeker ? seekerSpawnPoints[0] : hiderSpawnPoints[Random.Range(0, hiderSpawnPoints.Length)];
        
        // Spawn player
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.isHider = !isSeeker;
        
        players.Add(player);
    }
    
    System.Collections.IEnumerator GameTimer()
    {
        while (timeLeft > 0 && gameActive)
        {
            timerText.text = $"Time: {Mathf.FloorToInt(timeLeft / 60)}:{Mathf.FloorToInt(timeLeft % 60):00}";
            yield return new WaitForSeconds(1);
            timeLeft--;
        }
        
        // Game over
        EndGame();
    }
    
    void EndGame()
    {
        gameActive = false;
        timerText.text = "Game Over!";
        
        // Show results
        if (isSeeker)
        {
            // Seeker wins if caught all hiders
            int hidersLeft = GameObject.FindGameObjectsWithTag("Hider").Length;
            if (hidersLeft == 0)
            {
                scoreText.text = "🎉 You Win! All hiders caught!";
                score += 100;
            }
            else
            {
                scoreText.text = $"😢 You Lose! {hidersLeft} hiders escaped!";
            }
        }
        else
        {
            // Hider wins if not caught
            scoreText.text = "🎉 You Win! You survived!";
            score += 50;
        }
        
        // Save score to Nakama
        SaveScore();
    }
    
    async void SaveScore()
    {
        try
        {
            // Save to leaderboard
            var record = await NakamaManager.Client.WriteLeaderboardRecordAsync(
                NakamaManager.Session,
                "hide_and_seek_scores",
                score
            );
            
            Debug.Log("Score saved! Rank: " + record.Rank);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save score: " + e.Message);
        }
    }
    
    public void OnPlayerCaught(GameObject caughtPlayer)
    {
        // Change caught player to seeker
        PlayerController controller = caughtPlayer.GetComponent<PlayerController>();
        controller.GetCaught();
        
        // Update players alive count
        UpdatePlayersAlive();
    }
    
    void UpdatePlayersAlive()
    {
        int hidersLeft = GameObject.FindGameObjectsWithTag("Hider").Length;
        playersAliveText.text = $"Hiders Left: {hidersLeft}";
    }
}