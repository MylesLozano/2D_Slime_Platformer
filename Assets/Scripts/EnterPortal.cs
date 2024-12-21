using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterPortal : MonoBehaviour
{
    private bool isNearPortal = false;

    [SerializeField] private string nextSceneName; // Name of the next scene to load
    [SerializeField] private GameObject uiPrompt;  // UI prompt to display "Press E to Enter"

    private void Start()
    {
        // Ensure the UI prompt is hidden at the start
        if (uiPrompt != null)
            uiPrompt.SetActive(false);
    }

    private void Update()
    {
        // Check if the player is near the portal and presses 'E'
        if (isNearPortal && Input.GetKeyDown(KeyCode.E))
        {
            SaveCoinsAndLoadNextScene();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect if the player is near the portal
        if (collision.CompareTag("Player"))
        {
            isNearPortal = true;

            // Show the UI prompt
            if (uiPrompt != null)
                uiPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Reset when the player moves away from the portal
        if (collision.CompareTag("Player"))
        {
            isNearPortal = false;

            // Hide the UI prompt
            if (uiPrompt != null)
                uiPrompt.SetActive(false);
        }
    }

    private void SaveCoinsAndLoadNextScene()
    {
        // Save coins before loading the next scene
        SlimeMovement player = FindObjectOfType<SlimeMovement>();
        if (player != null)
        {
            player.SaveCoinsOnLevelComplete();
        }
        else
        {
            Debug.LogWarning("SlimeMovement script not found. Coins will not be saved.");
        }

        // Load the next scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log("Loading scene: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set in the inspector!");
        }
    }
}
