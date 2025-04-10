    using UnityEngine;

    [System.Serializable]
    public class VRMap
    {
        public Transform vrTarget;
        public Transform ikTarget;
        public Vector3 trackingPositionOffset;
        public Vector3 trackingRotationOffset;
        public bool isTracking = true;
        public bool onlyRotateWithCamera = false; // NUEVO

        public void Map()
        {
            if (!isTracking || vrTarget == null) return;

            ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);

            if (onlyRotateWithCamera)
            {
                // Rotación exacta de la cámara (para la cabeza)
                ikTarget.rotation = Camera.main.transform.rotation * Quaternion.Euler(trackingRotationOffset);
            }
            else
            {
                // Rotación normal para manos u otros
                ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
            }
        }
    }

    public class IKTargetFollowVRRig : MonoBehaviour
    {
        [Range(0,1)]
        public float turnSmoothness = 0.1f;
        public VRMap head;
        public VRMap leftHand;
        public VRMap rightHand;
        public VRMap leftHandTrack;
        public VRMap rightHandTrack;
        
        public Vector3 headBodyPositionOffset;
        public float headBodyYawOffset;

        // Update is called once per frame
      
        void LateUpdate()
        {
            // Primero: mover el cuerpo a la posición de la cabeza + offset
            transform.position = head.ikTarget.position + headBodyPositionOffset;

            // Obtener la rotación de la cámara (local player)
            Quaternion cameraRotation = Camera.main.transform.rotation;
            float cameraYaw = cameraRotation.eulerAngles.y;

            // Solo rotar el cuerpo en Y hacia la cámara (con suavizado)
            Quaternion targetBodyRotation = Quaternion.Euler(0f, cameraYaw + headBodyYawOffset, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetBodyRotation, turnSmoothness);

            // Actualizar mapeo de cabeza
            head.onlyRotateWithCamera = true; // importante
            head.Map();

            // Manos
            if (leftHand.vrTarget.gameObject.activeSelf)
            {
                leftHand.isTracking = true;
                leftHand.Map();
            }
            else
            {
                leftHand.isTracking = false;
                leftHandTrack.Map();
            }

            if (rightHand.vrTarget.gameObject.activeSelf)
            {
                rightHand.isTracking = true;
                rightHand.Map();
            }
            else
            {
                rightHand.isTracking = false;
                rightHandTrack.Map();
            }
        }
    }
