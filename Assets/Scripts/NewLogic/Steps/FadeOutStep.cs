using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FadeOutStep", menuName = "SceneSteps/FadeOut")]
public class FadeOutStep : ScriptPartSO
{
    public float duration = 1f;
    
    public override IEnumerator WaitForCompletion()
    {
        bool finished = false;

        SceneGameManager.Instance.PlayerFadeTo(1f, duration, () => finished = true);

        yield return new WaitUntil(() => finished);

        if (extraDelay > 0)
            yield return new WaitForSeconds(extraDelay);
    }

}