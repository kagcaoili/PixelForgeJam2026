using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    public InputManager inputManager;
    public OrderManager orderManager;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;

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
