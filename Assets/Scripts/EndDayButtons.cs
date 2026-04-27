using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndDayButtons : MonoBehaviour
{
    public GameObject PlayButton;
    public GameObject ExitButton;
    public TextMeshProUGUI SubtitleText;
    public RawImage newRecipeImage;

    public void OnEnable()
    {
        // if there is more days to play, show play button and start next day text, otherwise show exit button and end game text
        if (GameManager.Instance.dayManager.HasNextDay())
        {
            PlayButton.SetActive(true);
            ExitButton.SetActive(true);
            if (SubtitleText != null) SubtitleText.text = "Start Next Day?";
        }
        else
        {
            PlayButton.SetActive(false);
            ExitButton.SetActive(true);
            if (SubtitleText != null) SubtitleText.text = "Thanks for playing!";
        }

        newRecipeImage.texture = GameManager.Instance.dayManager.currentDay.endDayImage;
    }

    public void ContinuePlaying()
    {
        GameManager.Instance.ContinuePlaying();
    }

    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
        #endif
    }
}
