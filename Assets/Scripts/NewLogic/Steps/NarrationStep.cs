using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NarrationStep", menuName = "SceneSteps/Narration")]
public class NarrationStep : ScriptPartSO
{
    [TextArea]
    public string text;
    public string speaker;
    public AudioClip clip;
    
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(name) || name.StartsWith("New ")) 
        {
            stepType = StepType.Narration;
        }
    }

    public override IEnumerator WaitForCompletion()
    {
        if (clip != null)
        {
            var audioSource = SceneGameManager.Instance.narrationAudioSource;

            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitForSeconds(clip.length);
        }

        if (extraDelay > 0)
            yield return new WaitForSeconds(extraDelay);
    }
}