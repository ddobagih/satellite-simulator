using UnityEngine;

public class BillboardLabel : MonoBehaviour
{
    private Camera targetCamera;

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
    }
}
