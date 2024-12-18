using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel; // Reference to the Pause Menu Panel
    public Slider volumeSlider;       // Reference to the Volume Slider

    private bool isPaused = false;

    private void Start()
    {
        // Initialize slider value to current volume
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(AdjustVolume);
        }
    }

    private void Update()
    {
        // Check for the 'Esc' key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze the game
        pauseMenuPanel.SetActive(true); // Show the pause menu
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume the game
        pauseMenuPanel.SetActive(false); // Hide the pause menu
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Ensure time is reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is reset
        SceneManager.LoadScene("StartMenu"); // Load the Main Menu scene
    }

    public void AdjustVolume(float volume)
    {
        AudioListener.volume = volume; // Adjust global volume
    }
}
