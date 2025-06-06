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
        if (playerTransform == null || audioSource == null || narrationAudio == null)
        {
            Debug.LogWarning("Faltan referencias en NarrationStep.");
            yield break;
        }

        playerTransform.position = startPosition;
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "Chile");
        audioSource.clip = selectedCountry == "Australia" ? englishNarrationAudio : narrationAudio;      
        audioSource.Play();
        yield return new WaitForSeconds(narrationAudio.length);
    }

    public void LoadText()
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "Chile");
        narrationtext.text = selectedCountry == "Australia" ? englishNarrationText : narrationText;
    }
}
