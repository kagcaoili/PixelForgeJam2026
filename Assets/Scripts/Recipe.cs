using UnityEngine;

/// <summary>
/// To complete a recipe, the player must submit the matching requiredItemName
/// </summary>
[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public string requiredItemName;
    public int baseReward = 10; // For completing the recipe
}
