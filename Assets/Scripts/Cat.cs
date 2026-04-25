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

    [Header("Need Timer")]
    [Tooltip("The range of time before the cat starts needing food or petting")]
    public float minIdleTime = 5f;
    public float maxIdleTime = 15f;

    [Header("Interaction Timers")]
    [Tooltip("How long the player needs to hold to feed the cat")]
    public float feedDuration = 1f;
    [Tooltip("How long the player needs to hold to pick up the cat")]
    public float pickUpDuration = 3f;

    [Header("State")]
    public CatLocation location = CatLocation.Alley; // start in the alley
    public GameObject needLabelRoot;
    public TextMeshPro needLabel;
    [Tooltip("Indicator that shows if player is eligible to interact with it")]
    public GameObject hoverIndicator;
    public Image progressBarFill;
    public GameObject progressBar;

    private float timerProgress; // for interactions
    private float nextNeedTime; // randomized time betwen min and max 

    /// <summary>
    /// Called when players interacts with the cat
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        switch (location)
        {
            case CatLocation.Alley:
                if (needsPet)
                    Pet();
                break;
            case CatLocation.Held:
                Drop(player);
                break;
        }
    }

    public void InteractHold(Player player, float deltaTime)
    {
        // player can only feed cat in alley, if cat needs food, and if player is holding food
        if (location == CatLocation.Alley && needsFood && player.heldItem != null)
        {
            timerProgress += deltaTime;
            UpdateProgressBar();
            if (timerProgress >= feedDuration)
            {
                Feed(player);
                timerProgress = 0f; // reset timer after feeding
            }
        }
        else if (location == CatLocation.Kitchen)
        {
            timerProgress += deltaTime;
            UpdateProgressBar();
            if (timerProgress >= pickUpDuration)
            {
                PickUp(player);
                timerProgress = 0f; // reset timer after picking up
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
        location = CatLocation.Held;
    }

    void Drop(Player player)
    {
        Debug.Log("Drop cat");
        player.DropCat();

        // TODO: need to update location based on where it is dropped?
    }
    void Start()
    {
        nextNeedTime = Time.time + Random.Range(minIdleTime, maxIdleTime);
        UpdateNeedLabel();

        if (progressBar != null) progressBar.SetActive(false);
        if (hoverIndicator != null) hoverIndicator.SetActive(false);
    }

    void Update()
    {
        // Only update if the day has begun
        if (!GameManager.Instance.dayManager.isDayActive) return;

        UpdateNeeds();
    }

    void UpdateNeeds()
    {
        // only develop need in alley
        if (location != CatLocation.Alley) return;

        // already needs something. needy cat
        if (needsAttention) return;

        if (Time.time >= nextNeedTime)
        {
            PickNextNeed();
        }

    }

    void PickNextNeed()
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
        if (hoverIndicator != null) hoverIndicator.SetActive(true);
    }

    public void OnHoverExit()
    {
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
}

public enum CatLocation
{
    Alley,
    Held,
    Kitchen
}