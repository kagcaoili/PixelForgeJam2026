using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    public InputManager inputManager;
    public OrderManager orderManager;
    public DayManager dayManager;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public GameObject mainMenuUI;
    public GameObject leftPanelUI;
    public GameObject rightPanelUI;

    public int Score { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Score = 0;
        UpdateScoreUI();
    }

    public void StartGame()
    {
        mainMenuUI.SetActive(false);
        leftPanelUI.SetActive(true);
        rightPanelUI.SetActive(true);
        
        dayManager.StartDay(0); // Start the first day
    }

    /// <summary>
    /// Called from other scripts to add score the player's total score
    /// </summary>
    /// <param name="amount"></param>
    public void AddScore(int amount)
    {
        Score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText == null) return;
        scoreText.text = $"Score: {Score}";
    }
}
