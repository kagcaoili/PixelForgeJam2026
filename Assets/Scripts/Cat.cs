using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// It's a cat!! We're not supposed to feed it, but it's so cute.
/// So if cat is in the alley, player can feed it (hold) or pet it (tap)
/// If cat is in kitchen, player can pick it up (tap) and take it out to the alley
/// </summary>
public class Cat : MonoBehaviour, IInteractable
{
    [Header("Score Params")]
    [Tooltip("The amount of score the player gets for feeding this cat")]
    [SerializeField] int scoreFeedValue = 10;

    [Tooltip("Amount of score the player gets for petting this cat")]
    [SerializeField] int scorePetValue = 5;

    [Header("Needs")]
    public bool needsFood = false;
    public bool needsPet = false;
    public bool needsAttention => needsFood || needsPet;
    public bool isShooedAway = false;

    [Header("Need Timer")]
    [Tooltip("The range of time before the cat starts needing food or petting")]
    public float minIdleTime = 5f;
    public float maxIdleTime = 15f;
    [Tooltip("Unsatisfied cat will meow after this many seconds")]
    public float meowAfterSeconds = 10f;

    [Header("Interaction Timers")]
    [Tooltip("How long the player needs to hold to feed the cat")]
    public float feedDuration = 1f;
    [Tooltip("How long the player needs to hold to pick up the cat")]
    public float pickUpDuration = 3f;

    [Header("State")]
    public CatLocation location => isHeld ? CatLocation.Held : inKitchen ? CatLocation.Kitchen : CatLocation.Alley;
    public GameObject needLabelRoot;
    public TextMeshPro needLabel;
    [Tooltip("Indicator that shows if player is eligible to interact with it")]
    public GameObject hoverIndicator;
    public Image progressBarFill;
    public GameObject progressBar;
    public GameObject meowLabel;

    private float timerProgress; // for interactions
    private float needyTimer; // how long the cat has been needy for
    //private float nextNeedTime; // randomized time betwen min and max 

    private bool inKitchen = false;
    private bool isHeld = false;
    private bool hasMeowed = false;

    /// <summary>
    /// Called when players interacts with the cat
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        if (isShooedAway) return; // can't interact with cat if it's shooed away 

        switch (location)
        {
            case CatLocation.Alley:
                if (needsPet) {
                    player.interactingWithCat = true;
                    Pet();
                    player.interactingWithCat = false;
                }
                break;
            case CatLocation.Held:
                player.interactingWithCat = true;
                Drop(player);
                player.interactingWithCat = false;
                break;
        }
    }

    public void InteractHold(Player player, float deltaTime)
    {
        if (isShooedAway) return; // can't interact with cat if it's shooed away

        // player can only feed cat in alley, if cat needs food, and if player is holding food
        if (location == CatLocation.Alley && needsFood && player.heldItem != null)
        {
            player.interactingWithCat = true;
            timerProgress += deltaTime;
            UpdateProgressBar();
            if (timerProgress >= feedDuration)
            {
                Feed(player);
                timerProgress = 0f; // reset timer after feeding
                player.interactingWithCat = false;
            }
        }
        else if (location == CatLocation.Kitchen)
        {
            player.interactingWithCat = true;
            timerProgress += deltaTime;
            UpdateProgressBar();
            if (timerProgress >= pickUpDuration)
            {
                PickUp(player);
                timerProgress = 0f; // reset timer after picking up
                player.interactingWithCat = false;
            }
        }
    }

    /// <summary>
    /// Feeds the cat and add score to the player's total score
    /// </summary>
    void Feed(Player player)
    {
        Debug.Log("You fed the cat! Score +" + scoreFeedValue);
        GameManager.Instance.AddScore(scoreFeedValue);

        GameManager.Instance.managerNPC.NotifyPlayerInteractedWithCat(this);

        // get rid of food
        var item = player.TakeHeldItem();
        Destroy(item.gameObject);

        // reset needs
        //needsFood = false;
        //UpdateNeedLabel();
        //nextNeedTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
        GameManager.Instance.catManager.RemoveCat(this); // cat leaves
    }

    void Pet()
    {
        Debug.Log("You pet the cat! Score +" + scorePetValue);
        GameManager.Instance.AddScore(scorePetValue);
        GameManager.Instance.managerNPC.NotifyPlayerInteractedWithCat(this); 

        // reset needs
        //needsPet = false;
        //UpdateNeedLabel();
        //nextNeedTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
        GameManager.Instance.catManager.RemoveCat(this); // cat leaves
    }

    void PickUp(Player player)
    {
        Debug.Log("You picked up the cat!");
        if (player.heldItem != null)
        {
            Debug.Log("Player already holding something, so can't pick up cat");
            return;
        }
        if (player.heldCat != null)
        {
            Debug.Log("Player already holding a cat, so can't pick up another cat");
            return;
        }
        player.HoldCat(this);
        isHeld = true;
    }

    void Drop(Player player)
    {
        Debug.Log("Drop cat");
        player.DropCat();
        isHeld = false;
    }
    void Start()
    {
        //nextNeedTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
        UpdateNeedLabel();

        if (progressBar != null) progressBar.SetActive(false);
        if (hoverIndicator != null) hoverIndicator.SetActive(false);
        if (meowLabel != null) meowLabel.SetActive(false);
    }

    void Update()
    {
        // Only update if the day has begun
        if (!GameManager.Instance.dayManager.isDayActive) return;

        if (isShooedAway) return;

        UpdateNeeds();
    }

    void UpdateNeeds()
    {
        // only develop need in alley
        if (location != CatLocation.Alley) return;

        // already needs something. needy cat
        if (needsAttention)
        {
            if (!hasMeowed)
            {
                needyTimer += Time.deltaTime;
            }

            // if cat has been needy for more than meowAfterSeconds, meow and alert manager
            if (!hasMeowed && needyTimer >= meowAfterSeconds)
            {
                Debug.Log("Cat has meowed!");
                meowLabel.SetActive(true);
                GameManager.Instance.managerNPC.NotifyMeow(this);
                GameManager.Instance.musicManager.PlayRandomMeow();
                hasMeowed = true;
            }
            return;
        }

        PickNeed();

        //if (Time.time >= nextNeedTime)
        //{
        //    PickNextNeed();
        //}

    }

    void PickNeed()
    {
        // choose randomly between pets and food need
        if (Random.value < 0.5f)
        {
            needsFood = true;
            Debug.Log("Cat now needs food!");
        }
        else
        {
            needsPet = true;
            Debug.Log("Cat now needs petting!");
        }

        UpdateNeedLabel();
    }

    public void ClearNeeds()
    {
        needsFood = false;
        needsPet = false;
        UpdateNeedLabel();
    }

    void UpdateNeedLabel()
    {
        if (!needsAttention)
        {
            needLabelRoot.SetActive(false);
            return;
        }

        needLabelRoot.SetActive(true);
        if (needsFood)
        {
            needLabel.text = "Feed me";
        }
        else if (needsPet)
        {
            needLabel.text = "Pet me";
        }
    }

    public void OnHoverEnter()
    {
        if (isShooedAway) return; // can't interact with cat if it's shooed away
        if (hoverIndicator != null) hoverIndicator.SetActive(true);
    }

    public void OnHoverExit()
    {
        if (isShooedAway) return; // can't interact with cat if it's shooed away
        if (hoverIndicator != null) hoverIndicator.SetActive(false);
        timerProgress = 0f; // reset interaction timer if player walks away
        progressBar.SetActive(false); // hide progress bar when not interacting
    }

    public void UpdateProgressBar()
    {
        if (progressBarFill == null) return;

        // no need to show bar if cat doesn't need
        if (!needsAttention)
        {
            progressBar.SetActive(false);
            progressBarFill.fillAmount = 0f;
            return;
        }

        progressBar.SetActive(true);
        if (needsFood)
        {
            progressBarFill.fillAmount = timerProgress / feedDuration;
        }
    }

    /*
    // This didn't work, pick up logic for later?
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kitchen"))
        {
            inKitchen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Kitchen"))
        {
            inKitchen = false;
        }
    }
    */
}

public enum CatLocation
{
    Alley,
    Held,
    Kitchen
}