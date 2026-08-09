using UnityEngine;
using Nakama;
using System.Threading.Tasks;

public class NakamaManager : MonoBehaviour
{
    private const string ServerKey = "defaultkey";
    private const string Host = "45.76.130.15";
    private const int Port = 7350;
    
    public static Client Client { get; private set; }
    public static ISession Session { get; private set; }
    public static bool IsConnected { get; private set; }
    
    public static System.Action OnConnected;
    public static System.Action<string> OnError;
    
    async void Start()
    {
        await Connect();
    }
    
    public async Task Connect()
    {
        try
        {
            Debug.Log("🔄 Connecting to Nakama server...");
            
            Client = new Client("http", Host, Port, ServerKey);
            
            var deviceId = SystemInfo.deviceUniqueIdentifier;
            Session = await Client.AuthenticateDeviceAsync(deviceId);
            
            IsConnected = true;
            
            Debug.Log("✅ Connected! User ID: " + Session.UserId);
            Debug.Log("✅ Username: " + Session.Username);
            
            OnConnected?.Invoke();
        }
        catch (System.Exception e)
        {
            IsConnected = false;
            Debug.LogError("❌ Connection failed: " + e.Message);
            OnError?.Invoke(e.Message);
        }
    }
}