using UnityEngine;

namespace FinalController
{
    public class playerState : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovementState playerMovementStateCur { get; private set; } = PlayerMovementState.Idling;

        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            playerMovementStateCur = playerMovementState;
        }

        public bool isGroundedState()
        {
            return playerMovementStateCur == PlayerMovementState.Idling ||
                playerMovementStateCur == PlayerMovementState.Walking ||
                playerMovementStateCur == PlayerMovementState.Running ||
                playerMovementStateCur == PlayerMovementState.Sprinting;
        }
    }

    public enum PlayerMovementState
    {
        Idling,
        Walking,
        Running,
        Sprinting,
        Jumping,
        Falling,
        Strafing,
    }
}

