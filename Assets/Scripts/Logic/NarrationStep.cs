using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class NarrationStep : MonoBehaviour, IStep
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private AudioClip narrationAudio;
    [SerializeField] private AudioClip englishNarrationAudio;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI  narrationtext;
    [SerializeField] private string englishNarrationText;
    [SerializeField] private string narrationText;

    public UnityEvent onStartStep { get; }
    public UnityEvent OnStartStep => onStartStep; // Implementación de la interfaz


    public IEnumerator Execute()
    {
        if (playerTransform == null || audioSource == null)
        {
            Debug.LogWarning("Faltan referencias en NarrationStep.");
            yield break;
        }

        playerTransform.position = startPosition;
        string selectedLanguage = PlayerPrefs.GetString("language", "es");
        bool isEnglish = selectedLanguage == "en";

        AudioClip clipToPlay = isEnglish ? englishNarrationAudio : narrationAudio;

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
            yield return new WaitForSeconds(clipToPlay.length);
        }
    }

    public void LoadText()
    {
        if (narrationtext != null)
        {
            string selectedLanguage = PlayerPrefs.GetString("language", "es");
            narrationtext.text = selectedLanguage == "en" ? englishNarrationText : narrationText;
        }
    }
}
