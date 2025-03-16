using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int currentLevel; // Set this in the Inspector (e.g., 1 for Level 1, 2 for Level 2)

    public void CompleteLevel()
    {
        if (currentLevel <= 0)
        {
            Debug.LogError("Current level is not set or invalid. Please set the currentLevel in the inspector.");
            return;
        }

        // Unlock the next level
        UnlockNextLevel();

        // Save progress or do any cleanup
        Debug.Log("Level " + currentLevel + " completed!");

        // Load the Level Selection screen
        SceneManager.LoadScene("LevelSelection");
    }

    private void UnlockNextLevel()
    {
        int nextLevel = currentLevel + 1;

        // Unlock the next level by updating PlayerPrefs
        if (PlayerPrefs.GetInt($"Level{nextLevel}Unlocked", 0) == 0)
        {
            PlayerPrefs.SetInt($"Level{nextLevel}Unlocked", 1);
            PlayerPrefs.Save();
            Debug.Log($"Level {nextLevel} unlocked!");
        }
        else
        {
            Debug.Log($"Level {nextLevel} is already unlocked.");
        }
    }
}