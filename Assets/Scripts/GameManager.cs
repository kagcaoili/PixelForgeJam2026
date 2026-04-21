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
    public ManagerNPC managerNPC;
    public Player player;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public GameObject mainMenuUI;
    public GameObject leftPanelUI;
    public GameObject rightPanelUI;
    public GameObject endDayUI;
    public GameObject gameOverUI;

    public int Score { get; private set; }
    public bool GameOver { get; private set; } = false;

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
        mainMenuUI.SetActive(true);
        leftPanelUI.SetActive(false);
        rightPanelUI.SetActive(false);
        endDayUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    /// <summary>
    /// Called from start button on main menu to start the game
    /// </summary>
    public void StartGame()
    {
        Score = 0;
        UpdateScoreUI();

        mainMenuUI.SetActive(false);
        leftPanelUI.SetActive(true);
        rightPanelUI.SetActive(true);
        endDayUI.SetActive(false);
        gameOverUI.SetActive(false);

        dayManager.StartDay(0); // Start the first day
        Time.timeScale = 1f; // Ensure game is running at normal speed
    }

    /// <summary>
    /// Play next day
    /// </summary>
    public void ContinuePlaying()
    {
        mainMenuUI.SetActive(false);
        endDayUI.SetActive(false);
        leftPanelUI.SetActive(true);
        rightPanelUI.SetActive(true);
        gameOverUI.SetActive(false);

        dayManager.ContinueToNextDay();
    }

    /// <summary>
    /// Finished the day, either no more days or continue to next day
    /// </summary>
    public void ShowEndGameUI()
    {
        mainMenuUI.SetActive(false);
        leftPanelUI.SetActive(false);
        rightPanelUI.SetActive(false);
        endDayUI.SetActive(true);
        gameOverUI.SetActive(false);

        Time.timeScale = 0f; // Pause the game
    }

    public void ShowGameOverUI()
    {
        GameOver = true;
        mainMenuUI.SetActive(false);
        leftPanelUI.SetActive(false);
        rightPanelUI.SetActive(false);
        endDayUI.SetActive(false);
        gameOverUI.SetActive(true);

        Time.timeScale = 0f; // Pause the game
    }

    public void ResetGame()
    {
        Score = 0;
        UpdateScoreUI();
        GameOver = false;

        mainMenuUI.SetActive(true);
        leftPanelUI.SetActive(false);
        rightPanelUI.SetActive(false);
        endDayUI.SetActive(false);
        gameOverUI.SetActive(false);

        dayManager.Reset();
        orderManager.Reset();
        managerNPC.Reset();
        player.Reset();
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
