using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AggressorData
{
    public string countryKey;
    public GameObject prefab;
    public AudioClip audioClip;
}

public class AggresorStep : MonoBehaviour, IStep
{
    [Header("Animator Settings")]
    [SerializeField] private string animationTrigger;
    [SerializeField] private bool lookInChild = false; // Nuevo bool agregado
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;

    [Header("Aggressor Settings")]
    [SerializeField] private List<AggressorData> aggressorDataList;
    [SerializeField] private Transform spawnPoint;

    [Header("Settings")]
    [SerializeField] private bool destroyAggressorOnFinish = false;

    [Header("Events")]
    [SerializeField] private UnityEvent OnPlayStepEvent;

    public UnityEvent onStartStep { get; }
    public UnityEvent OnStartStep => onStartStep;

    private GameObject spawnedAggressor;
    private Dictionary<string, AggressorData> aggressorDataDict;

    private void Awake()
    {
        aggressorDataDict = new Dictionary<string, AggressorData>();

        foreach (var data in aggressorDataList)
        {
            if (!string.IsNullOrEmpty(data.countryKey) && data.prefab != null)
            {
                aggressorDataDict[data.countryKey.ToLower()] = data;
            }
        }
    }

    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("🚨 `spawnPoint` no está asignado en " + gameObject.name);
            return;
        }

        // Ver si ya hay un agresor existente en el spawnPoint
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

        // Activar agresor
        spawnedAggressor.SetActive(true);
        Debug.Log("🎭 Agresor activado en `Execute()`.");

        // Activar animación si aplica
        Animator aggressorAnimator = null;

        if (lookInChild && spawnedAggressor.transform.childCount > 0)
        {
            aggressorAnimator = spawnedAggressor.transform.GetChild(0).GetComponent<Animator>();
        }
        else
        {
            aggressorAnimator = spawnedAggressor.GetComponentInChildren<Animator>();
        }

        if (aggressorAnimator != null && !string.IsNullOrEmpty(animationTrigger))
        {
            aggressorAnimator.SetTrigger(animationTrigger);
        }
        // Reproducir audio si aplica
        AudioClip clip = GetAggressorAudio();
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
        }

        // Destruir agresor si se indicó
        if (destroyAggressorOnFinish)
        {
            Debug.Log("🗑️ Destruyendo agresor: " + spawnedAggressor.name);
            Destroy(spawnedAggressor);
            spawnedAggressor = null;
        }
    }

    private AudioClip GetAggressorAudio()
    {
        string key = PlayerPrefs.GetString("SelectedCountry", "Australia").ToLower();
        return aggressorDataDict.TryGetValue(key, out var data) ? data.audioClip : null;
    }

    private GameObject SpawnAggressorPrefab()
    {
        string key = PlayerPrefs.GetString("SelectedCountry", "Australia").ToLower();

        if (aggressorDataDict.TryGetValue(key, out var data) && data.prefab != null)
        {
            GameObject newAggressor = Instantiate(data.prefab, spawnPoint.position, spawnPoint.rotation);
            newAggressor.transform.SetParent(spawnPoint, false);
            newAggressor.transform.localPosition = Vector3.zero;
            newAggressor.transform.localRotation = Quaternion.identity;
            newAggressor.transform.localScale = Vector3.one;

            Debug.Log("✨ Agresor generado: " + newAggressor.name);
            return newAggressor;
        }

        return null;
    }
}