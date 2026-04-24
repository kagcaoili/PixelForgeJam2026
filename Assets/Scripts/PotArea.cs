using UnityEngine;

/// <summary>
/// Player interact with pot to convert item to something else, no need to hold
/// </summary>
public class PotArea : Station
{
    private void Update()
    {
        if (currentRule == null || currentRule.requiresHold) return;

        progress += Time.deltaTime;
        UpdateProgressBar();

        if (progress >= currentRule.duration)
        {
            Transform(); // turn input item into output item
        }
    }
}
