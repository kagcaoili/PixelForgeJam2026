using UnityEngine;

/// <summary>
/// An active order player is trying to complete
/// Needs to know the recipe, how much time is left and total time it was completed
/// </summary>
public class Order
{
    public Recipe recipe;
    public float timeRemaining;
    public float totalTime;

    public Order(Recipe recipe)
    {
        this.recipe = recipe;
        this.timeRemaining = GameManager.Instance.orderManager.patienceDuration;
        this.totalTime = GameManager.Instance.orderManager.patienceDuration;
    }

    /// <summary>
    /// Calculates the reward for completing this order based on how much time is left and the recipe's base reward.
    /// </summary>
    /// <returns></returns>
    public float CalculateReward()
    {
        float t = timeRemaining / totalTime; // Percentage of time left
        float bonus = t >= GameManager.Instance.orderManager.bonusPercentageThreshold ? recipe.baseReward * GameManager.Instance.orderManager.bonusMultiplier : 0f;

        float earnedPoints = recipe.baseReward + bonus;
        return earnedPoints;
    }
}
