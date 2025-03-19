using UnityEngine;

public class DebugPlayerPrefs : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("PlayerPrefs Debug:");
        Debug.Log($"Level1Unlocked: {PlayerPrefs.GetInt("Level1Unlocked", 0)}");
        Debug.Log($"Level2Unlocked: {PlayerPrefs.GetInt("Level2Unlocked", 0)}");
        Debug.Log($"Level3Unlocked: {PlayerPrefs.GetInt("Level3Unlocked", 0)}");
        Debug.Log($"Level4Unlocked: {PlayerPrefs.GetInt("Level4Unlocked", 0)}");
        Debug.Log($"Level5Unlocked: {PlayerPrefs.GetInt("Level5Unlocked", 0)}");
    }
}