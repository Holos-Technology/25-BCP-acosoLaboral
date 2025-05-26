using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scripter : MonoBehaviour
{
    
    [SerializeField]List<ScriptPartSO> scriptParts;
    private Coroutine runningCoroutine;

    public void Execute(SceneScriptable script)
    {
        scriptParts = script.steps;

        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);

        runningCoroutine = StartCoroutine(ExecuteSimulation());
    }

    private IEnumerator ExecuteSimulation()
    {
        foreach (var step in scriptParts)
        {
            if (step == null) continue;

            step.OnStart?.Invoke();

            float waitTime = step.GetWaitTime();

            switch (step.stepType)
            {
                case StepType.Audio:
                    var audioStep = step as AudioStep;
                    if (audioStep != null && audioStep.audioClip != null)
                    {
                        SceneGameManager.Instance.narrationAudioSource.clip = audioStep.audioClip;
                        SceneGameManager.Instance.narrationAudioSource.Play();
                    }
                    break;

                case StepType.Question:
                    var questionStep = step as QuestionStep;
                    if (questionStep != null)
                    {
                        GameObject questionGO = Instantiate(
                            SceneGameManager.Instance.questionPrefab,
                            SceneGameManager.Instance.questionPosition.position,
                            Quaternion.identity
                        );
                        // Aquí podrías inicializar el contenido del prefab si tiene un script tipo QuestionUI
                    }
                    break;

                case StepType.Likert:
                    var likertStep = step as LikertStep;
                    if (likertStep != null)
                    {
                        GameObject likertGO = Instantiate(
                            SceneGameManager.Instance.likertPrefab,
                            SceneGameManager.Instance.questionPosition.position,
                            Quaternion.identity
                        );
                        // Igual: inicializar contenido si el prefab tiene lógica propia
                    }
                    break;

                case StepType.Teleport:
                    var teleportStep = step as TeleportStep;
                    if (teleportStep != null && teleportStep.targetTransform != null)
                    {
                        SceneGameManager.Instance.defaultPlayerPosition.position = teleportStep.targetTransform.position;
                    }
                    break;

                // Puedes extender más tipos aquí...
            }

            yield return new WaitForSeconds(waitTime);

            step.OnEnd?.Invoke();
        }

        Debug.Log("[SCRIPT] Simulación completada");
    }

}
public enum StepType
{
    Narration,
    Audio,
    FadeIn,
    FadeOut,
    Teleport,
    Dialogue,
    Likert,
    Question
}