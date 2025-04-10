using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AggresorStep : MonoBehaviour ,IStep
{   
     [Header("Animator Settings")]
    [SerializeField] private string animationTrigger;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip peruvianAggressorAudio;
    [SerializeField] private AudioClip chileanAggressorAudio;
    [SerializeField] private AudioClip colombianAggressorAudio;
    [SerializeField] private AudioClip argentinianAggressorAudio;

    [Header("Prefab Settings")]
    [SerializeField] private GameObject peruvianAggressorPrefab;
    [SerializeField] private GameObject chileanAggressorPrefab;
    [SerializeField] private GameObject colombianAggressorPrefab;
    [SerializeField] private GameObject argentinianAggressorPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Settings")]
    [SerializeField] private bool destroyAggressorOnFinish = false; // ✅ Nueva opción para destruir el agresor después de Execute()

    [Header("Events")]
    [SerializeField] private UnityEvent OnPlayStepEvent;

    public UnityEvent onStartStep { get; }
    public UnityEvent OnStartStep => onStartStep; // Implementación de la interfaz
    
    private GameObject spawnedAggressor;

    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("🚨 `spawnPoint` no está asignado en " + gameObject.name);
            return;
        }

        // ✅ Buscar si ya hay un agresor en `spawnPoint`
        if (spawnPoint.childCount > 0)
        {
            spawnedAggressor = spawnPoint.GetChild(0).gameObject;
        }
        else
        {
            spawnedAggressor = SpawnAggressorPrefab();
        }

        if (spawnedAggressor != null)
        {
            spawnedAggressor.SetActive(true);
            Debug.Log("✅ Agresor asignado: " + spawnedAggressor.name);
        }
        else
        {
            Debug.LogError("🚨 No se pudo generar o encontrar un agresor.");
        }
    }


    public IEnumerator Execute()
    {
        OnPlayStepEvent?.Invoke();
        if (audioSource == null || spawnPoint == null)
        {
            Debug.LogWarning("⚠ Faltan referencias en `AggresorStep`.");
            yield break;
        }

        if (spawnedAggressor == null)
        {
            Debug.LogError("🚨 No se encontró un agresor en `spawnPoint`.");
            yield break;
        }

        // ✅ Activar el agresor si estaba desactivado
        spawnedAggressor.SetActive(true);
        Debug.Log("🎭 Agresor activado en `Execute()`.");

        // Configurar animación del agresor
        Animator aggressorAnimator = spawnedAggressor.GetComponentInChildren<Animator>();
        if (aggressorAnimator != null && animationTrigger != "")
        {
            aggressorAnimator.SetTrigger(animationTrigger);
        }

        // Reproducir audio del agresor según el país
        AudioClip clip = GetAggressorAudio();
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
        }

        // ✅ Si está activado `destroyAggressorOnFinish`, destruir el agresor al terminar `Execute()`
        if (destroyAggressorOnFinish)
        {
            Debug.Log("🗑️ Destruyendo agresor: " + spawnedAggressor.name);
            Destroy(spawnedAggressor);
            spawnedAggressor = null;
        }
    }

    private AudioClip GetAggressorAudio()
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "Chile");

        return selectedCountry switch
        {
            "Peru" or "Perú" => peruvianAggressorAudio,
            "Colombia" => colombianAggressorAudio,
            "Argentina" => argentinianAggressorAudio,
            _ => chileanAggressorAudio
        };
    }

    private GameObject SpawnAggressorPrefab()
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "Chile");
        GameObject prefabToSpawn = selectedCountry switch
        {
            "Peru" or "Perú"  => peruvianAggressorPrefab,
            "Colombia" => colombianAggressorPrefab,
            "Argentina" => argentinianAggressorPrefab,
            _ => chileanAggressorPrefab
        };

        if (prefabToSpawn == null) return null;

        // ✅ Crear agresor en `spawnPoint`
        GameObject newAggressor = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        newAggressor.transform.SetParent(spawnPoint, false);
        newAggressor.transform.localPosition = Vector3.zero;
        newAggressor.transform.localRotation = Quaternion.identity;
        newAggressor.transform.localScale = Vector3.one;

        Debug.Log("✨ Agresor generado en (0,0,0): " + newAggressor.name);
        return newAggressor;
    }
}
