using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "FadeInStep", menuName = "SceneSteps/FadeIn")]
public class FadeInStep : ScriptPartSO
{
    public float duration = 1f;
    public override IEnumerator WaitForCompletion()
    {
        bool finished = false;

        SceneGameManager.Instance.PlayerFadeTo(0f, duration, () => finished = true);

        yield return new WaitUntil(() => finished);

        if (extraDelay > 0)
            yield return new WaitForSeconds(extraDelay);
    }
}