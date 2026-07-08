using UnityEngine;
using UnityEngine.InputSystem; // import the input system into the script
public class PlayerController : MonoBehaviour
{
    // Stores the input action sheet usrd for input
    [SerializeField] private InputActionAsset inputActions;

    // ACTIONS
    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 moveInput;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundCheckDistance = 1f;

    // COMPONENTS
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;


    // awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get the move and jump actions from the input action sheet
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        rb = GetComponent<Rigidbody>();
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
        // read & store movement input from the action sheet    
        moveInput = moveAction.ReadValue<UnityEngine.Vector2>();

        if (jumpAction.WasPressedThisFrame())
        {
            //Tell the player to jump
            HandleJump();
        }
    }


    private void FixedUpdate()
    {
        HandleMovement();

    }

    private void HandleMovement()
    {
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
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

}