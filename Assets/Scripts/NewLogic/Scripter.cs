using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Scripter : MonoBehaviour
{
    
    [SerializeField]List<ScriptPartSO> scriptParts;
    private Coroutine runningCoroutine;
    public UnityEvent OnStart;
    public UnityEvent OnEnd;
    public void Execute(SceneScriptable script)
    {
        scriptParts = script.steps;

        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);

        runningCoroutine = StartCoroutine(ExecuteSimulation());
    }

  
    private IEnumerator ExecuteSimulation()
    {
        OnStart?.Invoke();
        yield return new WaitForSeconds(1.0f);
        foreach (var step in scriptParts)
        {
            if (step == null) continue;

            step.OnStart?.Invoke();

            // Esperar a que el paso se complete (audio, UI, etc.)
            yield return StartCoroutine(step.WaitForCompletion());
            
            Debug.Log(step.ToString());
            step.OnEnd?.Invoke();
        }
        OnEnd?.Invoke();
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
    Question,
    Recon
}