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

