using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> stepObjects = new List<MonoBehaviour>(); // Lista de Steps en el Inspector
    private Queue<IStep> stepQueue = new Queue<IStep>();
    private bool isRunning = false;

    private void Awake()
    {
        // Convertimos la lista en una cola y validamos los Steps
        LoadSteps();
    }

    private void LoadSteps()
    {
        stepQueue.Clear();
        foreach (MonoBehaviour obj in stepObjects)
        {
            if (obj is IStep step)
            {
                stepQueue.Enqueue(step);
            }
            else
            {
                Debug.LogWarning($"El objeto {obj.name} no implementa IStep y será ignorado.");
            }
        }
        
        StartSequence();
    }

    public void StartSequence()
    {
        if (!isRunning && stepQueue.Count > 0)
        {
            isRunning = true;
            StartCoroutine(RunSequence());
        }
    }

    private IEnumerator RunSequence()
    {
        while (stepQueue.Count > 0)
        {
            IStep currentStep = stepQueue.Dequeue();
            yield return StartCoroutine(currentStep.Execute()); // Ejecutar el Step como corrutina
        }
        isRunning = false;
    }
    
    public void ContinueSequence()
    {
        if (stepQueue.Count > 0)
        {
            StartCoroutine(RunSequence());
        }
        else
        {
            isRunning = false;
        }
    }
}
