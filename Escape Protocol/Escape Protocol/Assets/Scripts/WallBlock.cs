using UnityEngine;
using BigRookGames.Weapons;

public class WallBlock : MonoBehaviour
{
    // Detect colliosn with the player to end the game
    // Track health and destory itself when health reaches 0
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("WallBlock collided with: " + other.gameObject.name);

        // Try to get the player controller component off the object we colidded with
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        // check if the hit object is the player or not 
        if (player != null)
        {
            Destroy(other.gameObject); // Destroy the player object
            // end the game 
            GameManager.Instance.GameOver();
            Debug.Log("PLayer hit the wall block, game over!");
        }
        
       ProjectileController projectile = other.gameObject.GetComponent<ProjectileController>();

        if (projectile != null)
        {
            Debug.Log("BOOOOM");
            PlayerController.Instance?.AddScoreBonus(25);
            // Disable this block
            SetBlockState(false);
        }
    }

    public void SetBlockState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
