using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterTalkStep : MonoBehaviour, IStep
{
    [Header("Audio Config")]
    [SerializeField] private AudioSource characterAudioSource;
    
    [Header("Audio Clips por País")]
    [SerializeField] private AudioClip peruvianAudio;
    [SerializeField] private AudioClip chileanAudio;
    [SerializeField] private AudioClip colombianAudio;
    [SerializeField] private AudioClip argentinianAudio;


    public UnityEvent onStartStep;

    public IEnumerator Execute()
    {
        if (characterAudioSource == null)
        {
            Debug.LogWarning("No se asignó un AudioSource en CharacterTalkStep.");
            yield break;
        }

        // Obtener el AudioClip correcto según la nacionalidad
        AudioClip selectedClip = GetCharacterAudio();

        if (selectedClip == null)
        {
            Debug.LogWarning("No hay AudioClip asignado para este país.");
            yield break;
        }

        // Reproducir el audio
        characterAudioSource.clip = selectedClip;
        characterAudioSource.Play();

        // Esperar hasta que termine el audio
        yield return new WaitForSeconds(selectedClip.length);
    }

    private AudioClip GetCharacterAudio()
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "Chile"); // Chile por defecto

        switch (selectedCountry)
        {
            case "Peru" or "Perú": return peruvianAudio;
            case "Colombia": return colombianAudio;
            case "Argentina": return argentinianAudio;
            default: return chileanAudio; // Chile en todos los demás casos
        }
    }

}
