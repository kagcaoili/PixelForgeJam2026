using UnityEngine;

public abstract class Station : MonoBehaviour, IInteractable
{
    public TransformRule[] transformRules;
    public Transform itemHoldPoint; // where item is parented when being transformed
    [Tooltip("Indicator that shows if player is eligible to interact with it")]
    public GameObject hoverIndicator;

    protected Item currentItem;
    protected TransformRule currentRule;
    protected float progress;

    private void Start()
    {
        if (hoverIndicator != null) hoverIndicator.SetActive(false);
    }

    public virtual void Interact(Player player)
    {
        // If player is holding smthing and we dont't have an item, try to start a transform
        if (player.heldItem != null && currentItem == null)
        {
            foreach (TransformRule rule in transformRules)
            {
                if (player.heldItem.itemName == rule.inputItemName)
                {
                    currentItem = player.TakeHeldItem();
                    currentItem.transform.SetParent(itemHoldPoint);
                    currentItem.transform.localPosition = Vector3.zero;
                    currentRule = rule;
                    progress = 0f;
                    break;
                }
            }
        }
        // Player isn't holding anything and station has an item, player will try to pick it up
        else if (player.heldItem == null && currentItem != null)
        {
            player.Hold(currentItem);
            currentItem = null;
            currentRule = null;
            progress = 0f;
        }
    }

    public virtual void InteractHold(Player player, float deltaTime)
    {
        // Default holding does nothing
        // cutting board overrides this to actually do something
    }

    public void Transform()
    {
        if (currentItem == null || currentRule == null) return;

        Destroy(currentItem.gameObject);
        GameObject output = Instantiate(currentRule.outputPrefab, itemHoldPoint.position, Quaternion.identity, itemHoldPoint);
        currentItem = output.GetComponent<Item>();
        currentItem.transform.localPosition = Vector3.zero;
        currentRule = null;
        progress = 0f;
    }

    public virtual void OnHoverEnter()
    {
        if (hoverIndicator != null) hoverIndicator.SetActive(true);
    }

    public virtual void OnHoverExit()
    {
        if (hoverIndicator != null) hoverIndicator.SetActive(false);
    }
}
