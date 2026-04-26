using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// Manager walks outside kitchen and has matience meter. 
/// If they here a cat meow, they will investigate and shoo them away.
/// If they see you with the cats in the alley, they will lose patience and cats go away.
/// If they lose all patience, the day ends in failure.
/// </summary>
public class ManagerNPC : MonoBehaviour
{
    [Header("Patience")]
    [Tooltip("How much patience the manager has for missed orders or cats on the counter before the day ends in failure")]
    public float patienceLossPerExpiredOrder = 5f;
    public float patienceLossPerCaughtWithCat = 10f;

    [Header("Movement Settings")]
    [Tooltip("The movement speed")]
    public float speed = 5f;
    [Tooltip("The points the manager will patrol between when walking around")]
    public Transform[] patrolPoints;
    [Tooltip("Indices of patrol points that are entrances to alley. Manager will invesigate to closest one if they hear a cat meow")]
    public int[] alleyEntranceIndices;
    [Tooltip("How long the manager will wait at each patrol point before moving to the next one")]
    public float howLongToWaitAtPoint = 2f;

    [Header("UI")]
    public TextMeshProUGUI patienceText;

    public float Patience { get; private set; }
    float maxPatience; // obtained from Day data

    public bool inAlley => System.Array.IndexOf(alleyEntranceIndices, waitingAtPointIndex) >= 0;
    int patrolDirection = 1; // 1 for forward, -1 for backward through patrol points

    int currentPatrolIndex = 0; // traveling
    int waitingAtPointIndex = -1; // -1 means not currently waiting, index of patrol point waiting
    float patrolTimer = 0f;

    Transform investigatePoint;

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

    /*
    // couldn't get this to work so gonna depend on indices 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Alley"))
        {
            inAlley = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Alley"))
        {
            inAlley = false;
        }
    }
    */

    void Update()
    {

        // Only update if the day has begun
        if (!GameManager.Instance.dayManager.isDayActive) return;

        UpdatePatrol();
        UpdateShoo();
    }

    void UpdatePatrol()
    {
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // look in the direciton
        Vector3 dir = (targetPoint.position - transform.position).normalized;
        if (dir.sqrMagnitude > 0f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // Check if reached patrol point. 0.1f is close enough since its floating point
        // Wait at the spot before moving to next one
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            patrolTimer += Time.deltaTime;
            waitingAtPointIndex = currentPatrolIndex; // update index its waiting

            // look in direction of next patrol point while waiting
            int nextPatrolIndex = (currentPatrolIndex + 1 + patrolPoints.Length) % patrolPoints.Length;
            Vector3 nextDir = (patrolPoints[nextPatrolIndex].position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(nextDir);
            
            if (patrolTimer >= howLongToWaitAtPoint) // Wait for 2 seconds at the point
            {
                patrolTimer = 0f;
                currentPatrolIndex = nextPatrolIndex; // move to next patrol point
                waitingAtPointIndex = -1; // no longer waiting
            }
        }
    }

    void UpdateShoo()
    {
        // if cats in the alley and manager is at alley, shoo all the cats away
        if (inAlley)
        {
            GameManager.Instance.catManager.ShooCats();
        }
    }

    /// <summary>
    /// Called by player when they interact with a cat. If manager is in the alley and player interacts with a cat in the alley, manager loses patience.
    /// </summary>
    /// <param name="cat"></param>
    public void NotifyPlayerInteractedWithCat(Cat cat)
    {
        // if player interacts with cat in alley when manager is in the alley
        if (inAlley && cat.location == CatLocation.Alley)
        {
            LosePatienceFromCaughtWithCat();
            Debug.Log("Manager caught you with a cat in alley");
        }
    }

    /// <summary>
    /// Called by cats when they meow. Manager will investigate the closest alley entrance if they hear a cat meow.
    /// </summary>
    /// <param name="cat"></param>
    public void NotifyMeow(Cat cat)
    {
        
    }


}
