using UnityEngine;

[CreateAssetMenu(fileName = "AudioStep", menuName = "Acoso Laboral/Audio")]
public class AudioStep : ScriptPartSO
{
    public AudioClip audioClip;

    public override float GetWaitTime()
    {
        return (audioClip != null ? audioClip.length : 0f) + extraDelay;
    }
}
