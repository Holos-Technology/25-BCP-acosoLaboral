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
    /// Devuelve el tiempo total que se debe esperar después de este paso.
    /// </summary>
    public abstract float GetWaitTime();
}

