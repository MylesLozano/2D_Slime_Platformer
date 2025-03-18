using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public void GoToCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }
}