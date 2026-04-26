using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    public void ResumeGame()
    {
        GameManager.Instance.HidePauseMenu();
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ResetGame();
    }
}
