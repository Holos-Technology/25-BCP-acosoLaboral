using UnityEngine;

public class HandOffsetCorrection : MonoBehaviour
{
    public Transform xrRig; // El XR Rig o la cámara
    public Vector3 offset; // Ajusta este valor según necesites

    void Update()
    {
        transform.position = xrRig.position + offset;
    }

}
