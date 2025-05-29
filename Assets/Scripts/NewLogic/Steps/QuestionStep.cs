using UnityEngine;

[CreateAssetMenu(fileName = "QuestionStep", menuName = "SceneSteps/Question")]
public class QuestionStep : ScriptPartSO
{
    [TextArea]
    public string question;
    public string[] options;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(name) || name.StartsWith("New ")) 
        {
            stepType = StepType.Question;
        }
    }
}