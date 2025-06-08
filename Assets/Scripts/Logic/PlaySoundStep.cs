using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlaySoundStep : MonoBehaviour,IStep
{  
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [FormerlySerializedAs("clipToPlay")] [SerializeField] private AudioClip clipToPlaySpanish;
    [SerializeField] private AudioClip clipToPlayEnglish;

    [SerializeField] private float extraWaitTime = 3f; // Tiempo extra después de que termine el audio
    
    public UnityEvent OnFinished;
    public UnityEvent OnStart;
    
    private bool playing = false;
    private bool waitingForTrigger = false;
    [Header("Trigger Settings")]
    [SerializeField] private bool requiresTrigger = true; // Si es falso, no espera el trigger y ejecuta normalmente
    [SerializeField] private string requiredTriggerTag = "recognize trigger"; // Tag del trigger esperado

    [Header("Skippable Settings")]
    [SerializeField] private bool isSkippable = false;
    [SerializeField] private float skipDelay = 5f; // tiempo para saltar si no hay interacción
    private Coroutine skipCoroutine;

      private AudioClip SelectedClip
    {
        get
        {
            string selectedLanguage = PlayerPrefs.GetString("SelectedLanguage", "Spanish");
            string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "Chile"); // Default Chile

            // Solo usar el audio en inglés si es Australia + Inglés
            if (selectedLanguage == "English" && selectedCountry == "Australia" && clipToPlayEnglish != null)
            {
                return clipToPlayEnglish;
            }
            else
            {
                return clipToPlaySpanish;
            }
        }
    }

    public void StartPlayingAudioOnLoop()
    {
        if (audioSource == null || SelectedClip == null)
        {
            Debug.LogWarning("PlaySoundStep: No audio source or clip assigned.");
            return;
        }

        audioSource.loop = true;
        audioSource.clip = SelectedClip;
        audioSource.Play();
    }

    public IEnumerator Execute()
    {
        if (SelectedClip == null || audioSource == null)
        {
            Debug.LogWarning("PlaySoundStep: No audio source or clip assigned.");
            yield break;
        }

        playing = true;
        OnStart?.Invoke();

        audioSource.loop = false;
        audioSource.clip = SelectedClip;
        audioSource.Play();

        yield return new WaitForSeconds(SelectedClip.length + extraWaitTime);

        if (requiresTrigger)
        {
            waitingForTrigger = true;
            Debug.Log("Esperando trigger para continuar...");

            if (isSkippable)
            {
                skipCoroutine = StartCoroutine(SkipAfterDelay());
            }

            while (waitingForTrigger)
            {
                yield return null;
            }

            if (skipCoroutine != null)
            {
                StopCoroutine(skipCoroutine);
                skipCoroutine = null;
            }
        }

        playing = false;
        OnFinished?.Invoke();
    }

    private IEnumerator SkipAfterDelay()
    {
        float elapsed = 0f;
        while (elapsed < skipDelay)
        {
            if (!waitingForTrigger) yield break; // Ya se activó el trigger
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (waitingForTrigger)
        {
            Debug.Log("Tiempo de espera agotado, saltando al siguiente paso.");
            waitingForTrigger = false;
        }
    }   

    private void TryToContinue(Collider other)
    {
        if (requiresTrigger && waitingForTrigger && !string.IsNullOrEmpty(requiredTriggerTag) && other.CompareTag(requiredTriggerTag))
        {
            waitingForTrigger = false;
            Debug.Log($"Trigger '{requiredTriggerTag}' reconocido, ejecutando siguiente paso.");
        }
    }

    private void OnTriggerEnter(Collider other) => TryToContinue(other);
    private void OnTriggerStay(Collider other) => TryToContinue(other);
}
