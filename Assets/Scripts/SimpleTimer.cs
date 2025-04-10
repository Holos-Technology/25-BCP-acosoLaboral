using UnityEngine;
using UnityEngine.Events;

public class SimpleTimer : MonoBehaviour
{    
    [SerializeField] private float timerDuration = 3f; // Espera 3 segundos después del envío
    private float currentTime;
    private bool isRunning;

    public UnityEvent onTimerComplete; // Evento que se ejecuta después del temporizador

    void Start()
    {
        FormularioManager.Instance.SendFormulario();
        // 🔹 Escuchar el evento cuando el formulario se haya enviado
        FormularioManager.Instance.onFormularioEnviado.AddListener(StartTimer);
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                isRunning = false;
                onTimerComplete?.Invoke(); // 🔹 Se ejecuta cuando el temporizador termina
            }
        }
    }

    public void StartTimer()
    {
        Debug.Log("⏳ Formulario enviado. Iniciando temporizador de 3 segundos...");
        currentTime = timerDuration;
        isRunning = true;
    }
}
