using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Has list of recipes and generates new orders for the player to complete. 
/// Also keeps track of player's current orders and checks if they are completed or not
/// </summary>
public class OrderManager : MonoBehaviour
{

    [Header("Order Settings")]

    [Tooltip("List of all possible recipes that can be ordered")]
    public Recipe[] recipePool;

    [Tooltip("Number of orders to generate at the start of the game")]
    public int initialOrderCount = 1;

    [Tooltip("Time interval (in seconds) between spawning new orders")]
    public float orderSpawnInterval = 10f;

    [Tooltip("Maximum number of active orders at a time")]
    public float maxActiveOrders = 3;

    [Tooltip("Time before an order expires")]
    public float patienceDuration = 30f;

    [Header("Scoring")]

    [Tooltip("Percentage player must complete order by to get bonus points")]
    public float bonusPercentageThreshold = 0.5f; // Percentage of time remaining for bonus

    [Tooltip("Bonus multiplier awarded for completing an order quickly")]
    public float bonusMultiplier = 0.5f; // e.g. 50% bonus if completed above threshold

    private List<Order> activeOrders = new List<Order>();
    private float orderSpawnTimer = 0f;

    void Update()
    {
        // tick down timers for active orders and remove expired ones
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            activeOrders[i].timeRemaining -= Time.deltaTime;
            if (activeOrders[i].timeRemaining <= 0)
            {
                // Order expired, remove it
                Debug.Log($"Found expired order: {activeOrders[i].recipe.recipeName}");
                activeOrders.RemoveAt(i);

                // TODO: SFX? Visual penalty for expired order
            }
        }

        // spawn new orders at intervals if we have room for more
        orderSpawnTimer += Time.deltaTime;
        if (orderSpawnTimer >= orderSpawnInterval && activeOrders.Count < maxActiveOrders)
        {
            orderSpawnTimer = 0f;
            int randomIndex = Random.Range(0, recipePool.Length);
            Recipe randomRecipe = recipePool[randomIndex];
            Order newOrder = new Order(randomRecipe);
            activeOrders.Add(newOrder);
            Debug.Log($"Spawned new order: {randomRecipe.recipeName}");
        }
    }

    public bool CompleteOrder(Item item)
    {
        for (int i = 0; i < activeOrders.Count; i++)
        {
            if (activeOrders[i].recipe.requiredItemName == item.itemName)
            {
                float earnedPoints = activeOrders[i].CalculateReward();
                GameManager.Instance.AddScore(Mathf.RoundToInt(earnedPoints));
                Debug.Log($"Completed order: {activeOrders[i].recipe.recipeName} for {Mathf.RoundToInt(earnedPoints)} points");
                activeOrders.RemoveAt(i);
                return true;
            }
        }
        return false; // No matching order found
    }

}
