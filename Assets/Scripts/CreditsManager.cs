using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}