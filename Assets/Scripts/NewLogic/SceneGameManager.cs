using System.Collections.Generic;
using UnityEngine;

public class SceneGameManager : MonoBehaviour
{
    public static SceneGameManager Instance { get; private set; }

    [Header("Scriptables por país")] public List<SceneScriptable> chileScene;
    public List<SceneScriptable> argentinaScene;
    public List<SceneScriptable> peruScene;
    public List<SceneScriptable> australiaScene;

    [Header("AudioSources")] public AudioSource sfxAudioSource;
    public AudioSource narrationAudioSource;

    [Header("Posiciones clave")] public Transform defaultPlayerPosition;
    public Transform defaultAggressorPosition;
    public Transform questionPosition;

    [Header("Teleport Targets")] public Transform teleportTarget;

    [Header("Prefabs")] public GameObject likertPrefab;
    public GameObject questionPrefab;

    [SerializeField] Scripter sceneScripter;
    private void Awake()
    {
        // Garantiza que solo haya una instancia
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}