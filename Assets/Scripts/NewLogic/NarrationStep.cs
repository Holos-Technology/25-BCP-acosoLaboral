using UnityEngine;

[CreateAssetMenu(fileName = "NarrationStep", menuName = "SceneSteps/Narration")]
public class NarrationStep : ScriptPartSO
{
    [TextArea]
    public string text;
    public string speaker;

    public override float GetWaitTime()
    {
        return extraDelay;
    }
}