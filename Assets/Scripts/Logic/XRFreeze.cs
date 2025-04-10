using UnityEngine;

public class XRFreeze : MonoBehaviour
{
    public Transform xrOrigin; // XR Origin como referencia
    public Vector3 minBounds = new Vector3(-0.5f, 0.8f, -0.5f); // Límite mínimo
    public Vector3 maxBounds = new Vector3(0.5f, 1.5f, 0.5f);  // Límite máximo

    private Vector3 initialLocalPosition;

    void Start()
    {
        if (xrOrigin != null)
        {
            initialLocalPosition = transform.localPosition; // Guarda la posición inicial relativa
        }
    }

    void LateUpdate()
    {
        if (xrOrigin != null)
        {
            // Obtiene la posición relativa al XR Origin
            Vector3 localPos = transform.localPosition;

            // Aplica los límites de movimiento
            localPos.x = Mathf.Clamp(localPos.x, minBounds.x, maxBounds.x);
            localPos.y = Mathf.Clamp(localPos.y, minBounds.y, maxBounds.y);
            localPos.z = Mathf.Clamp(localPos.z, minBounds.z, maxBounds.z);

            // Asigna la nueva posición dentro de los límites
            transform.localPosition = localPos;
        }
    }
}
