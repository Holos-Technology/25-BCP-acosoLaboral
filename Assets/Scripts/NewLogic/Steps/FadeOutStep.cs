using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeOutStep", menuName = "SceneSteps/FadeOut")]
public class FadeOutStep : ScriptPartSO
{
    public float duration = 1f;

    public override IEnumerator WaitForCompletion()
    {
        yield break;
    }

}