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
    private float bonusScore;
    private float highScore;

    private Vector2 moveInput;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundCheckDistance = 1f;

    // COMPONENTS
    [SerializeField] private Rigidbody rb;


    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float strafeSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Speed Progression")]
    [SerializeField] private float speedIncreaseAt100 = 2f;
    [SerializeField] private float speedIncreaseAt1000 = 6f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float hitCooldown = 1.25f;
    [SerializeField] private float hurtSpeedMultiplier = 0.55f;

    private int currentHealth;
    private float lastHitTime = -10f;
    private float hurtEndTime;
    private CameraShake cameraShake;
    private float shieldEndTime;
    private int awardedXpScore;

    public float CurrentScore => score;
    public int CurrentHealth => currentHealth;
    public float CurrentStrafeDirection => moveInput.x;
    public int CurrentDanger => Mathf.Clamp(1 + Mathf.FloorToInt(score / 100f), 1, 10);


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
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");

        rb = GetComponent<Rigidbody>();
        transform.position += Vector3.up * .5f;
        if (GetComponent<PlayerMechAnimation>() == null)
        {
            gameObject.AddComponent<PlayerMechAnimation>();
        }
        if (GetComponent<CameraModeController>() == null)
        {
            gameObject.AddComponent<CameraModeController>();
        }
        if (GetComponent<ShieldPickupSpawner>() == null) gameObject.AddComponent<ShieldPickupSpawner>();
        if (GetComponent<CoinPickupSpawner>() == null) gameObject.AddComponent<CoinPickupSpawner>();
        score = 0f;
        currentHealth = maxHealth;
        highScore = PlayerPrefs.GetFloat("HighScore", 0f); // Load the high score from PlayerPrefs
    }

    private void Start()
    {
        UIManager.Instance.UpdateHighScore(highScore); // Update the high score UI
        GameManager.Instance.BeginRun();

        if (Camera.main != null)
        {
            cameraShake = Camera.main.GetComponent<CameraShake>();
            if (cameraShake == null) cameraShake = Camera.main.gameObject.AddComponent<CameraShake>();
        }
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
        score = transform.position.z + bonusScore;
        int wholeScore = Mathf.FloorToInt(score);
        if (wholeScore > awardedXpScore)
        {
            XpSystem.AddXp(wholeScore - awardedXpScore);
            awardedXpScore = wholeScore;
            UIManager.Instance.UpdateXp(XpSystem.Xp, XpSystem.Level);
        }
        int dangerLevel = Mathf.Clamp(1 + Mathf.FloorToInt(score / 100f), 1, 10);
        UIManager.Instance.UpdateScore((int)score, currentHealth, maxHealth, dangerLevel);

        // read & store movement input from the action sheet    
        moveInput = ReadMoveInput();

        if (jumpAction.WasPressedThisFrame() || Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandleJump();
        }

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
        float speedMultiplier = Time.time < hurtEndTime ? hurtSpeedMultiplier : 1f;
        float currentForwardSpeed = forwardSpeed;

        if (score >= 1000f)
        {
            currentForwardSpeed += speedIncreaseAt1000;
        }
        else if (score >= 100f)
        {
            currentForwardSpeed += speedIncreaseAt100;
        }

        // Constant forward movement
        Vector3 forwardMovement = Vector3.forward * currentForwardSpeed * speedMultiplier;

        // Player-controlled strafing
        Vector3 strafeMovement = Vector3.right * moveInput.x * strafeSpeed * speedMultiplier;

        Vector3 movement = (forwardMovement + strafeMovement) * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);
    }

    private Vector2 ReadMoveInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return moveAction.ReadValue<Vector2>();

        bool useWasd = PlayerPrefs.GetInt("UseWasdControls", 0) == 1;
        bool left = useWasd ? keyboard.aKey.isPressed : keyboard.leftArrowKey.isPressed;
        bool right = useWasd ? keyboard.dKey.isPressed : keyboard.rightArrowKey.isPressed;
        return new Vector2(right ? 1f : left ? -1f : 0f, 0f);
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

    public void TakeHit()
    {
        if (Time.time < lastHitTime + hitCooldown || GameManager.isGameOver) return;
        if (Time.time < shieldEndTime)
        {
            UIManager.Instance.ShowHitNotice(currentHealth);
            return;
        }

        currentHealth--;
        lastHitTime = Time.time;
        hurtEndTime = Time.time + hitCooldown;
        UIManager.Instance.ShowHitNotice(currentHealth);
        cameraShake?.Shake();
        AudioManager.Instance?.PlaySound("PlayerHit");

        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void AddScoreBonus(int points)
    {
        bonusScore += points;
    }

    public void ActivateShield(float seconds)
    {
        shieldEndTime = Time.time + seconds;
        UIManager.Instance.SetShieldStatus(true);
        UIManager.Instance.ShowRoundMessage("SHIELD ON!\n5 SECONDS");
        Invoke(nameof(HideShieldMessage), 1.5f);
    }

    public void AddLife() { currentHealth++; UIManager.Instance.ShowHitNotice("PERK: +1 LIFE"); }
    public void StartSpeedBurst() { hurtEndTime = Time.time + 10f; hurtSpeedMultiplier = 1.6f; Invoke(nameof(EndSpeedBurst), 10f); }
    private void EndSpeedBurst() { hurtSpeedMultiplier = .55f; }

    private void HideShieldMessage()
    {
        UIManager.Instance.HideRoundMessage();
        UIManager.Instance.SetShieldStatus(false);
    }

}
