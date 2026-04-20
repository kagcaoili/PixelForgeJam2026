using UnityEngine;

[System.Serializable]
public struct Objective
{
    [Tooltip("Complete X orders, reach Y score, patience above Z")]
    public ObjectiveType type;
    [Tooltip("Number of ordres to complete, score to reach, etc.")]
    public int targetValue;

    public int CurrentProgress()
    {
        switch (type)
        {
            case ObjectiveType.OrdersCompleted:
                return GameManager.Instance.dayManager.ordersCompletedToday;
            case ObjectiveType.ScoreThreshold:
                return GameManager.Instance.Score;
            default:
                return 0;
        }
    }

    public bool IsCompleted()
    {
        return CurrentProgress() >= targetValue;
    }
}

public enum ObjectiveType
{
    OrdersCompleted, // Complete a certain number of orders
    ScoreThreshold, // Reach a certain score
}

