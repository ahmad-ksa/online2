using UnityEngine;
using UnityEngine.UI;
using Nakama;
using System.Collections.Generic;
using System.Threading.Tasks;

public class HideAndSeekGame : MonoBehaviour
{
    [Header("Login Panel")]
    public GameObject loginPanel;
    public Text statusText;
    public Button playButton;
    
    [Header("Lobby Panel")]
    public GameObject lobbyPanel;
    public Text roomCodeText;
    public Button createRoomButton;
    public Button joinRoomButton;
    public InputField roomCodeInput;
    public Text playersListText;
    
    [Header("Game Panel")]
    public GameObject gamePanel;
    public Text roleText;
    public Text timerText;
    public Text playersAliveText;
    public Button leaveButton;
    
    [Header("Chat")]
    public InputField chatInput;
    public Button sendButton;
    public Text chatLog;
    public ScrollRect scrollRect;
    
    private string currentMatchId;
    private bool isSeeker = false;
    private List<string> messages = new List<string>();
    private int maxMessages = 15;
    
    void Start()
    {
        // Setup UI
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(false);
        
        statusText.text = "Connecting to server...";
        playButton.interactable = false;
        playButton.GetComponentInChildren<Text>().text = "Connecting...";
        
        // Subscribe to events
        NakamaManager.OnConnected += OnConnected;
        NakamaManager.OnError += OnError;
        
        // Setup buttons
        playButton.onClick.AddListener(OnPlayClicked);
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        leaveButton.onClick.AddListener(LeaveGame);
        sendButton.onClick.AddListener(SendChatMessage);
    }
    
    void OnDestroy()
    {
        NakamaManager.OnConnected -= OnConnected;
        NakamaManager.OnError -= OnError;
    }
    
    void OnConnected()
    {
        statusText.text = "✅ Connected!";
        playButton.GetComponentInChildren<Text>().text = "Play";
        playButton.interactable = true;
    }
    
    void OnError(string error)
    {
        statusText.text = "❌ Error: " + error;
        playButton.GetComponentInChildren<Text>().text = "Retry";
        playButton.interactable = true;
    }
    
    void OnPlayClicked()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        AddChatMessage("System", "Welcome to Hide and Seek!");
    }
    
    async void CreateRoom()
    {
        try
        {
            // Create match
            var match = await NakamaManager.Socket.RpcAsync("create_match", "");
            currentMatchId = match.Payload;
            
            roomCodeText.text = "Room Code: " + currentMatchId.Substring(0, 6).ToUpper();
            AddChatMessage("System", "Room created! Share the code with friends.");
            
            // Join as seeker (first player)
            isSeeker = true;
            JoinMatch();
        }
        catch (System.Exception e)
        {
            AddChatMessage("System", "Failed to create room: " + e.Message);
        }
    }
    
    async void JoinRoom()
    {
        string code = roomCodeInput.text.Trim().ToUpper();
        if (string.IsNullOrEmpty(code))
        {
            AddChatMessage("System", "Please enter a room code!");
            return;
        }
        
        try
        {
            // Join match
            var match = await NakamaManager.Socket.RpcAsync("join_match", code);
            currentMatchId = match.Payload;
            
            AddChatMessage("System", "Joined room!");
            
            // Join as hider
            isSeeker = false;
            JoinMatch();
        }
        catch (System.Exception e)
        {
            AddChatMessage("System", "Failed to join room: " + e.Message);
        }
    }
    
    async void JoinMatch()
    {
        try
        {
            // Join the match
            await NakamaManager.Socket.JoinMatchAsync(currentMatchId);
            
            lobbyPanel.SetActive(false);
            gamePanel.SetActive(true);
            
            // Set role
            roleText.text = isSeeker ? "🔍 You are the SEEKER!" : "🫥 You are a HIDER!";
            roleText.color = isSeeker ? Color.red : Color.green;
            
            AddChatMessage("System", $"Game started! You are {(isSeeker ? "SEEKER" : "HIDER")}");
            
            // Start timer
            StartCoroutine(GameTimer());
        }
        catch (System.Exception e)
        {
            AddChatMessage("System", "Failed to join match: " + e.Message);
        }
    }
    
    System.Collections.IEnumerator GameTimer()
    {
        int timeLeft = 180; // 3 minutes
        
        while (timeLeft > 0)
        {
            timerText.text = $"Time: {timeLeft / 60}:{timeLeft % 60:00}";
            yield return new WaitForSeconds(1);
            timeLeft--;
        }
        
        // Game over
        timerText.text = "Game Over!";
        AddChatMessage("System", "Game ended!");
    }
    
    async void LeaveGame()
    {
        try
        {
            if (!string.IsNullOrEmpty(currentMatchId))
            {
                await NakamaManager.Socket.LeaveMatchAsync(currentMatchId);
            }
            
            gamePanel.SetActive(false);
            lobbyPanel.SetActive(true);
            AddChatMessage("System", "Left the game.");
        }
        catch (System.Exception e)
        {
            AddChatMessage("System", "Error leaving game: " + e.Message);
        }
    }
    
    async void SendChatMessage()
    {
        if (string.IsNullOrEmpty(chatInput.text))
            return;
            
        try
        {
            var message = chatInput.text;
            chatInput.text = "";
            
            // Send via socket
            var chatMessage = new
            {
                username = NakamaManager.Session.Username,
                text = message,
                timestamp = System.DateTime.Now.ToString("HH:mm:ss")
            };
            
            await NakamaManager.Socket.SendMatchStateAsync(
                currentMatchId, 
                1, // op code for chat
                JsonUtility.ToJson(chatMessage)
            );
            
            AddChatMessage(NakamaManager.Session.Username, message);
            chatInput.Select();
            chatInput.ActivateInputField();
        }
        catch (System.Exception e)
        {
            AddChatMessage("System", "Failed to send: " + e.Message);
        }
    }
    
    void AddChatMessage(string username, string message)
    {
        string formatted = $"[{System.DateTime.Now:HH:mm:ss}] {username}: {message}";
        messages.Add(formatted);
        
        if (messages.Count > maxMessages)
            messages.RemoveAt(0);
            
        chatLog.text = string.Join("\n", messages);
        
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }
}