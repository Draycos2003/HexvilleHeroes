using UnityEngine;
using UnityEngine.InputSystem;

namespace FinalController
{
    [DefaultExecutionOrder(-2)]
    public class playerLocomotionInput : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        #region Class Variables
        [SerializeField] private bool holdToSprint = true; 
        public InputSystem_Actions Action { get; private set; }
        public Vector2 movementInput { get; private set; }
        public Vector2 lookInput { get; private set; }
        public bool jumpPressed { get; private set; }
        public bool sprintToggledOn { get; private set; }
        public bool walkToggledOn { get; private set; }
        #endregion

        #region StartUp
        private void OnEnable()
        {
            Action = new InputSystem_Actions();
            Action.Enable();

            Action.Player.Enable();
            Action.Player.SetCallbacks(this);
        }

        private void OnDisable()
        {
            Action.Player.Disable();
            Action.Player.RemoveCallbacks(this);
        }
        #endregion

        #region LateUpdate Logic
        private void LateUpdate()
        {
            jumpPressed = false;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
            print(movementInput);
        }
        #endregion

        #region Input Callbacks
        public void OnSprint(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                print("sprintONNN");

                sprintToggledOn = holdToSprint || !sprintToggledOn;

                print(sprintToggledOn);
            }
            else if(context.canceled)
            {
                print("sprintOFFF");

                sprintToggledOn = !holdToSprint && sprintToggledOn;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(!context.performed) { return; }

            jumpPressed = true;
        }

        public void OnToggleWalk(InputAction.CallbackContext context)
        {
            if (!context.performed) { return; }

            walkToggledOn = !walkToggledOn;
        }

        public void OnAttack1(InputAction.CallbackContext context)
        {
        }

        public void OnAttack2(InputAction.CallbackContext context)
        {
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnNewaction(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }
        #endregion
    }


}
