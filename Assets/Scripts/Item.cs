using UnityEngine;

public class Item : MonoBehaviour
{
    [Tooltip("The name of the item, used as id for rules")]
    public string itemName;

    [Tooltip("The display name of the item, used for UI and such")]
    public string displayName;
}
