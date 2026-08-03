using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Stores the one (and only) instance of this script
    public static GameManager Instance {get; private set;}
    [SerializeField] public static bool isGameOver = false; // A flag to determine if the game is over or not

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

        // Reset the game over flag
        isGameOver = false;
    } 

    public void GameOver()
    {
        if (isGameOver) return; // Do nothing if the game is already over 

        PlayerController.Instance.GameOver();
        // Set the game to be over
        isGameOver = true;
        // Trigger Game Over UI
        UIManager.Instance.ToggleGameOverUI(true);
    }

    public void LoadMainMenu()
    {
        // Play UI Audio
        // Load the Main Menu Scene
        SceneManager.LoadScene(0);
    }

    public void LoadCurrentScene()
    {
        // Play UI Audio
        // Restarts the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}