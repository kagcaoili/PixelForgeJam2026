using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// It's a cat!! We're not supposed to feed it, but it's so cute.
/// </summary>
public class Cat : MonoBehaviour, IInteractable
{
    [Tooltip("The amount of score the player gets for feeding this cat")]
    [SerializeField] int scoreValue = 10;

    /// <summary>
    /// Called when players interacts with the cat
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        Feed();
    }

    /// <summary>
    /// Feeds the cat and add score to the player's total score
    /// </summary>
    void Feed()
    {
        Debug.Log("You fed the cat! Score +" + scoreValue);
        GameManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }
}
