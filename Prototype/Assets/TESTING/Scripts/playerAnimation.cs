using System;
using UnityEngine;

namespace FinalController
{
    public class playerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float locomotionBlendSpeed;


        private playerLocomotionInput playerLocomotionInput;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");

        private Vector3 blendInputCur;

        private void Awake()
        {
            playerLocomotionInput = GetComponent<playerLocomotionInput>();
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            Vector2 inputTarget = playerLocomotionInput.movementInput;
            blendInputCur = Vector3.Lerp(blendInputCur, inputTarget, locomotionBlendSpeed * Time.deltaTime);
            
            animator.SetFloat(inputXHash, blendInputCur.x);
            animator.SetFloat(inputYHash, blendInputCur.y);
        }
    }
}

