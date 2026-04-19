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

    InputSystem_Actions inputActions;
    Rigidbody rb;
    Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = GameManager.Instance.inputManager.inputActions;
    }

    void Update()
    {
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
        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            //Debug.Log("Player interacted!");
            TryInteract();
        }
    }

    /// <summary>
    /// Checks for interactable objects within the player's interactable range and interacts with the first one found
    /// </summary>
    void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactableRange);
        foreach(Collider hit in hits)
        {
            //Debug.Log("Colliding with " + hit.name);
            // Checks if the hit object or its parent has an Interactable component
            if (hit.TryGetComponent(out IInteractable target))
            {
                Debug.Log("Interacting with " + hit.name);
                target.Interact(this);
                return;
            } else if (hit.GetComponentInParent<IInteractable>() != null)
            {
                IInteractable parentTarget = hit.GetComponentInParent<IInteractable>();
                Debug.Log("Interacting with parent " + parentTarget);
                parentTarget.Interact(this);
                return;
            }
        }
    }

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

    void OnDrawGizmosSelected()
    {
        // Helps visualize the player interactable area. Select player and see it in scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactableRange);
    }

}
