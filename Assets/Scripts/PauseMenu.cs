using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel; // Reference to the Pause Menu Panel
    [SerializeField] private Slider volumeSlider;       // Reference to the Volume Slider

    private bool isPaused = false;
    private int currentLevel; // Track the current level

    private void Start()
    {
        // Initialize slider value to current volume
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(AdjustVolume);
        }

        // Get the current level from the active scene name
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Level"))
        {
            // Extract the level number from the scene name (e.g., "Level1" -> 1)
            if (int.TryParse(sceneName.Replace("Level", ""), out currentLevel))
            {
                Debug.Log($"Current level: {currentLevel}");
            }
            else
            {
                Debug.LogError("Failed to parse current level from scene name.");
            }
        }
        else
        {
            Debug.LogError("Current scene is not a level scene.");
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
        UnlockAllPreviousLevels(); // 🔥 Unlock all previous levels before quitting

        Time.timeScale = 1f; // Ensure time is reset
        SceneManager.LoadScene("StartMenu");
    }

    public void AdjustVolume(float volume)
    {
        AudioListener.volume = volume; // Adjust global volume
    }

    private void UnlockAllPreviousLevels()
    {
        for (int i = 1; i <= currentLevel; i++)
        {
            PlayerPrefs.SetInt($"Level{i}Unlocked", 1);
        }
        PlayerPrefs.Save();
    }

}