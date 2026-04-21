using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Has list of recipes and generates new orders for the player to complete. 
/// Also keeps track of player's current orders and checks if they are completed or not
/// </summary>
public class OrderManager : MonoBehaviour
{

    [Header("Scoring")]

    [Tooltip("Percentage player must complete order by to get bonus points")]
    public float bonusPercentageThreshold = 0.5f; // Percentage of time remaining for bonus

    [Tooltip("Bonus multiplier awarded for completing an order quickly")]
    public float bonusMultiplier = 0.5f; // e.g. 50% bonus if completed above threshold

    [Header("UI")]
    public GameObject orderPrefab;
    public Transform orderListParent;

    private List<Order> activeOrders = new List<Order>();
    private float orderSpawnTimer = 0f;
    private Dictionary<Order, OrderUI> ordersUIMap = new Dictionary<Order, OrderUI>();

    void Update()
    {
        // Only update orders if the day has begun
        if (!GameManager.Instance.dayManager.isDayActive) return;

        UpdateExistingOrders();
        SpawnNewOrder();
    }

    private void UpdateExistingOrders()
    {
        // tick down timers for active orders and remove expired ones
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            activeOrders[i].timeRemaining -= Time.deltaTime;
            if (activeOrders[i].timeRemaining <= 0)
            {
                // Order expired, remove it
                Debug.Log($"Found expired order: {activeOrders[i].recipe.recipeName}");

                // Penalize player for missed order
                GameManager.Instance.managerNPC.LosePatienceFromExpiredOrder();

                RemoveOrderFromUI(activeOrders[i]);

                activeOrders.RemoveAt(i);

                // TODO: SFX? Visual penalty for expired order
            }
        }
    }

    private void SpawnNewOrder()
    {
        Day currentDay = GameManager.Instance.dayManager.currentDay;

        // spawn new orders at intervals if we have room for more
        if (activeOrders.Count < currentDay.maxActiveOrders)
        {
            orderSpawnTimer += Time.deltaTime;
            if (orderSpawnTimer >= currentDay.orderSpawnInterval)
            {
                SpawnOrder();
            }
        }
    }

    void SpawnOrder()
    {
        Day currentDay = GameManager.Instance.dayManager.currentDay;
        orderSpawnTimer = 0f;
        int randomIndex = Random.Range(0, currentDay.recipePool.Length);
        Recipe randomRecipe = currentDay.recipePool[randomIndex];
        Order newOrder = new Order(randomRecipe);
        activeOrders.Add(newOrder);

        // add to UI
        GameObject orderGO = Instantiate(orderPrefab, orderListParent);
        OrderUI orderUI = orderGO.GetComponent<OrderUI>();
        orderUI.SetOrder(newOrder);
        ordersUIMap[newOrder] = orderUI;

        Debug.Log($"Spawned new order: {randomRecipe.recipeName}");
    }

    /// <summary>
    /// At start of day, spawn initial orders so player doesn't have to wait for first order to spawn
    /// </summary>
    public void SpawnInitialOrders()
    {
        Day currentDay = GameManager.Instance.dayManager.currentDay;
        for (int i = 0; i < currentDay.initialOrderCount; i++)
        {
            SpawnOrder();
        }
    }

    /// <summary>
    /// Called by serving area when player tries to submit item for completion
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CompleteOrder(Item item)
    {
        //Debug.Log($"Submitting '{item.itemName}'. Active orders: {activeOrders.Count}");
        //foreach (var o in activeOrders)
        //    Debug.Log($"order wants '{o.recipe.requiredItemName}'");

        for (int i = 0; i < activeOrders.Count; i++)
        {
            if (activeOrders[i].recipe.requiredItemName.Equals(item.itemName))
            {
                float earnedPoints = activeOrders[i].CalculateReward();
                GameManager.Instance.AddScore(Mathf.RoundToInt(earnedPoints));
                GameManager.Instance.dayManager.NotifyOrderCompleted();
                Debug.Log($"Completed order: {activeOrders[i].recipe.recipeName} for {Mathf.RoundToInt(earnedPoints)} points");
                
                RemoveOrderFromUI(activeOrders[i]);
                activeOrders.RemoveAt(i);

                return true;
            }
        }
        return false; // No matching order found
    }

    /// <summary>
    /// Called to reset orders on new game
    /// </summary>
    public void Reset()
    {
        activeOrders.Clear();
        foreach (var orderUI in ordersUIMap.Values)
        {
            Destroy(orderUI.gameObject);
        }
        ordersUIMap.Clear();
    }

    void RemoveOrderFromUI(Order order)
    {
        if (ordersUIMap.TryGetValue(order, out OrderUI orderUI))
        {
            Destroy(orderUI.gameObject);
            ordersUIMap.Remove(order);
        }
    }

}
