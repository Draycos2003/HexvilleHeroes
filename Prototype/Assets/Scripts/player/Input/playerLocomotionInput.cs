using UnityEngine;
using UnityEngine.InputSystem;

namespace FinalController
{
    [DefaultExecutionOrder(-2)]
    public class playerLocomotionInput : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        #region Class Variables
        [SerializeField] private bool holdToSprint = true; 
        public Vector2 movementInput { get; private set; }
        public Vector2 lookInput { get; private set; }
        public bool jumpPressed { get; private set; }
        public bool sprintToggledOn { get; private set; }
        public bool walkToggledOn { get; private set; }
        #endregion

        #region StartUp
        private void OnEnable()
        {
            if(playerInputManager.Instance?.playerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            playerInputManager.Instance.playerControls.Player.Enable();
            playerInputManager.Instance.playerControls.Player.SetCallbacks(this);
        }

        private void OnDisable()
        {
            playerInputManager.Instance.playerControls.Player.Disable();
            playerInputManager.Instance.playerControls.Player.RemoveCallbacks(this);
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
        }
        #endregion

        #region Input Callbacks
        public void OnSprint(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                sprintToggledOn = holdToSprint || !sprintToggledOn;
            }
            else if(context.canceled)
            {
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
