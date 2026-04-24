using UnityEngine;

/// <summary>
/// Player interacts with resource area to obtain a resource item
/// </summary>
public class ResourceArea : MonoBehaviour, IInteractable
{
    [Tooltip("The item prefab that the player can collect from this resource area")]
    public Item resourceItem;

    public void Interact(Player player)
    {
        // If player is already holding an item, do nothing
        if (player.heldItem != null) return;

        Item newItem = Instantiate(resourceItem);
        player.Hold(newItem);
    }
}
