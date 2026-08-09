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
    
    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isHidden = false;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Set player appearance based on role
        if (isHider)
        {
            // Hiders are green
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            // Seekers are red
            GetComponent<Renderer>().material.color = Color.red;
        }
    }
    
    void Update()
    {
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        
        if (moveDirection.magnitude > 0.1f)
        {
            // Rotate towards movement
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // Move
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
        
        // Hide ability (for hiders)
        if (isHider && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleHide();
        }
    }
    
    void ToggleHide()
    {
        isHidden = !isHidden;
        
        if (isHidden)
        {
            // Become invisible
            GetComponent<Renderer>().enabled = false;
            hideEffect.SetActive(true);
        }
        else
        {
            // Become visible
            GetComponent<Renderer>().enabled = true;
            hideEffect.SetActive(false);
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
            
            // Notify game manager
            HideAndSeekGame game = FindObjectOfType<HideAndSeekGame>();
            if (game != null)
            {
                // Update UI
            }
        }
    }
}