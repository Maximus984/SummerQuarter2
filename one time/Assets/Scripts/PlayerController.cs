using UnityEngine;
using UnityEngine.InputSystem; // import the input system into the script
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance {get; private set;} // Singleton instance of the PlayerController
    // Stores the input action sheet usrd for input
    [SerializeField] private InputActionAsset inputActions;

    // ACTIONS
    private InputAction moveAction;
    private InputAction jumpAction;
    private float score = 0f;
    private float highScore;

    private Vector2 moveInput;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundCheckDistance = 1f;

    // COMPONENTS
    [SerializeField] private Rigidbody rb;


    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float strafeSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;


    // awake is called when the script instance is being loaded
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Get the move and jump actions from the input action sheet
        moveAction = InputSystem.actions.FindAction("Move");

        rb = GetComponent<Rigidbody>();
        score = 0f;
        highScore = PlayerPrefs.GetFloat("HighScore", 0f); // Load the high score from PlayerPrefs
    }

    private void Start()
    {
        UIManager.Instance.UpdateHighScore(highScore); // Update the high score UI
    }

    private void OnEnable()
    {
        // turn on the player actions map 
        inputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
            // turn off the player actions map
            inputActions.FindActionMap("Player").Disable();
    }
  

    private void Update()
    {
        // Udate the score based on time
        score = transform.position.z;
        UIManager.Instance.UpdateScore((int)score);

        // read & store movement input from the action sheet    
        moveInput = moveAction.ReadValue<UnityEngine.Vector2>();

        // Check if player has fallen off the map
        if (transform.position.y < -10f)
        {
            Destroy(gameObject); // Destroy the player object
            GameManager.Instance.GameOver();
        }
    }


    private void FixedUpdate()
    {
        HandleMovement();

    }

    private void HandleMovement()
    {
        // Constant forward movement
        Vector3 forwardMovement = Vector3.forward * forwardSpeed;

        // Player-controlled strafing
        Vector3 strafeMovement = Vector3.right * moveInput.x * strafeSpeed;

        Vector3 movement = (forwardMovement + strafeMovement) * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);
    }
 

    private void HandleJump()
    {    
        if (isGrounded()) rb.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
    }

    private bool isGrounded()
    {
        bool isGrounded = Physics.Raycast(transform.position,
         Vector3.down, groundCheckDistance,
         groundLayerMask);

        Debug.Log($"Is Ground: {isGrounded}");
        return isGrounded;
    }

    public void GameOver()
    {
        // Check if the current score is greater than the high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore); // Save the new high score to PlayerPrefs
            UIManager.Instance.UpdateHighScore((int)highScore); // Update the high score UI
            PlayerPrefs.Save(); // Ensure the high score is saved immediately
        }
    }

}