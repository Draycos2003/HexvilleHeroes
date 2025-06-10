using System;
using UnityEngine;

namespace FinalController
{
    public class playerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float locomotionBlendSpeed;
        [SerializeField] private playerController pc;


        private playerLocomotionInput playerLocomotionInput;
        private playerState playerState;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        public static int isIdlingHash = Animator.StringToHash("isIdling");
        public static int isGroundedHash = Animator.StringToHash("isGrounded");
        public static int isFallingHash = Animator.StringToHash("isFalling");
        public static int isJumpingHash = Animator.StringToHash("isJumping");
        public static int isRotatingHash = Animator.StringToHash("isRotating");
        public static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");

        private Vector3 blendInputCur;

        private void Awake()
        {
            playerLocomotionInput = GetComponent<playerLocomotionInput>();
            playerState = GetComponent<playerState>();
            pc = GetComponent<playerController>();
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            bool isIdling = playerState.playerMovementStateCur == PlayerMovementState.Idling;
            bool isRunning = playerState.playerMovementStateCur == PlayerMovementState.Running;
            bool isSprinting = playerState.playerMovementStateCur == PlayerMovementState.Sprinting;
            bool isJumping = playerState.playerMovementStateCur == PlayerMovementState.Jumping;
            bool isFalling = playerState.playerMovementStateCur == PlayerMovementState.Falling;
            bool isGrounded = playerState.isGroundedState();

            Vector2 inputTarget = isSprinting ? playerLocomotionInput.movementInput * 1.5f : playerLocomotionInput.movementInput;
            blendInputCur = Vector3.Lerp(blendInputCur, inputTarget, locomotionBlendSpeed * Time.deltaTime);

            animator.SetBool(isIdlingHash, isIdling);
            animator.SetBool(isGroundedHash, isGrounded);
            animator.SetBool(isFallingHash, isFalling);
            animator.SetBool(isJumpingHash, isJumping);
            animator.SetBool(isRotatingHash, pc.isRotating);

            animator.SetFloat(inputXHash, blendInputCur.x);
            animator.SetFloat(inputYHash, blendInputCur.y);
            animator.SetFloat(inputMagnitudeHash, blendInputCur.magnitude);
            animator.SetFloat(rotationMismatchHash, pc.rotationMismatch);
        }
    }
}

