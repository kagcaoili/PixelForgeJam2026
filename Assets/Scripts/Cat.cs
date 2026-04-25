using UnityEditor.EditorTools;
using UnityEngine;

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

    [Header("Needs Timers")]
    public float feedDuration = 1f;
    public float petDuration = 0f; // isntat?
    public float pickUpDuration = 3f;

    [Header("State")]
    public CatLocation location = CatLocation.Alley; // start in the alley

    private float timerProgress;

    /// <summary>
    /// Called when players interacts with the cat
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        switch (location)
        {
            case CatLocation.Alley:
                Pet();
                break;
            case CatLocation.Held:
                Drop(player);
                break;
        }
    }

    public void InteractHold(Player player, float deltaTime)
    {
        if (location == CatLocation.Alley)
        {
            timerProgress += deltaTime;
            if (timerProgress >= feedDuration)
            {
                Feed();
                timerProgress = 0f; // reset timer after feeding
            }
        }
        else if (location == CatLocation.Kitchen)
        {
            timerProgress += deltaTime;
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
    void Feed()
    {
        Debug.Log("You fed the cat! Score +" + scoreFeedValue);
        GameManager.Instance.AddScore(scoreFeedValue);
    }

    void Pet()
    {
        Debug.Log("You pet the cat! Score +" + scorePetValue);
        GameManager.Instance.AddScore(scorePetValue);
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
}

public enum CatLocation
{
    Alley,
    Held,
    Kitchen
}