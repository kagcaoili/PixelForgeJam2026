using UnityEngine;

public class InputManager : MonoBehaviour
{
    public InputSystem_Actions inputActions { get; private set; }

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }
}
