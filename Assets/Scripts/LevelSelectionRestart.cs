using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionRestart : MonoBehaviour
{
    [SerializeField] private Button restartButton; // Reference to the restart button

    private void Start()
    {
        // Ensure the button is assigned
        if (restartButton == null)
        {
            Debug.LogError("Restart button is not assigned in the inspector.");
            return;
        }

        // Add a listener to the button's click event
        restartButton.onClick.AddListener(ResetEverything);
    }

    private void ResetEverything()
    {
        // Reset all level unlocks
        for (int i = 1; i <= 5; i++) // Adjust based on total levels
        {
            PlayerPrefs.DeleteKey($"Level{i}Unlocked");
        }

        // Unlock the first level by default
        PlayerPrefs.SetInt("Level1Unlocked", 1);

        // Reset coins and player progress
        PlayerPrefs.DeleteKey("CoinCount");
        PlayerPrefs.DeleteAll(); //Fully resets all stored progress
        PlayerPrefs.Save();

        Debug.Log("All game progress has been reset to default.");

        // Reload the level selection scene to reflect changes
        SceneManager.LoadScene("LevelSelection");
    }
}
