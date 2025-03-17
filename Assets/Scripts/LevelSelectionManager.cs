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
        PlayerPrefs.SetInt("Level1Unlocked", 1);
        PlayerPrefs.Save();

        // Check PlayerPrefs to lock/unlock levels and update UI
        UpdateLevelButtons();
    }

    private void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // Level index starts from 1
            bool isUnlocked = PlayerPrefs.GetInt($"Level{levelIndex}Unlocked", 0) == 1;

            // Debug log to check the status of each level
            Debug.Log($"Level {levelIndex} is unlocked: {isUnlocked}");

            // Update button interactability
            levelButtons[i].interactable = isUnlocked;

            // Show or hide lock icons
            if (lockIcons[i] != null)
            {
                lockIcons[i].SetActive(!isUnlocked); // Show lock icon if the level is locked
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

    public void UnlockNextLevel(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        PlayerPrefs.SetInt($"Level{nextLevel}Unlocked", 1); // Unlock the next level
        PlayerPrefs.Save();
        Debug.Log($"Level {nextLevel} unlocked!");

        // Update UI after unlocking
        if (nextLevel - 1 < levelButtons.Length && lockIcons.Length > nextLevel - 1)
        {
            levelButtons[nextLevel - 1].interactable = true; // Enable the next level button
            lockIcons[nextLevel - 1]?.SetActive(false); // Hide the lock icon
        }
    }
}