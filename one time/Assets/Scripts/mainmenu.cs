using UnityEngine;
using UnityEngine.SceneManagement; // Enable this script to change scenes and more

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the next scene in the build index (the game scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);    
    }

    public void ExitGame()
    {
        // Close the game application
        Application.Quit();
    }
}