using UnityEngine;

[CreateAssetMenu(fileName = "NarrationStep", menuName = "SceneSteps/Narration")]
public class NarrationStep : ScriptPartSO
{
    [TextArea]
    public string text;
    public string speaker;

    
    
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(name) || name.StartsWith("New ")) 
        {
            stepType = StepType.Narration;
        }
    }
}