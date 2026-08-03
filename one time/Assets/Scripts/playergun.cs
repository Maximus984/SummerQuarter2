using UnityEngine;
using UnityEngine.InputSystem;

public class playergun : MonoBehaviour
{
    // Store a bullet prefab
    // Instantiate a bullet forward at a certain rate

    [SerializeField] private GameObject bulletPrefab; // The prefab of the bullet this shoots
    [SerializeField] private Transform gunBarrel; // The barrel where bullets are spawned
    [SerializeField] private float fireRate = 1.5f; // How fast the gun fires projectiles
    private float fireRateTimer = 0f; // Timer to track when the gun can fire again

    private InputAction shootAction;
    [SerializeField] private InputActionAsset inputActions;

    private void Shoot()
    {
        // Create a bullet at the gun barrel
        Instantiate(bulletPrefab, gunBarrel.position, Quaternion.identity);
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

    private void Awake()
    {
        // Repeatedly trigger the shoot method
        shootAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        // Decremenent the fire rate timer
        fireRateTimer -= Time.deltaTime;

        if (shootAction.WasPressedThisFrame())
        {
            // Check if we are ready to fire again 
            if (fireRateTimer <= 0f)
            {
                // Call the shoot method
                Shoot();
                // Reset the fire rate timer
                fireRateTimer = fireRate;
            }
        }
    }



}
