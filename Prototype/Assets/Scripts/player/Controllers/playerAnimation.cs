using System;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace FinalController
{
    public class playerAnimation : MonoBehaviour
    {
        #region Main Fields

        [SerializeField] private Animator animator;
        [SerializeField] private float locomotionBlendSpeed;
        [SerializeField] private playerController pc;

        private playerInput playerInput;
        private playerState playerState;

        #endregion

        #region Player Input Direction

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");

        #endregion

        #region Locomotion Input Hashes

        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        private static int isIdlingHash = Animator.StringToHash("isIdling");
        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        private static int isJumpingHash = Animator.StringToHash("isJumping");

        #endregion

        #region Camera Rotation Hashes

        private static int isRotatingHash = Animator.StringToHash("isRotating");
        private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");

        #endregion

        #region Action Input Hashes

        private static int isAttackingHash = Animator.StringToHash("isAttacking");
        private static int isRangedHash = Animator.StringToHash("isRanged");
        private static int isReloadingHash = Animator.StringToHash("isReloading");
        private static int canComboHash = Animator.StringToHash("canCombo");
        private static int isInteractingHash = Animator.StringToHash("isInteracting");
        private static int isPlayingActionHash = Animator.StringToHash("isPlayingAction");
        private int[] actionHashes;

        #endregion

        #region Input Control Variables

        private Vector3 blendInputCur;

        private float sprintMaxBlendValue = 1.5f;
        private float runMaxBlendValue = 1f;
        private float walkMaxBlendValue = 0.5f;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            playerInput = GetComponent<playerInput>();
            playerState = GetComponent<playerState>();
            pc = GetComponent<playerController>();

            actionHashes = new int[] { isInteractingHash };
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        #endregion

        private void UpdateAnimationState()
        {
            bool isIdling = playerState.playerMovementStateCur == PlayerMovementState.Idling;
            bool isRunning = playerState.playerMovementStateCur == PlayerMovementState.Running;
            bool isSprinting = playerState.playerMovementStateCur == PlayerMovementState.Sprinting;
            bool isJumping = playerState.playerMovementStateCur == PlayerMovementState.Jumping;
            bool isFalling = playerState.playerMovementStateCur == PlayerMovementState.Falling;
            bool isGrounded = playerState.isGroundedState();
            bool isPlayingAction = actionHashes.Any(hash => animator.GetBool(hash));

            bool isRunBlendValue = isRunning || isJumping || isFalling;
            Vector2 inputTarget = isSprinting ? playerInput.movementInput * sprintMaxBlendValue :
                                  isRunBlendValue ? playerInput.movementInput * runMaxBlendValue : playerInput.movementInput * walkMaxBlendValue;

            blendInputCur = Vector3.Lerp(blendInputCur, inputTarget, locomotionBlendSpeed * Time.deltaTime);

            // Locomotion
            animator.SetBool(isIdlingHash, isIdling);
            animator.SetBool(isGroundedHash, isGrounded);
            animator.SetBool(isFallingHash, isFalling);
            animator.SetBool(isJumpingHash, isJumping);
            animator.SetBool(isRotatingHash, pc.isRotating);

            // Actions
            animator.SetBool(isAttackingHash, playerInput.attackPressed || playerInput.rangedAttack);
            animator.SetBool(isRangedHash, playerInput.rangedAttack);
            animator.SetBool(isReloadingHash, playerInput.isReloading);
            animator.SetBool(isInteractingHash, playerInput.interactPressed);
            animator.SetBool(isPlayingActionHash, isPlayingAction);
            

            // Input
            animator.SetFloat(inputXHash, blendInputCur.x);
            animator.SetFloat(inputYHash, blendInputCur.y);
            animator.SetFloat(inputMagnitudeHash, blendInputCur.magnitude);
            animator.SetFloat(rotationMismatchHash, pc.rotationMismatch);
        }
    }
}

