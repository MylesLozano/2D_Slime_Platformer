using UnityEngine;
using UnityEngine.UI;

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
        for (int i = 1; i <= 5; i++) // Adjust the number of levels as needed
        {
            PlayerPrefs.DeleteKey($"Level{i}Unlocked");
        }

        // Unlock the first level by default
        PlayerPrefs.SetInt("Level1Unlocked", 1);

        // Reset coins
        PlayerPrefs.DeleteKey("CoinCount");

        // Save changes
        PlayerPrefs.Save();

        Debug.Log("All progress has been reset.");

        // Reload the level selection scene to update the UI
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelection");
    }
}