using UnityEngine;
using Nakama;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    
    [Header("UI")]
    public GameObject loginPanel;
    public GameObject gamePanel;
    
    private List<GameObject> spawnedPlayers = new List<GameObject>();
    
    void Start()
    {
        // Hide game panel initially
        gamePanel.SetActive(false);
    }
    
    public async void SpawnPlayer()
    {
        try
        {
            // Wait for connection
            while (!NakamaManager.IsConnected)
            {
                await Task.Delay(100);
            }
            
            // Choose random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Spawn player
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Setup player
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                // Randomly assign role (first player is seeker)
                controller.isHider = spawnedPlayers.Count > 0;
            }
            
            // Add to list
            spawnedPlayers.Add(player);
            
            // Show game panel
            loginPanel.SetActive(false);
            gamePanel.SetActive(true);
            
            // Start game
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.StartGame(controller.isHider == false); // isSeeker = !isHider
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to spawn player: " + e.Message);
        }
    }
    
    public void DespawnAllPlayers()
    {
        foreach (GameObject player in spawnedPlayers)
        {
            Destroy(player);
        }
        spawnedPlayers.Clear();
        
        // Show login panel
        gamePanel.SetActive(false);
        loginPanel.SetActive(true);
    }
}