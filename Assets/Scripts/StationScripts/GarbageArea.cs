using UnityEngine;

/// <summary>
/// Player interacts with garbage area to throw away item they are holding
/// </summary>
public class GarbageArea : MonoBehaviour, IInteractable
{
    public void Interact(Player player)
    {
        // If player isn't holding an item
        if (player.heldItem == null) return;

        Item heldItem = player.heldItem;
        player.Drop();
        Destroy(heldItem.gameObject);
    }
}
