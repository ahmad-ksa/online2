using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    [Header("Login Panel")]
    public GameObject loginPanel;
    public Text statusText;
    public Button connectButton;
    
    [Header("Chat Panel")]
    public GameObject chatPanel;
    public InputField chatInput;
    public Button sendButton;
    public Text chatLog;
    public ScrollRect scrollRect;
    
    [Header("Players Panel")]
    public Text playersText;
    public Text userInfoText;
    
    private List<string> messages = new List<string>();
    private int maxMessages = 20;
    
    void Start()
    {
        // Setup UI
        loginPanel.SetActive(true);
        chatPanel.SetActive(false);
        
        statusText.text = "Connecting to server...";
        connectButton.interactable = false;
        connectButton.GetComponentInChildren<Text>().text = "Connecting...";
        
        // Subscribe to events
        NakamaManager.OnConnected += OnConnected;
        NakamaManager.OnError += OnError;
        
        // Setup chat
        sendButton.onClick.AddListener(SendChatMessage);
        chatInput.onEndEdit.AddListener(OnChatInputSubmit);
    }
    
    void OnDestroy()
    {
        NakamaManager.OnConnected -= OnConnected;
        NakamaManager.OnError -= OnError;
    }
    
    void OnConnected()
    {
        statusText.text = "✅ Connected!";
        connectButton.GetComponentInChildren<Text>().text = "Enter Game";
        connectButton.interactable = true;
        
        userInfoText.text = $"User: {NakamaManager.Session.Username}\nID: {NakamaManager.Session.UserId.Substring(0, 8)}...";
        
        // Show players count
        UpdatePlayersCount();
    }
    
    void OnError(string error)
    {
        statusText.text = "❌ Error: " + error;
        connectButton.GetComponentInChildren<Text>().text = "Retry";
        connectButton.interactable = true;
    }
    
    public void OnConnectButtonClicked()
    {
        if (NakamaManager.IsConnected)
        {
            // Enter game
            loginPanel.SetActive(false);
            chatPanel.SetActive(true);
            AddChatMessage("System", "Welcome to the game!");
        }
        else
        {
            // Retry connection
            statusText.text = "Connecting...";
            connectButton.interactable = false;
            _ = new NakamaManager().Connect();
        }
    }
    
    void OnChatInputSubmit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendChatMessage();
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
            
            // Send to Nakama storage
            var storageObject = new Nakama.WriteStorageObject
            {
                Collection = "chat",
                Key = "message_" + System.DateTime.Now.Ticks,
                Value = JsonUtility.ToJson(new ChatMessage 
                { 
                    username = NakamaManager.Session.Username,
                    text = message,
                    timestamp = System.DateTime.Now.ToString("HH:mm:ss")
                })
            };
            
            await NakamaManager.Client.WriteStorageObjectsAsync(NakamaManager.Session, storageObject);
            
            AddChatMessage(NakamaManager.Session.Username, message);
            
            // Focus back on input
            chatInput.Select();
            chatInput.ActivateInputField();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to send message: " + e.Message);
            AddChatMessage("System", "Failed to send message!");
        }
    }
    
    void AddChatMessage(string username, string message)
    {
        string formattedMessage = $"[{System.DateTime.Now:HH:mm:ss}] {username}: {message}";
        messages.Add(formattedMessage);
        
        if (messages.Count > maxMessages)
            messages.RemoveAt(0);
            
        chatLog.text = string.Join("\n", messages);
        
        // Scroll to bottom
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }
    
    async void UpdatePlayersCount()
    {
        try
        {
            // Get online users
            var result = await NakamaManager.Client.ListUsersAsync(NakamaManager.Session);
            playersText.text = $"Online Players: {result.Users.Count()}";
        }
        catch (System.Exception e)
        {
            playersText.text = "Online Players: --";
            Debug.LogError("Failed to get players: " + e.Message);
        }
    }
}

[System.Serializable]
public class ChatMessage
{
    public string username;
    public string text;
    public string timestamp;
}