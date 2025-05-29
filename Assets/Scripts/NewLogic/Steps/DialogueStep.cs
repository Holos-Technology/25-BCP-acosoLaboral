using UnityEngine;

[CreateAssetMenu(fileName = "DialogueStep", menuName = "SceneSteps/Dialogue")]
public class DialogueStep : ScriptPartSO
{
    [TextArea]
    public string dialogue;
    public string characterName;

    
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(name) || name.StartsWith("New ")) 
        {
            stepType = StepType.Dialogue;
        }
    }
}