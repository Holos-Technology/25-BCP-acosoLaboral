using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    [Header("Referencias")]
    public Transform leftEye;
    public Transform rightEye;

  
    [Header("Jitter (realismo)")]
    public float jitterAmount = 0.03f;
    public float jitterSpeed = 3f;

    [Header("Rotación")]
    public float rotationSpeed = 5f;

    [Header("Offset vertical (altura mirada)")]
    public float verticalLookOffset = 0.5f;

    private Vector3 jitterOffset;
    private Vector3 targetPosition;
    private Transform headTransform;

    void Start()
    {
        GameObject headObj = GameObject.FindGameObjectWithTag("Head");
        if (headObj != null)
        {
            headTransform = headObj.transform;
            targetPosition = headTransform.position + new Vector3(0f, verticalLookOffset, 0f);
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con tag 'Head'");
        }
    }

    void Update()
    {
        if (headTransform == null || !headTransform.gameObject.activeInHierarchy) return;

        Vector3 desiredPosition = headTransform.position + new Vector3(0f, verticalLookOffset, 0f);

        // Solo actualiza si la posición no está en (0, 0, 0)
        if (headTransform.position != Vector3.zero)
        {
            jitterOffset = Vector3.Lerp(jitterOffset, Random.insideUnitSphere * jitterAmount, Time.deltaTime * jitterSpeed);
            targetPosition = Vector3.Lerp(targetPosition, desiredPosition + jitterOffset, Time.deltaTime * jitterSpeed);
        }
    }

    void LateUpdate()
    {
        if (leftEye != null)
        {
            Vector3 leftTarget = targetPosition + new Vector3(-0.01f, 0f, 0f);
            Quaternion lookRot = Quaternion.LookRotation(leftTarget - leftEye.position);
            leftEye.rotation = Quaternion.Slerp(leftEye.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }

        if (rightEye != null)
        {
            Vector3 rightTarget = targetPosition + new Vector3(0.01f, 0f, 0f);
            Quaternion lookRot = Quaternion.LookRotation(rightTarget - rightEye.position);
            rightEye.rotation = Quaternion.Slerp(rightEye.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }
    void OnEnable()
    {
        if (headTransform == null)
        {
            GameObject headObj = GameObject.FindGameObjectWithTag("Head");
            if (headObj != null)
            {
                headTransform = headObj.transform;
            }  
            if (leftEye != null) leftEye.localRotation = Quaternion.identity;
            if (rightEye != null) rightEye.localRotation = Quaternion.identity;
        }
    }
}
