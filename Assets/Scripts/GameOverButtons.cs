using TMPro;
using UnityEngine;

public class GameOverButtons : MonoBehaviour
{
    public GameObject PlayAgainButton;
    public GameObject ExitButton;

    public void ResetGame()
    {
        GameManager.Instance.ResetGame();
    }

    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
        #endif
    }
}
