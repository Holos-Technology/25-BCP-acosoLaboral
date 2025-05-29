using UnityEngine;

[CreateAssetMenu(fileName = "TeleportStep", menuName = "SceneSteps/Teleport")]
public class TeleportStep : ScriptPartSO
{
    public Transform targetTransform;
    
    
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(name) || name.StartsWith("New ")) 
        {
            stepType = StepType.Teleport;
        }
    }
}