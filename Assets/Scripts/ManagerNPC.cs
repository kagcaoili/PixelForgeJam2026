using TMPro;
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
    public int alleyEntranceIndex = 3;
    [Tooltip("How long the manager will wait at each patrol point before moving to the next one")]
    public float howLongToWaitAtPoint = 2f;
    public float howLongToShooCats = 7f; // how long manager will spend shooing cats before going back to normal patrol

    [Header("UI")]
    public TextMeshProUGUI patienceText;
    public GameObject caughtLabel; // shown when player is caught with cat in alley
    public GameObject meowLabel; // shown when manager hears meow and is investigating

    public float Patience { get; private set; }
    float maxPatience; // obtained from Day data

    public bool inAlley => waitingAtPointIndex == alleyEntranceIndex;
    int patrolDirection = 1; // 1 for forward, -1 for backward through patrol points

    int currentPatrolIndex = 0; // traveling
    int waitingAtPointIndex = -1; // -1 means not currently waiting, index of patrol point waiting
    float patrolTimer = 0f;
    bool investigating = false; // if manager is currently investigating a meow
    float shooTimer = 0f; // how long manager has been shooing cats for
    float originalSpeed; // to reset speed after shooing
    bool caughtPlayer = false; // prevents spamming of caught if player is standing in alley

    void Start()
    {
        caughtLabel.SetActive(false);
        meowLabel.SetActive(false);
        originalSpeed = speed;
    }

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
        UpdateInvestigation();
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
            bool stillShooing = investigating && currentPatrolIndex == alleyEntranceIndex;
            // if shooing, keep waiting at alley entrance until done shooing
            if (!stillShooing) patrolTimer += Time.deltaTime;

            waitingAtPointIndex = currentPatrolIndex; // update index its waiting

            // look in direction of next patrol point while waiting
            int nextPatrolIndex = (currentPatrolIndex + patrolDirection + patrolPoints.Length) % patrolPoints.Length;
            Vector3 nextDir = (patrolPoints[nextPatrolIndex].position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(nextDir);
            
            // if investigating, don't wait, go straight to next point if not already at alley entrance
            if (patrolTimer >= howLongToWaitAtPoint || (investigating && (currentPatrolIndex != alleyEntranceIndex))) // Wait for 2 seconds at the poin
            {
                patrolTimer = 0f;
                currentPatrolIndex = nextPatrolIndex; // move to next patrol point
                waitingAtPointIndex = -1; // no longer waiting
                if (caughtLabel.activeSelf) caughtLabel.SetActive(false); // hide caught label when moving again
            }
        }
    }

    void UpdateShoo()
    {
        if (!inAlley) 
        {   
            caughtPlayer = false; // reset caught player when manager leaves alley
            return; // can only shoo cats if manager is in the alley
        }

        // if player is even IN the alley, lose patience
        //if (GameManager.Instance.player.interactingWithCat || GameManager.Instance.alleyZone.playerInAlley)
        if (!caughtPlayer && GameManager.Instance.alleyZone.playerInAlley)
        {
            caughtPlayer = true;
            // trying to shoo but caught player with cat in alley
            LosePatienceFromCaughtWithCat();
            caughtLabel.SetActive(true);
            Debug.Log("caught player with cat in alley while trying to shoo");
        }

        // if cats in the alley and manager is at alley, shoo all the cats away
        GameManager.Instance.catManager.NotifyShooCats();
    }

    void UpdateInvestigation()
    {
        if (!investigating) return;

        // if manager is at alley entrance, shoo cats and stop investigating
        if (inAlley)
        {
            shooTimer += Time.deltaTime;
            if (shooTimer >= howLongToShooCats)
            {
                shooTimer = 0f;
                investigating = false;
                patrolDirection = 1; // reset patrol direction
                // make manager go back to normal speed
                speed = originalSpeed;
                meowLabel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Called by player when they interact with a cat. If manager is in the alley and player interacts with a cat in the alley, manager loses patience.
    /// </summary>
    /// <param name="cat"></param>
    public void NotifyPlayerInteractedWithCat(Cat cat)
    {

        // if player interacts with cat in alley when manager is in the alley
        if (inAlley || investigating)
        {
            caughtLabel.SetActive(true);
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
        investigating = true;
        meowLabel.SetActive(true);

        // if waiting at alley entrance, just stay there and shoo cats
        if (inAlley)
        {
            UpdateShoo();
            investigating = false;
            patrolDirection = 1; // reset patrol direction
            meowLabel.SetActive(false);
            return;
        }

        int bestDir = 1; // default to going forward
        int bestDist = int.MaxValue;

        // make manager go faster! (lol funny bug where more meows make manager go even faster. funny chaos so keeping it)
        speed *= 1.5f;

        // change direction to closest alley entrance

        int distanceToAlleyGoingForward = (alleyEntranceIndex - currentPatrolIndex + patrolPoints.Length) % patrolPoints.Length;
        int distanceToAlleyGoingBackward = (currentPatrolIndex - alleyEntranceIndex + patrolPoints.Length) % patrolPoints.Length;

        if (distanceToAlleyGoingForward <= distanceToAlleyGoingBackward && distanceToAlleyGoingForward < bestDist)
        {
            bestDist = distanceToAlleyGoingForward;
            bestDir = 1;
        }
        else if (distanceToAlleyGoingBackward < distanceToAlleyGoingForward && distanceToAlleyGoingBackward < bestDist)
        {
            bestDist = distanceToAlleyGoingBackward;
            bestDir = -1;
        }

        patrolDirection = bestDir;
        //investigating = false;
    }


}
