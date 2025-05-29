using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioStep", menuName = "SceneSteps/Audio")]
public class AudioStep : ScriptPartSO
{
    public AudioClip audioClip;
    
  
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(name) || name.StartsWith("New ")) 
        {
            stepType = StepType.Audio;
        }
    }
    
    
    
    public override IEnumerator WaitForCompletion()
    {
        if (audioClip == null)
        {
            yield return new WaitForSeconds(extraDelay);
            yield break;
        }

        AudioSource audioSource = SceneGameManager.Instance.narrationAudioSource;
        audioSource.clip = audioClip;
        audioSource.Play();

        yield return new WaitWhile(() => audioSource.isPlaying);
        if (extraDelay > 0)
            yield return new WaitForSeconds(extraDelay);
    }
}
