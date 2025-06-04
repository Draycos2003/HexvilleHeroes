using UnityEngine;

public class InputManager : MonoBehaviour
{
    InputSystem_Actions inputActions;

    public Vector2 movementInput;

    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions = new InputSystem_Actions();

            inputActions.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
