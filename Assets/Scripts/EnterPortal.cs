using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnterPortal : MonoBehaviour
{
    private bool isNearPortal = false;

    [SerializeField] private string nextSceneName; // Name of the next scene to load
    [SerializeField] private GameObject uiPrompt;  // UI prompt to display "Press E to Enter"
    [SerializeField] private AudioSource portalAudioSource; // Reference to the AudioSource component

    private void Start()
    {
        // Ensure the UI prompt is hidden at the start
        if (uiPrompt != null)
            uiPrompt.SetActive(false);

        // Ensure the AudioSource is assigned
        if (portalAudioSource == null)
        {
            portalAudioSource = GetComponent<AudioSource>();
            if (portalAudioSource == null)
            {
                Debug.LogWarning("No AudioSource found on the portal. Please add an AudioSource component.");
            }
        }
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
        // Play the portal sound effect
        if (portalAudioSource != null && portalAudioSource.clip != null)
        {
            portalAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Portal AudioSource or AudioClip is not set.");
        }

        // Save coins before loading the next scene
        SlimeMovement player = FindObjectOfType<SlimeMovement>();
        if (player != null)
        {
            int currentLevel = player.currentLevel;

            // Unlock the current level
            PlayerPrefs.SetInt($"Level{currentLevel}Unlocked", 1);

            // If this is Level 5, unlock all levels
            if (SceneManager.GetActiveScene().name == "Level5")
            {
                UnlockAllLevels();
            }

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();

            // Save coins
            player.SaveCoinsOnLevelComplete();
        }
        else
        {
            Debug.LogWarning("SlimeMovement script not found. Coins will not be saved.");
        }

        // Load the next scene with a small delay
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log("Loading scene: " + nextSceneName);
            StartCoroutine(LoadNextSceneAfterDelay(0.5f, nextSceneName)); // Add a 0.5-second delay
        }
        else
        {
            Debug.LogError("Next scene name is not set in the inspector!");
        }
    }

    private void UnlockAllLevels()
    {
        for (int i = 1; i <= 5; i++) // Unlock all 5 levels
        {
            PlayerPrefs.SetInt($"Level{i}Unlocked", 1);
            Debug.Log($"Level {i} unlocked!");
        }
        PlayerPrefs.Save();
        Debug.Log("All levels unlocked and saved!");
    }

    private IEnumerator LoadNextSceneAfterDelay(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        SceneManager.LoadScene(sceneName); // Load the next scene
        Debug.Log("SCENE LOADED!");
    }
}