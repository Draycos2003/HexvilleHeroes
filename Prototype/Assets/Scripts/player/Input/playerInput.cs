using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace FinalController
{
    [DefaultExecutionOrder(-2)]
    public class playerInput : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        private playerState playerState;
        private Animator animator;

        [Header("Combo Settings")]
        public int maxCombo = 3;
        public float comboResetTime = 1.0f;

        private int currentCombo = 0;
        private float lastAttackTime = 0f;
        private bool isAttacking = false;

        #region Locomotion Input Varibles
        [SerializeField] private bool holdToSprint = true; 
        public Vector2 movementInput { get; private set; }
        public Vector2 lookInput { get; private set; }
        public bool jumpPressed { get; private set; }
        public bool sprintToggledOn { get; private set; }
        public bool walkToggledOn { get; private set; }
        #endregion

        #region Combat Input Variables

        public bool attackPressed { get; private set; }
        public bool interactPressed { get; private set; }

        #endregion

        #region StartUp

        private void Awake()
        {
            playerState = GetComponent<playerState>();
            animator = GetComponent<Animator>();
        }

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

        #endregion

        #region Update Logic

        private void Update()
        {
            if(movementInput != Vector2.zero || 
                playerState.playerMovementStateCur == PlayerMovementState.Jumping ||
                playerState.playerMovementStateCur == PlayerMovementState.Falling)
            {
                interactPressed = false;
            }
        }

        #endregion

        #region Input Callbacks

        public void OnMove(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
        }

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
            if (!context.performed) { return; }

            if(Time.time - lastAttackTime > comboResetTime)
            {
                currentCombo = 0;
            }

            if (!animator.GetBool("isAttacking"))
            {
                currentCombo++;
                currentCombo = Mathf.Clamp(currentCombo, 1, maxCombo);
                attackPressed = true;
                lastAttackTime = Time.time;
            }
            else
            {
                if(currentCombo < maxCombo)
                {
                    currentCombo++;
                    currentCombo = Mathf.Clamp(currentCombo, 1, maxCombo);
                }
            }

        }

        public void OnAttack2(InputAction.CallbackContext context)
        {
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) { return; }

            interactPressed = true;

            print(interactPressed);
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

        #region Animation Events

        public void SetInteractPressedFalse()
        {
            interactPressed = false;
        }

        public void SetAttackPressedFalse()
        {
            attackPressed = false;
        }

        #endregion
    }


}
