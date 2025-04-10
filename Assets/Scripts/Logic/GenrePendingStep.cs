using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GenrePendingStep :MonoBehaviour, IStep
{
    [SerializeField] private GameObject maleStepObject;  // Prefab o GameObject con el Step masculino
    [SerializeField] private GameObject femaleStepObject; // Prefab o GameObject con el Step femenino

    [SerializeField] private bool testMode = false; // Activar pruebas desde el Editor
    [SerializeField] private string testGender = "Masculino"; // Género manual para pruebas

   
    public UnityEvent OnStartStep; // Implementación de la interfaz
    public IEnumerator Execute()
    {
        // Si testMode está activado, usar testGender, si no, usar PlayerPrefs
        string selectedGender = testMode ? testGender : PlayerPrefs.GetString("SelectedGender", "Masculino");
        OnStartStep.Invoke();
        // Determinar qué Step ejecutar
        IStep stepToExecute = null;

        if (selectedGender == "Masculino" && maleStepObject != null)
        {
            stepToExecute = maleStepObject.GetComponent<IStep>();
        }
        else if (selectedGender == "Femenino" && femaleStepObject != null)
        {
            stepToExecute = femaleStepObject.GetComponent<IStep>();
        }

        if (stepToExecute != null)
        {
            yield return StartCoroutine(stepToExecute.Execute()); // Ejecutar Step correcto
        }
        else
        {
            Debug.LogWarning("No se encontró un Step válido para el género seleccionado.");
        }
    }

    public void SpecialNarration()
    {           
        string selectedGender = testMode ? testGender : PlayerPrefs.GetString("SelectedGender", "Masculino");

        if (selectedGender == "Masculino" && maleStepObject != null)
        {
            maleStepObject.GetComponent<NarrationStep>().LoadText();
        }
        else if (selectedGender == "Femenino" && femaleStepObject != null)
        {
            femaleStepObject.GetComponent<NarrationStep>().LoadText();        
        }
   
    }
   
}
