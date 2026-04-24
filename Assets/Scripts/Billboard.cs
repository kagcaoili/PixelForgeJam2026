using UnityEngine;

/// <summary>
/// Quick billboard script to make text face the camera
/// </summary>
//[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    private Transform cam;

    private void Start() {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (cam == null) return;
        transform.forward = cam.forward;
    }
}