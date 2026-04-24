using UnityEngine;

/// <summary>
/// Player holds interaction button with the cutting board to convert an item to something else
/// </summary>
public class CuttingBoardArea : Station
{
    public override void InteractHold(Player player, float deltaTime)
    {
        if (currentItem == null || currentRule == null || !currentRule.requiresHold) return;

        progress += deltaTime;
        UpdateProgressBar();
        if (progress >= currentRule.duration)
        {
            Transform(); // turn input item into output item
        }
    }
}
