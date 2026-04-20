using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;
    public Color successColor = Color.green;
    public Color inProgressColor = Color.white;

    Objective objective;

    public void SetObjective(Objective objective)
    {
        this.objective = objective;
        
        switch (objective.type)
        {
            case ObjectiveType.OrdersCompleted:
                objectiveText.text = $"Complete {objective.CurrentProgress()}/{objective.targetValue} orders";
                break;
            case ObjectiveType.ScoreThreshold:
                objectiveText.text = $"Reach {objective.CurrentProgress()}/{objective.targetValue} score";
                break;
        }
    }

    void Update()
    {
        UpdateStatus();
    }

    void UpdateStatus()
    {
        SetObjective(objective); // Update text with current progress

        if (objective.IsCompleted())
        {
            objectiveText.color = successColor;
        }
        else
        {
            objectiveText.color = inProgressColor;
        }
    }
}
