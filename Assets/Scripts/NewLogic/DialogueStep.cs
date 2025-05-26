using UnityEngine;

[CreateAssetMenu(fileName = "DialogueStep", menuName = "SceneSteps/Dialogue")]
public class DialogueStep : ScriptPartSO
{
    [TextArea]
    public string dialogue;
    public string characterName;

    public override float GetWaitTime()
    {
        return extraDelay;
    }
}