using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip backgroundMusic;

    public GameObject settingsMenu; // Reference to the SettingsMenu Panel
    public Slider volumeSlider; // Volume slider
    public Toggle musicToggle; // Music on/off toggle

    private void Start()
    {
        // Initialize AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.Play();

        // Set default values
        if (volumeSlider != null)
            volumeSlider.value = audioSource.volume;

        if (musicToggle != null)
            musicToggle.isOn = true;
    }

    public void PlayGame()
    {
        // Reset coins before starting a new game
        ResetCoins();

        // Load the first scene (or next scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void AdjustVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void ToggleMusic(bool isEnabled)
    {
        audioSource.mute = !isEnabled;
    }

    private void ResetCoins()
    {
        // Reset the coin count in PlayerPrefs
        PlayerPrefs.DeleteKey("CoinCount");
        Debug.Log("Coins reset to zero.");
    }
}