using UnityEngine;

public class playergun : MonoBehaviour
{
    // Store a bullet prefab
    // Instantiate a bullet forward at a certain rate

    [SerializeField] private GameObject bulletPrefab; // The prefab of the bullet this shoots
    [SerializeField] private Transform gunBarrel; // The barrel where bullets are spawned
    [SerializeField] private float fireRate = 2f; // How fast the gun fires projectiles

    private void Shoot()
    {
        // Create a bullet at the gun barrel
        Instantiate(bulletPrefab, gunBarrel.position, Quaternion.identity);
    }

    private void Awake()
    {
        // Repeatedly trigger the shoot method
        InvokeRepeating(nameof(Shoot), Random.Range(0,1), fireRate);
    }

}
