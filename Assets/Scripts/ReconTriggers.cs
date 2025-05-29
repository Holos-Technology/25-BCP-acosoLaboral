using System;
using UnityEngine;

public class ReconTriggers : MonoBehaviour
{
    public event Action OnTriggered;
    [SerializeField] private string tagName = "recognize trigger";
    
    [Header("Dirección de aparición")]
    public bool spawnLeft = false;
    public bool spawnRight = false;

    [SerializeField] private float spawnDistance = 1f;
    private void OnEnable()
    {
        if (spawnLeft)
        {
            SpawnLeft();
        }
        else if (spawnRight)
        {
            SpawnRight();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("recognize trigger")) 
        {
            Debug.Log("[RECON] Trigger activado");
            OnTriggered?.Invoke();
        }
    }

    public void SpawnRight()
    {
        Transform cam = Camera.main.transform;
        Vector3 newPos = cam.position + cam.right * spawnDistance;
        transform.position = newPos;
        transform.LookAt(cam);
    }

    public void SpawnLeft()
    {
        Transform cam = Camera.main.transform;
        Vector3 newPos = cam.position - cam.right * spawnDistance;
        transform.position = newPos;
        transform.LookAt(cam);
    }
}
