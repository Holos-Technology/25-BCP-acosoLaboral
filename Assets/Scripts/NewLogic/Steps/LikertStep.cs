using UnityEngine;

[CreateAssetMenu(fileName = "LikertStep", menuName = "SceneSteps/Likert")]
public class LikertStep : ScriptPartSO
{
    [TextArea]
    public string statement;
    public int scaleFrom = 1;
    public int scaleTo = 5;

    
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(name) || name.StartsWith("New ")) 
        {
            stepType = StepType.Likert;
        }
    }
}