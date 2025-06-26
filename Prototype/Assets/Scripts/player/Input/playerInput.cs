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
        private bool queuedNextAttack = false;

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
        public bool rangedAttack { get; private set; }
        public bool isReloading { get; private set; }
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
            if(playerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            playerInputManager.Instance.PlayerControls.Player.Enable();
            playerInputManager.Instance.PlayerControls.Player.SetCallbacks(this);
        }

        private void OnDisable()
        {
            playerInputManager.Instance.PlayerControls.Player.Disable();
            playerInputManager.Instance.PlayerControls.Player.RemoveCallbacks(this);
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

        public void OnAttack1(InputAction.CallbackContext context)
        {
            if (!context.performed) { return; }

            if (gamemanager.instance.activeMenu != null) { return; }

            if (playerController.instance.isRanged)
            {
                rangedAttack = true;
                attackPressed = true;
                return;
            }

            if (Time.time - lastAttackTime > comboResetTime)
            {
                currentCombo = 0;
            }

            if (!animator.GetBool("isAttacking"))
            {
                // start combo
                currentCombo = 1;
                PlayCombo(currentCombo);
            }
            else
            {
                // queue next attack
                if (currentCombo < maxCombo)
                {
                    queuedNextAttack = true;
                }
            }

        }

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
            isReloading = false;
       
            if (queuedNextAttack && currentCombo < maxCombo)
            {
                currentCombo++;
                queuedNextAttack = false;
                PlayCombo(currentCombo);
            }
            else
            {
                currentCombo = 0;
                queuedNextAttack = false;
                attackPressed = false;
                animator.SetInteger("comboStep", 0);
            }
        }

        public void SetRangedAttackFalse()
        {
            rangedAttack = false;
            isReloading = true;
        }

        private void PlayCombo(int comboStep)
        {
            lastAttackTime = Time.time;

            attackPressed = true;
            animator.SetInteger("comboStep", comboStep);
        }

        #endregion
    }


}
