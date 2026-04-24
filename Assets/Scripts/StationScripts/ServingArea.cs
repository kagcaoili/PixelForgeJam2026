using UnityEngine;

/// <summary>
/// Player interacts with serving area to complete an order
/// </summary>
public class ServingArea : MonoBehaviour, IInteractable
{
    public void Interact(Player player)
    {
        // If player is not holding anything, do nothing
        if (player.heldItem == null) return;

        // Check if player's held item matches any active order
        if(GameManager.Instance.orderManager.CompleteOrder(player.heldItem))
        {
            Destroy(player.heldItem.gameObject);
            player.Drop();
        }
        else
        {
            Debug.Log("Invalid order, player is holding " + player.heldItem.itemName);
            // TODO: Visual feedback for incorrect order?
        }
    }
}
