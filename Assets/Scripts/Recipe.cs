using UnityEngine;

/// <summary>
/// To complete a recipe, the player must submit the matching requiredItemName
/// </summary>
[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    [Tooltip("Display name for this recipe")]
    public string recipeName;
    [Tooltip("Name of the item required to complete this recipe")]
    public string requiredItemName;
    [Tooltip("Base reward for completing this recipe, before any time bonuses")]
    public int baseReward = 10;
    [Tooltip("Time in seconds before the order expires")]
    public int expirationDuration = 30;
}
