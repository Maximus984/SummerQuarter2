using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Stores the one (and only) instance of this script
    public static GameManager Instance {get; private set;}
    public static bool isGameOver = false;

    private void Awake()
    {
        // Check our singleton
        if (Instance == null)
        {
            // Assign this instance of the script as THE instance
            Instance = this; 
        }
        else // There is already a GameManager assigned
        {
            // Destroy this extra copy of this script
            Destroy(gameObject);
        }

        // Reset the flag
        isGameOver = false;
    } 

    public void GameOver()
    {
        if (!isGameOver) isGameOver = true;
        // Trigger Lose state UI
        // ...
        Debug.Log("GAME OVER");
        
        // Load the scene at build index 0
        //SceneManager.LoadScene(0);
    }

}
