using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [Tooltip("The movement speed of the player")]
    public float speed = 5f;

    [Tooltip("The range at which the player can interact with objects")]
    [SerializeField] float interactableRange = 1f;

    [Tooltip("The point where held items will be positioned")]
    public Transform holdPoint;

    // The item that the player is currently holding, null if not holding anything
    public Item HeldItem;

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
    }

    /// <summary>
    /// Checks for interaction input
    /// </summary>
    void UpdateInteraction()
    {
        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            Debug.Log("Player interacted!");
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
            Debug.Log("Colliding with " + hit.name);
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
        HeldItem = item;
        item.transform.parent = holdPoint;
        item.transform.localPosition = Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactableRange);
    }

}
