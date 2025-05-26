using UnityEngine;


[CreateAssetMenu(fileName = "FadeInStep", menuName = "Acoso Laboral/FadeIn")]
public class FadeInStep : ScriptPartSO
{
    public float duration = 1f;

    public override float GetWaitTime()
    {
        return duration + extraDelay;
    }
}