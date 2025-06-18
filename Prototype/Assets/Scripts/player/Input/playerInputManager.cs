using UnityEngine;

namespace FinalController
{
    [DefaultExecutionOrder(-3)]
    public class playerInputManager : MonoBehaviour
    {
        public static playerInputManager Instance;
        public InputSystem_Actions PlayerControls {  get; private set; }

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
            PlayerControls = new InputSystem_Actions();
            PlayerControls.Enable();
        }

        private void OnDisable()
        {
            PlayerControls.Disable();
        }
    }
}
