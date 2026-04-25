using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    
    [Header("Movement Settings")]
    [Tooltip("The movement speed of the player")]
    public float speed = 5f;

    [Tooltip("The speed at which the player turns to face movement direction")]
    [SerializeField] public float turnSpeed = 12f;

    [Tooltip("The range at which the player can interact with objects")]
    [SerializeField] public float interactableRange = 1f;

    [Header("Object References")]
    [Tooltip("The point where held items will be positioned")]
    public Transform holdPoint;

    // The item that the player is currently holding, null if not holding anything
    public Item heldItem { get; private set; }
    public Cat heldCat { get; private set; } // cant hold item and cat at same time

    Vector3 originalPosition; // Used for resetting player position and rotation at new game
    Vector3 originalRotation;
    InputSystem_Actions inputActions;
    Rigidbody rb;
    Vector3 moveInput;
    IInteractable currentTarget; // The interactable the player is currently looking at

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = GameManager.Instance.inputManager.inputActions;
        originalPosition = transform.position;
        originalRotation = transform.eulerAngles;
    }

    void Update()
    {
        if (!GameManager.Instance.dayManager.isDayActive) return; // Don't allow player to move or interact if the day isn't active
        
        UpdateMovement();
        UpdateInteraction();
    }

    /// <summary>
    /// Supports WASD movement
    /// </summary>
    void UpdateMovement()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        moveInput.x = kb.dKey.isPressed ? 1 : kb.aKey.isPressed ? -1 : 0;
        moveInput.z = kb.wKey.isPressed ? 1 : kb.sKey.isPressed ? -1 : 0;

        rb.linearVelocity = moveInput.normalized * speed;

        // Rotate player to face movement direction if moving
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(moveInput.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Checks for interaction input
    /// </summary>
    void UpdateInteraction()
    {
        // check if current target is destroyed to clear it
        if (currentTarget != null && (currentTarget as UnityEngine.Object) == null)
        {
            currentTarget = null;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, interactableRange);
        // look for closest hit
        float bestDist = float.MaxValue;
        IInteractable bestTarget = null;
        foreach(Collider hit in hits)
        {
            // Checks if the hit object or its parent has an Interactable component
            if (hit.TryGetComponent(out IInteractable target))
            {
                Debug.Log("Found interactable: " + hit.name);
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestTarget = target;
                }
            } else if (hit.GetComponentInParent<IInteractable>() != null)
            {
                Debug.Log("Found interactable in parent: " + hit.name);
                IInteractable parentTarget = hit.GetComponentInParent<IInteractable>();
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestTarget = parentTarget;
                }
            }
        }

        Debug.Log("Best target: " + (bestTarget != null ? bestTarget.ToString() : "null"));
        Debug.Log("Current target: " + (currentTarget != null ? currentTarget.ToString() : "null"));
        // Hover logic
        if (bestTarget != currentTarget)
        {
            if (currentTarget != null) currentTarget.OnHoverExit();
            currentTarget = bestTarget;
            if (currentTarget != null) currentTarget.OnHoverEnter();
        }

        if (currentTarget == null) return;

        // Press E to interact
        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            //Debug.Log("Player interacted!");
            currentTarget.Interact(this);
        }

        // Hold E to interact
        if (inputActions.Player.Interact.IsPressed())
        {
            currentTarget.InteractHold(this, Time.deltaTime);
        }
    }

    #region Item Handling
    public void Hold(Item item)
    {
        heldItem = item;
        item.transform.parent = holdPoint;
        item.transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        if (heldItem == null) return;

        heldItem.transform.parent = null;
        heldItem = null;
    }

    // Similar to drop but also returns the item, used in station
    public Item TakeHeldItem()
    {
        if (heldItem == null) return null;

        Item item = heldItem;
        item.transform.parent = null;
        heldItem = null;
        return item;
    }
    #endregion

    #region Cat Handling
    public void HoldCat(Cat cat)
    {
        heldCat = cat;
        cat.transform.parent = holdPoint;
        cat.transform.localPosition = Vector3.zero;
    }

    public void DropCat()
    {
        if (heldCat == null) return;

        heldCat.transform.parent = null;
        heldCat = null;
    }
    #endregion

    public void Reset()
    {
        transform.position = originalPosition;
        transform.eulerAngles = originalRotation;
        Drop();
    }

    void OnDrawGizmosSelected()
    {
        // Helps visualize the player interactable area. Select player and see it in scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactableRange);
    }

}
