using UnityEngine;
using System.Collections;


public class CameraFacing : MonoBehaviour
{
    private enum FacingType
    {
        BILLBOARD = 0,
        LOOK_AT = 1
    }

    private Camera cam;
    [SerializeField]
    private FacingType facingType;

    private void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (Camera.main == null) return;
        transform.LookAt((facingType == FacingType.BILLBOARD?transform.position + cam.transform.rotation * Vector3.forward: cam.transform.position),
            cam.transform.rotation * Vector3.up);
    }
}