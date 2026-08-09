using UnityEngine;
using UnityEngine.UI;

public class MobileUI : MonoBehaviour
{
    [Header("Mobile Controls")]
    public SimpleJoystick joystick;  // Changed from FixedJoystick
    public Button hideButton;
    public Button catchButton;
    public Button jumpButton;
    
    [Header("UI Panels")]
    public GameObject mobileControlsPanel;
    public GameObject pcControlsPanel;
    
    [Header("Settings")]
    public bool isMobile = true;
    
    private PlayerController playerController;
    
    void Start()
    {
        // Find player controller
        playerController = FindObjectOfType<PlayerController>();
        
        // Setup UI based on platform
        SetupUIForPlatform();
        
        // Setup button listeners
        if (hideButton != null)
            hideButton.onClick.AddListener(OnHideButtonClicked);
            
        if (catchButton != null)
            catchButton.onClick.AddListener(OnCatchButtonClicked);
            
        if (jumpButton != null)
            jumpButton.onClick.AddListener(OnJumpButtonClicked);
    }
    
    void SetupUIForPlatform()
    {
        // Check if mobile
        isMobile = Application.isMobilePlatform;
        
        // Show appropriate controls
        if (mobileControlsPanel != null)
            mobileControlsPanel.SetActive(isMobile);
            
        if (pcControlsPanel != null)
            pcControlsPanel.SetActive(!isMobile);
    }
    
    void OnHideButtonClicked()
    {
        if (playerController != null && playerController.isHider)
        {
            playerController.ToggleHide();
        }
    }
    
    void OnCatchButtonClicked()
    {
        if (playerController != null && !playerController.isHider)
        {
            // Seeker catch logic
            CatchNearestPlayer();
        }
    }
    
    void OnJumpButtonClicked()
    {
        if (playerController != null)
        {
            // Jump logic
            CharacterController controller = playerController.GetComponent<CharacterController>();
            if (controller != null && controller.isGrounded)
            {
                // Simple jump
                controller.Move(Vector3.up * 5f * Time.deltaTime);
            }
        }
    }
    
    void CatchNearestPlayer()
    {
        // Find all hiders
        GameObject[] hiders = GameObject.FindGameObjectsWithTag("Hider");
        
        if (hiders.Length == 0)
            return;
        
        // Find nearest hider
        GameObject nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (GameObject hider in hiders)
        {
            float distance = Vector3.Distance(playerController.transform.position, hider.transform.position);
            if (distance < nearestDistance)
            {
                nearest = hider;
                nearestDistance = distance;
            }
        }
        
        // Catch if close enough
        if (nearest != null && nearestDistance < 3f)
        {
            PlayerController caughtController = nearest.GetComponent<PlayerController>();
            if (caughtController != null)
            {
                caughtController.GetCaught();
                
                // Notify game manager
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.OnPlayerCaught(nearest);
                }
            }
        }
    }
    
    // Update button visibility based on role
    public void UpdateButtonVisibility(bool isHider)
    {
        if (hideButton != null)
            hideButton.gameObject.SetActive(isHider);
            
        if (catchButton != null)
            catchButton.gameObject.SetActive(!isHider);
    }
}