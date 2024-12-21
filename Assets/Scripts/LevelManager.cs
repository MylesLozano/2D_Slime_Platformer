using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int currentLevel; // Set this in the Inspector (e.g., 1 for Level 1, 2 for Level 2)

    public void CompleteLevel()
    {
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
