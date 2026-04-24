using UnityEngine;

/// <summary>
/// Place on any object that the player can interact with
/// </summary>
public interface IInteractable
{
    void Interact(Player player);
    void InteractHold(Player player, float deltaTime) {} // made this default since most interactables don't hold rn
}
