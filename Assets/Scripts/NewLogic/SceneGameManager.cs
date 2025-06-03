using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SceneGameManager : MonoBehaviour
{
    public static SceneGameManager Instance { get; private set; }

    [SerializeField] public GameObject[] reconObjects;
    public int indexRecon = 0;
    
    [Header("¿Usar escenas por idioma?")]
    public bool isLanguageScript;

    [Header("Scriptables por idioma")]
    public SceneScriptable spanishScenes;
    public SceneScriptable englishScenes;

    [Header("Scriptables por país")]
    public SceneScriptable chileScenesFem;
    public SceneScriptable chileScenesMale;
    public SceneScriptable argentinaScenesFem;
    public SceneScriptable argentinaScenesMale;
    public SceneScriptable peruScenesFem;
    public SceneScriptable peruScenesMale;
    public SceneScriptable australiaScenesFem;
    public SceneScriptable australiaScenesMale;

    [Header("AudioSources")]
    public AudioSource sfxAudioSource1;
    public AudioSource sfxAudioSource2;
    public AudioSource sfxAudioSource3;
    public AudioSource sfxAudioSource4;
    public AudioSource sfxTimer;
    public AudioSource narrationAudioSource;

    [Header("Posiciones clave")]
    public Transform defaultPlayerPosition;
    public Transform defaultAggressorPosition;
    public Transform questionPosition;

    [Header("Teleport Targets")]
    public Transform teleportTarget;

    [Header("Prefabs")]
    public GameObject likertPrefab;
    public GameObject questionPrefab;

    [SerializeField] Scripter sceneScripter;

    [SerializeField] private GameObject player;
    PlayerController playerController;
    
    private string selectedGender;

    [SerializeField] private TextMeshProUGUI narrativeText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
            
        EnsureAudioSources();
        
        LoadScriptableFromPrefs(); // 👈 Ejecuta directamente desde PlayerPrefs
    }

    private void Start()
    {
        player= GameObject.FindGameObjectWithTag("Player");
        player.transform.position = defaultPlayerPosition.position;
        playerController = player.GetComponent<PlayerController>();
        playerController.BlackScreen();
    }

    private void LoadScriptableFromPrefs()
    {
        SceneScriptable selected = null;
        selectedGender = PlayerPrefs.GetString("SelectedGender", "Femenino"); // Por defecto "Femenino"

        if (isLanguageScript)
        {
            string language = PlayerPrefs.GetString("SelectedLanguage", "Spanish");

            if (language == "English")
                sceneScripter.Execute(englishScenes);
            else
                sceneScripter.Execute(spanishScenes);
        }
        else
        {
            string country = PlayerPrefs.GetString("SelectedCountry", "Chile");

            switch (country)
            {
                case "Chile":
                    selected = selectedGender == "Masculino" ? chileScenesMale : chileScenesFem;
                    break;

                case "Argentina":
                    selected = selectedGender == "Masculino" ? argentinaScenesMale : argentinaScenesFem;
                    break;

                case "Perú":
                    selected = selectedGender == "Masculino" ? peruScenesMale : peruScenesFem;
                    break;
                case "Peru":
                    selected = selectedGender == "Masculino" ? peruScenesMale : peruScenesFem;
                    break;

                case "Australia":
                    selected = selectedGender == "Masculino" ? australiaScenesMale : australiaScenesFem;
                    break;

                default:
                    Debug.LogWarning($"⚠️ País no reconocido: {country}, usando Chile por defecto.");
                    selected = selectedGender == "Masculino" ? chileScenesMale : chileScenesFem;
                    break;
            }
            sceneScripter.Execute(selected);
        }
    }
    public void PlayerFadeTo(float alpha, float duration, TweenCallback onComplete)
    {
        playerController.SetAlpha(alpha, duration, onComplete);
    }

    public void SetText(string text)
    {
        narrativeText.text = text;
    }

    public void StartNarration(AudioClip clip)
    {
        narrationAudioSource.clip = clip;
        narrationAudioSource.Play();
    }
    private void EnsureAudioSources()
    {
        sfxAudioSource1 ??= gameObject.AddComponent<AudioSource>();
        sfxAudioSource2 ??= gameObject.AddComponent<AudioSource>();
        sfxAudioSource3 ??= gameObject.AddComponent<AudioSource>();
        sfxAudioSource4 ??= gameObject.AddComponent<AudioSource>();
        sfxTimer ??= gameObject.AddComponent<AudioSource>();
        narrationAudioSource ??= gameObject.AddComponent<AudioSource>();
    }
}