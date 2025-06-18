using UnityEngine;
using UnityEngine.InputSystem;

namespace FinalController
{
    [DefaultExecutionOrder(-2)]
    public class thirdPersonInput : MonoBehaviour, InputSystem_Actions.IThirdPersonActions
    {
        #region Class Variables
        
        public Vector2 scrollInput {  get; private set; }

        [SerializeField] thirdPersonCamera Camera;

        #endregion

        #region StartUp

        private void OnEnable()
        {
            if (playerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            playerInputManager.Instance.PlayerControls.ThirdPerson.Enable();
            playerInputManager.Instance.PlayerControls.ThirdPerson.SetCallbacks(this);
        }

        private void OnDisable()
        {
            playerInputManager.Instance.PlayerControls.ThirdPerson.Disable();
            playerInputManager.Instance.PlayerControls.ThirdPerson.RemoveCallbacks(this);
        }

        #endregion

        #region Update

        private void Update()
        {
            Camera.distance = Mathf.Clamp(Camera.distance + scrollInput.y * -1f, Camera.minDistance, Camera.minDistance);
        }

        private void LateUpdate()
        {
           scrollInput = Vector2.zero; 
        }

        #endregion

        #region Input Callbacks

        public void OnScrollCamera(InputAction.CallbackContext context)
        {
            if(!context.performed) { return; }

            scrollInput = context.ReadValue<Vector2>();
            scrollInput *= (Camera.zoomSpeed);
            print(scrollInput);
        }

        #endregion
    }


}
