using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderUI : MonoBehaviour
{
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI timeRemainingText;

    Order order;

    public void SetOrder(Order order)
    {
        this.order = order;
        recipeNameText.text = order.recipe.recipeName;
        UpdateTimeRemaining();
    }

    void Update()
    {
        if (order == null) return;
        UpdateTimeRemaining();
    }

    void UpdateTimeRemaining()
    {
        if (order == null) return;
        timeRemainingText.text = $"Time Left: {order.timeRemaining:0.0}s";
    }
}
