using UnityEngine;

/// <summary>
/// Place on any object that the player can interact with
/// </summary>
public interface IInteractable
{
    void Interact(Player player);
}
