using UnityEngine;

[System.Serializable]
public class TransformRule
{
    [Tooltip("The name of the item that can be transformed. E.g. 'RawTuna'")]
    public string inputItemName;

    [Tooltip("The prefab that will be spawned when the transformation is complete. E.g. SalmonSashimi prefab")]
    public GameObject outputPrefab;

    [Tooltip("How long the transformation takes")]
    public float duration = 2f;
    
    [Tooltip("Whether the player needs to hold the interact button for the transformation to complete")]
    public bool requiresHold = false; // chop needs hold, everything else doesn't

}