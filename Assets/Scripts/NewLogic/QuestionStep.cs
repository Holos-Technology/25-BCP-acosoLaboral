using UnityEngine;

[CreateAssetMenu(fileName = "QuestionStep", menuName = "SceneSteps/Question")]
public class QuestionStep : ScriptPartSO
{
    [TextArea]
    public string question;
    public string[] options;

    public override float GetWaitTime()
    {
        return extraDelay;
    }
}