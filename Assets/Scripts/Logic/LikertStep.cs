using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LikertStep : MonoBehaviour,IStep
{    
    [SerializeField] private bool isMaleQuestion;

    [Header("UI Elements")]
    [SerializeField] private GameObject likertPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] emotionButtons;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text timeLeftText;

    [Header("Settings")]
    [SerializeField] private float maxTime = 60f;
    [SerializeField] private string questionString = "Seleccione la emoción que sintió:";
    [SerializeField] private string questionTextEnglish = "Select the emotion you felt:";

    [SerializeField] private string questionJsonData = ""; // ✅ Clave de la pregunta
    [SerializeField] private string questionSceneJsonData = ""; // ✅ Clave de la escena

    [Header("Aggressor Settings")]
    [SerializeField] private GameObject aggressorGameObject; // 🔹 Referencia al agresor
    [SerializeField] private bool reactivateAggressor = true; // 🔹 Si debe volver a aparecer después del Likert

    [Header("Events")]
    public UnityEvent onQuestionAnswered;

    private HashSet<int> selectedEmotions = new HashSet<int>(); 
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    private bool answerConfirmed = false;
    [SerializeField] private AudioClip initialAudioClip; // 🎵 Nuevo AudioClip inicial
    [SerializeField] private AudioClip audioEnglish;
    
    public UnityEvent onStartStep;
    private float currentTimeRemaining;
    public void InitializeQuestion()
    { 
        if (FormularioManager.Instance == null)
        {
            Debug.LogWarning("⚠️ No hay instancia de FormularioManager.");
            return;
        }
        RegisterEmptyQuestion();
    }

    /// <summary>
    /// Registra todas las preguntas con "-" en la categoría correspondiente (`isMaleQuestion`).
    /// </summary>
    private void RegisterEmptyQuestion()
    {   
        if (FormularioManager.Instance == null)
        {
            Debug.LogWarning("⚠️ No se puede registrar pregunta vacía. FormularioManager.Instance es null.");
            return;
        }

        string selectedScenario = PlayerPrefs.GetString("SelectedScenario", "Faena");
        string currentGender = isMaleQuestion ? "Masculino" : "Femenino";
        string jugadorKey = $"Jugador {currentGender} {selectedScenario}";

        string escenaKey = !string.IsNullOrEmpty(questionSceneJsonData) ? questionSceneJsonData : "Escena Desconocida";
        string questionKey = !string.IsNullOrEmpty(questionJsonData) ? questionJsonData : "Pregunta Desconocida";

        // ✅ Solo registrar en el género correspondiente
        EnsureFieldExists(jugadorKey, escenaKey, questionKey);
    }
    private void EnsureFieldExists(string jugadorKey, string escenaKey, string questionKey)
    {
        if (FormularioManager.Instance == null)
        {
            Debug.LogWarning("⚠️ No se puede asegurar estructura de formulario. FormularioManager.Instance es null.");
            return;
        }

        if (FormularioManager.Instance.formulario.fields == null)
            FormularioManager.Instance.formulario.fields = new Dictionary<string, object>();

        if (!FormularioManager.Instance.formulario.fields.ContainsKey(jugadorKey))
            FormularioManager.Instance.UpdateAnswer(jugadorKey, new Dictionary<string, object>());

        var jugadorDict = (Dictionary<string, object>)FormularioManager.Instance.formulario.fields[jugadorKey];

        if (!jugadorDict.ContainsKey(escenaKey))
            jugadorDict[escenaKey] = new Dictionary<string, object>();

        var escenaDict = (Dictionary<string, object>)jugadorDict[escenaKey];

        if (!escenaDict.ContainsKey(questionKey))
            escenaDict[questionKey] = "-";

        jugadorDict[escenaKey] = escenaDict;
        FormularioManager.Instance.UpdateAnswer(jugadorKey, jugadorDict);
        FormularioManager.Instance.SaveFormulario();
    }

   public IEnumerator Execute()
    {
        onStartStep?.Invoke();
        if (initialAudioClip != null)
        {
            audioSource.clip = initialAudioClip;
            audioSource.Play();
        }
        aggressorGameObject.SetActive(false);

        // Configurar la pregunta en la UI
        string selectedLanguage = PlayerPrefs.GetString("language", "es");
        bool isEnglish = selectedLanguage == "en";

        // Asignar texto en base al idioma
        questionText.text = isEnglish ? questionTextEnglish : questionString;
        timeLeftText.text = isEnglish ? "Time left:" : "Tiempo restante:";

        // Reproducir audio si hay clip asignado
        audioSource.clip = isEnglish ? audioEnglish : initialAudioClip;
        if (audioSource.clip != null)
            audioSource.Play();

        // Inicializar pantalla
        likertPanel.SetActive(true);
        confirmButton.interactable = false;
        selectedEmotions.Clear();
        answerConfirmed = false;

        currentTimeRemaining = maxTime;
        StartCoroutine(StartTimer());

        // Configurar botones
        for (int i = 0; i < emotionButtons.Length; i++)
        {
            int index = i;
            emotionButtons[i].onClick.RemoveAllListeners();
            emotionButtons[i].onClick.AddListener(() => ToggleEmotion(index));
            emotionButtons[i].interactable = true;
            emotionButtons[i].image.color = Color.white;
        }

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => StartCoroutine(ConfirmAnswer()));
        yield return new WaitUntil(() => answerConfirmed || currentTimeRemaining <= 0);
        if (!answerConfirmed)
        {
            answerConfirmed = true;

            string selectedScenario = PlayerPrefs.GetString("SelectedScenario", "Faena");
            string genderKey = isMaleQuestion ? "Masculino" : "Femenino";
            string jugadorKey = $"Jugador {genderKey} {selectedScenario}";

            string escenaKey = !string.IsNullOrEmpty(questionSceneJsonData) ? questionSceneJsonData : "Escena Desconocida";
            string questionKey = !string.IsNullOrEmpty(questionJsonData) ? questionJsonData : "Pregunta Desconocida";

            if (FormularioManager.Instance != null)
            {
                if (FormularioManager.Instance.formulario.fields == null)
                    FormularioManager.Instance.formulario.fields = new Dictionary<string, object>();

                if (!FormularioManager.Instance.formulario.fields.ContainsKey(jugadorKey))
                    FormularioManager.Instance.UpdateAnswer(jugadorKey, new Dictionary<string, object>());

                var jugadorDict = (Dictionary<string, object>)FormularioManager.Instance.formulario.fields[jugadorKey];

                if (!jugadorDict.ContainsKey(escenaKey))
                    jugadorDict[escenaKey] = new Dictionary<string, object>();

                var escenaDict = (Dictionary<string, object>)jugadorDict[escenaKey];
                escenaDict[questionKey] = "Fuera de tiempo";

                jugadorDict[escenaKey] = escenaDict;
                FormularioManager.Instance.UpdateAnswer(jugadorKey, jugadorDict);
                FormularioManager.Instance.SaveFormulario();

                Debug.Log($"⏰ Tiempo agotado. Registrada respuesta automática: {jugadorKey} -> {escenaKey} -> {questionKey}: Fuera de tiempo");
            }
            else
            {
                Debug.LogWarning("⚠️ FormularioManager.Instance es null. No se pudo guardar la respuesta por timeout.");
            }
        }

        if (aggressorGameObject != null && reactivateAggressor)
        {
            aggressorGameObject.SetActive(true);
        }

        likertPanel.SetActive(false);
        onQuestionAnswered?.Invoke();
    }
    private IEnumerator StartTimer()
    {
        while (currentTimeRemaining > 0 && !answerConfirmed)
        {
            currentTimeRemaining -= Time.deltaTime;
            timerText.text = Mathf.Ceil(currentTimeRemaining).ToString();
            yield return null;
        }

        if (currentTimeRemaining <= 0)
        {
            confirmButton.interactable = true;
        }
    }
    private void ToggleEmotion(int index)
    {
        if (selectedEmotions.Contains(index))
        {
            selectedEmotions.Remove(index);
            emotionButtons[index].image.color = Color.white;
        }
        else
        {
            selectedEmotions.Add(index);
            emotionButtons[index].image.color = Color.green;
        }

        confirmButton.interactable = selectedEmotions.Count > 0;
    }

    private IEnumerator ConfirmAnswer()
    {
        answerConfirmed = true;

        List<string> selectedAnswers = new List<string>();
        foreach (int index in selectedEmotions)
        {
            selectedAnswers.Add(emotionButtons[index].gameObject.name);
        }
        string answersString = selectedAnswers.Count > 0 ? string.Join(", ", selectedAnswers) : "-";

        string selectedScenario = PlayerPrefs.GetString("SelectedScenario", "Faena");
        string genderKey = isMaleQuestion ? "Masculino" : "Femenino";
        string jugadorKey = $"Jugador {genderKey} {selectedScenario}";

        string escenaKey = !string.IsNullOrEmpty(questionSceneJsonData) ? questionSceneJsonData : "Escena Desconocida";
        string questionKey = !string.IsNullOrEmpty(questionJsonData) ? questionJsonData : "Pregunta Desconocida";

        if (FormularioManager.Instance != null)
        {
            if (FormularioManager.Instance.formulario.fields == null)
                FormularioManager.Instance.formulario.fields = new Dictionary<string, object>();

            if (!FormularioManager.Instance.formulario.fields.ContainsKey(jugadorKey))
                FormularioManager.Instance.UpdateAnswer(jugadorKey, new Dictionary<string, object>());

            var jugadorDict = (Dictionary<string, object>)FormularioManager.Instance.formulario.fields[jugadorKey];

            if (!jugadorDict.ContainsKey(escenaKey))
                jugadorDict[escenaKey] = new Dictionary<string, object>();

            var escenaDict = (Dictionary<string, object>)jugadorDict[escenaKey];
            escenaDict[questionKey] = answersString;

            jugadorDict[escenaKey] = escenaDict;
            FormularioManager.Instance.UpdateAnswer(jugadorKey, jugadorDict);
            FormularioManager.Instance.SaveFormulario();

            Debug.Log($"✅ Respuesta Likert guardada: {jugadorKey} -> {escenaKey} -> {questionKey}: \"{answersString}\"");
        }
        else
        {
            Debug.LogWarning("⚠️ FormularioManager.Instance es null. No se pudo guardar la respuesta.");
        }

        yield return null;
    }

    public void SetQuestion(string newQuestionSpanish, string newQuestionEnglish)
    {
        questionString = newQuestionSpanish;
        questionTextEnglish = newQuestionEnglish;
    }
}
