using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons; // Array for level buttons
    [SerializeField] private GameObject[] lockIcons; // Array for lock icons

    private void Start()
    {
        // Ensure arrays are of the same length
        if (levelButtons.Length != lockIcons.Length)
        {
            Debug.LogError("LevelButtons and LockIcons arrays must be of the same length. Disabling LevelSelectionManager.");
            this.enabled = false;
            return;
        }

        // Unlock the first level by default
        if (!PlayerPrefs.HasKey("Level1Unlocked"))
        {
            PlayerPrefs.SetInt("Level1Unlocked", 1);
            PlayerPrefs.Save();
        }

        // Check PlayerPrefs to lock/unlock levels and update UI
        UpdateLevelButtons();
    }

    public void UpdateLevelButtons()
    {
        // Ensure levelButtons and lockIcons arrays are properly assigned
        if (levelButtons == null || lockIcons == null)
        {
            Debug.LogError("LevelButtons or LockIcons arrays are not assigned in the inspector.");
            return;
        }

        // Ensure levelButtons and lockIcons arrays have the same length
        if (levelButtons.Length != lockIcons.Length)
        {
            Debug.LogError("LevelButtons and LockIcons arrays must be of the same length.");
            return;
        }

        // Loop through each level button and update its state
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // Level index starts from 1
            bool isUnlocked = PlayerPrefs.GetInt($"Level{levelIndex}Unlocked", 0) == 1;

            // Debug log to check the status of each level
            Debug.Log($"Level {levelIndex} is unlocked: {isUnlocked}");

            // Update button interactability
            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable = isUnlocked;
            }
            else
            {
                Debug.LogWarning($"Level button for Level {levelIndex} is not assigned in the inspector.");
            }

            // Show or hide lock icons
            if (lockIcons[i] != null)
            {
                lockIcons[i].SetActive(!isUnlocked); // Show lock icon if the level is locked
            }
            else
            {
                Debug.LogWarning($"Lock icon for Level {levelIndex} is not assigned in the inspector.");
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene("Level" + levelIndex);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void UnlockAllLevels()
    {
        for (int i = 1; i <= levelButtons.Length; i++)
        {
            PlayerPrefs.SetInt($"Level{i}Unlocked", 1); // Unlock all levels
        }
        PlayerPrefs.Save();
        Debug.Log("All levels unlocked!");

        // Update the UI to reflect the unlocked levels
        UpdateLevelButtons();
    }
}