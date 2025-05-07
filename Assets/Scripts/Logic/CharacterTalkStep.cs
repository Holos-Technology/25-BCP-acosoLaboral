using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterTalkStep : MonoBehaviour, IStep
{
    [Header("Audio Config")]
    [SerializeField] private AudioSource characterAudioSource;

    [Header("Audios por País")]
    [SerializeField] private List<CountryAudioData> countryAudios;

    public UnityEvent onStartStep;

    public IEnumerator Execute()
    {
        onStartStep?.Invoke();

        if (characterAudioSource == null)
        {
            Debug.LogWarning("No se asignó un AudioSource en CharacterTalkStep.");
            yield break;
        }

        // Obtener el AudioClip correspondiente
        AudioClip selectedClip = GetAudioClipByCountry();

        if (selectedClip == null)
        {
            Debug.LogWarning("No hay AudioClip asignado para el país seleccionado.");
            yield break;
        }

        characterAudioSource.clip = selectedClip;
        characterAudioSource.Play();

        yield return new WaitForSeconds(selectedClip.length);
    }

    private AudioClip GetAudioClipByCountry()
    {
        string country = PlayerPrefs.GetString("SelectedCountry", "Australia").ToLower();

        foreach (var entry in countryAudios)
        {
            if (entry.countryKey.ToLower() == country)
                return entry.audioClip;
        }

        return null;
    }

}
[System.Serializable]
public class CountryAudioData
{
    public string countryKey;
    public AudioClip audioClip;
}