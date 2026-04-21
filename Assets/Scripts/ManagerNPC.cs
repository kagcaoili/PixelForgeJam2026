using TMPro;
using UnityEngine;

public class ManagerNPC : MonoBehaviour
{
    [Header("Patience")]
    [Tooltip("How much patience the manager has for missed orders or cats on the counter before the day ends in failure")]
    public float patienceLossPerExpiredOrder = 5f;
    public float patienceLossPerCaughtWithCat = 10f;

    [Header("Movement Settings")]
    [Tooltip("The movement speed")]
    public float speed = 5f;

    [Tooltip("The range manager can see")] // TODO
    [SerializeField] public float viewCone = 10f;

    [Header("UI")]
    public TextMeshProUGUI patienceText;

    public float Patience { get; private set; }
    float maxPatience; // obtained from Day data

    /// <summary>
    /// Called by DayManager at the start of each day to initialize patience values and UI
    /// </summary>
    public void StartDay()
    {
        maxPatience = GameManager.Instance.dayManager.currentDay.patienceMeterStartAmount;
        Patience = maxPatience;
        UpdatePatienceUI();
    }

    /// <summary>
    /// Public accessors when player fails order
    /// </summary>
    public void LosePatienceFromExpiredOrder()
    {
        LosePatience(patienceLossPerExpiredOrder);
    }

    /// <summary>
    /// Public accessors when player is caught with cat
    /// </summary>
    public void LosePatienceFromCaughtWithCat()
    {
        LosePatience(patienceLossPerCaughtWithCat);
    }

    /// <summary>
    /// TODO: Probably reset position of manager and any other things to reset at new game
    /// </summary>
    public void Reset()
    {
        
    }

    private void LosePatience(float amount)
    {
        Patience = Mathf.Max(Patience - amount, 0);
        UpdatePatienceUI();

        if (Patience <= 0)
        {
            Debug.Log("Manager has lost all patience!");
            GameManager.Instance.dayManager.EndDay(false); // End day in failure
        }
    }

    private void UpdatePatienceUI()
    {
        patienceText.text = $"<b>Patience Meter:</b>\n{Mathf.CeilToInt(Patience)} / {Mathf.CeilToInt(maxPatience)}";
    }
}
