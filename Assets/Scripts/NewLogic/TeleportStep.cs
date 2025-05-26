using UnityEngine;

[CreateAssetMenu(fileName = "TeleportStep", menuName = "SceneSteps/Teleport")]
public class TeleportStep : ScriptPartSO
{
    public Transform targetTransform;

    public override float GetWaitTime()
    {
        return extraDelay;
    }
}