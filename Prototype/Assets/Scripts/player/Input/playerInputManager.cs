using UnityEngine;

namespace FinalController
{
    [DefaultExecutionOrder(-3)]
    public class playerInputManager : MonoBehaviour
    {
        public static playerInputManager Instance;
        public InputSystem_Actions playerControls {  get; private set; }

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            playerControls = new InputSystem_Actions();
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
}
