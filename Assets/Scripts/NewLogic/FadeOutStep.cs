using UnityEngine;

[CreateAssetMenu(fileName = "FadeOutStep", menuName = "SceneSteps/FadeOut")]
public class FadeOutStep : ScriptPartSO
{
    public float duration = 1f;

    public override float GetWaitTime()
    {
        return duration + extraDelay;
    }
}