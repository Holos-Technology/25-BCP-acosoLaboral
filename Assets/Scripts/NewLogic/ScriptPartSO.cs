using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ScriptPartSO : ScriptableObject
{
    
    public StepType stepType;
    
    [Header("Eventos")]
    public UnityEvent OnStart;
    public UnityEvent OnEnd;
    [Tooltip("Tiempo adicional a esperar luego de este paso")]
    public float extraDelay = 0f;
    /// <summary>
    /// Si el paso requiere espera activa (como un audio), implementa esto.
    /// </summary>
    public virtual IEnumerator WaitForCompletion()
    {
        yield return new WaitForSeconds(extraDelay);
    }
}

