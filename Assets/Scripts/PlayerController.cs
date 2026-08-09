using UnityEngine;
using Nakama;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    
    [Header("Hide/Seek")]
    public bool isHider = true;
    public GameObject hideEffect;
    
    [Header("Mobile Controls")]
    public FixedJoystick joystick; // Assign in Inspector
    public Button hideButton;      // Assign in Inspector
    
    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isHidden = false;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Auto-find mobile controls if not assigned
        if (joystick == null)
            joystick = FindObjectOfType<FixedJoystick>();
            
        if (hideButton == null)
        {
            // Find button in children
            hideButton = GetComponentInChildren<Button>();
        }
        
        // Setup hide button listener
        if (hideButton != null)
        {
            hideButton.onClick.AddListener(ToggleHide);
        }
        
        // Set player appearance based on role
        if (isHider)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }
    
    void Update()
    {
        // Mobile Movement (Joystick)
        if (joystick != null)
        {
            float horizontal = joystick.Horizontal;
            float vertical = joystick.Vertical;
            
            moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            
            if (moveDirection.magnitude > 0.1f)
            {
                // Rotate towards movement
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                
                // Move
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
        }
        
        // PC Movement (Keyboard)
        else
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
            
            // PC Hide (Space)
            if (isHider && Input.GetKeyDown(KeyCode.Space))
            {
                ToggleHide();
            }
        }
    }
    
    // Public method for mobile button
    public void ToggleHide()
    {
        if (!isHider) return; // Only hiders can hide
        
        isHidden = !isHidden;
        
        if (isHidden)
        {
            // Become invisible
            GetComponent<Renderer>().enabled = false;
            if (hideEffect != null)
                hideEffect.SetActive(true);
            
            // Change button text
            if (hideButton != null)
                hideButton.GetComponentInChildren<Text>().text = "Show";
        }
        else
        {
            // Become visible
            GetComponent<Renderer>().enabled = true;
            if (hideEffect != null)
                hideEffect.SetActive(false);
            
            // Change button text
            if (hideButton != null)
                hideButton.GetComponentInChildren<Text>().text = "Hide";
        }
    }
    
    // Called when seeker catches this player
    public void GetCaught()
    {
        if (isHider)
        {
            // Change to seeker
            isHider = false;
            GetComponent<Renderer>().material.color = Color.red;
            
            // Show if hidden
            if (isHidden)
            {
                GetComponent<Renderer>().enabled = true;
                isHidden = false;
            }
        }
    }
}