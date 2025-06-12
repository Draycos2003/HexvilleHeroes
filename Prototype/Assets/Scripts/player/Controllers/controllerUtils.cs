using UnityEngine;

namespace FinalController
{
    public static class controllerUtils
    {
        public static Vector3 GetNormalWithSphereCast(CharacterController controller, LayerMask layerMask = default)
        {
            Vector3 normal = Vector3.up;
            Vector3 center = controller.transform.position + controller.center;
            float distance = (controller.height / 2f) + controller.stepOffset + 0.01f;

            RaycastHit hit;
            if(Physics.SphereCast(center, controller.radius, Vector3.down, out hit, distance, layerMask))
            {
                normal = hit.normal;
            }
            return normal;
        }
    }
}
