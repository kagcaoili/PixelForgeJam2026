using UnityEngine;
using TMPro;

public class DayManager : MonoBehaviour
{
    [Tooltip("List of scriptable day objects in game in order of progression")]
    public Day[] days;
    public Day currentDay { get; private set; }
    public bool isDayActive { get; private set; } = false; // Whether the player can currently receive orders and complete objectives
    public int ordersCompletedToday { get; private set; } = 0;

    [Header("UI References")]
    public TextMeshProUGUI dayText;
    public GameObject objectiveUIPrefab;
    public Transform objectiveUIParent;

    private int currentDayIndex = 0;

    /// <summary>
    /// Public called by start button on UI to start day
    /// </summary>
    /// <param name="newDayIndex"></param>
    public void StartDay(int newDayIndex)
    {
        currentDayIndex = newDayIndex;
        currentDay = days[currentDayIndex];
        ordersCompletedToday = 0;
        isDayActive = true;
        UpdateDayUI();
        GameManager.Instance.orderManager.SpawnInitialOrders();
        GameManager.Instance.managerNPC.StartDay();

        // Error if the day has no objectives
        if (currentDay.objectives == null || currentDay.objectives.Length == 0)
        {
            Debug.LogError($"Day {currentDay.dayName} has no objectives!");
        }
    }

    void Update()
    {
        if (!isDayActive) return;
        if (currentDay == null) return;
        if (currentDay.objectives == null || currentDay.objectives.Length == 0) return;

        if (AreAllObjectivesCompleted())
        {
            EndDay(true);
        }
    }

    bool AreAllObjectivesCompleted()
    {
        foreach (Objective objective in currentDay.objectives)
        {
            if (!objective.IsCompleted())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Call by ManagerNPC when patience runs out or
    /// if we fulfill all objectives and succeed
    /// </summary>
    /// <param name="success"></param>
    public void EndDay(bool success)
    {
        isDayActive = false;

        if (success)
        {
            Debug.Log($"Day {currentDay.dayName} completed successfully");
            GameManager.Instance.ShowEndGameUI();
        } 
        else
        {
            Debug.Log($"Day {currentDay.dayName} failed");
            GameManager.Instance.ShowGameOverUI();
        }
    }

    /// <summary>
    /// Reset game
    /// </summary>
    public void Reset()
    {
        currentDayIndex = 0;
        currentDay = null;
        ordersCompletedToday = 0;
        isDayActive = false;
    }

    void UpdateDayUI()
    {
        if (dayText == null) return;
        dayText.text = $"{currentDay.dayName}";

        // Clear old objectives
        foreach (Transform child in objectiveUIParent)
        {
            if (child.gameObject.GetComponent<ObjectiveUI>() == null) continue; // skip non-objective UI elements
            Destroy(child.gameObject);
        }

        // Create new objective UIs
        foreach (Objective objective in currentDay.objectives)
        {
            GameObject objUI = Instantiate(objectiveUIPrefab, objectiveUIParent);
            ObjectiveUI objectiveUI = objUI.GetComponent<ObjectiveUI>();
            objectiveUI.SetObjective(objective);
        }
    }

    /// <summary>
    /// Used to show information about next day before player clicks start
    /// </summary>
    /// <returns></returns>
    public Day GetNextDay()
    {
        if (currentDayIndex + 1 < days.Length)
        {
            return days[currentDayIndex + 1];
        }
        else
        {
            Debug.Log("No more days available!");
            return null;
        }
    }

    public bool HasNextDay()
    {
        return currentDayIndex + 1 < days.Length;
    }

    /// <summary>
    /// Called by UI to start next day after player clicks continue on end day screen
    /// </summary>
    /// <returns></returns>
    public bool ContinueToNextDay()
    {
        if (HasNextDay())
        {
            StartDay(currentDayIndex + 1);
            return true;
        }
        else
        {
            Debug.Log("No more days available!");
            return false;
        }
    }

    /// <summary>
    /// Called by OrderManager when an order is completed to update progress towards objectives that require completing orders
    /// </summary>
    public void NotifyOrderCompleted()
    {
        ordersCompletedToday++;
    }
}
