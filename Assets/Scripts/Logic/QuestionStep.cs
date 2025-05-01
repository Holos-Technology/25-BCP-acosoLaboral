using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestionStep : MonoBehaviour, IStep
{
    [SerializeField] private bool isMaleQuestion;
    
     [Header("UI Elements")]
    [SerializeField] private GameObject multipleChoicePanel;
    [SerializeField] private GameObject multipleChoicePanelChild;
    [SerializeField] private TMP_Text questionText; // ✅ Ahora es dinámico
    [SerializeField] private Button[] multipleChoiceButtons;
    [SerializeField] private TMP_Text[] buttonTexts;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text timerText;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip initialAudioClip;
    [Header("Answer Options")]
    [SerializeField] private List<AnswerOption> options = new List<AnswerOption>();

    [Header("Settings")]
    [SerializeField] private float maxTime = 30f;
    [SerializeField] private bool requireAggressor = false;

    [SerializeField] private GameObject agressor;
    private int selectedAnswerIndex = -1;
    private bool answerConfirmed = false;
    private string selectedCountry;
    [SerializeField]  string questionString = "Selecciona una opción:"; // ✅ Pregunta por defecto
    
    [Header("Events")]
    public UnityEvent onQuestionAnswered;

    public UnityEvent onStartStep;
    
        
    private Color defaultButtonColor = new Color(0.498f, 0.498f, 0.498f, 1f);
    private Color selectedButtonColor = Color.green;
    
    float timeRemaining;
    private bool timerExpired = false;

    [SerializeField] private string questionJsonData = "";
    [SerializeField] private string questionSceneJsonData = "";
    
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
    /// Registra la pregunta con "-" en la categoría correspondiente (`isMaleQuestion`)
    /// </summary>
    private void RegisterEmptyQuestion()
    {
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
            agressor.SetActive(false);
            selectedCountry = PlayerPrefs.GetString("SelectedCountry", "Chile"); 

            timeRemaining = maxTime;

            questionText.text = questionString;
            if (initialAudioClip != null)
            {
                audioSource.clip = initialAudioClip;
                audioSource.Play();
            }

            confirmButton.interactable = false;
            selectedAnswerIndex = -1;
            answerConfirmed = false;

            multipleChoicePanel.SetActive(true);
            multipleChoicePanelChild.SetActive(true);
            StartCoroutine(StartTimer());

            // Configurar botones según las opciones
            for (int i = 0; i < multipleChoiceButtons.Length; i++)
            {
                if (i < options.Count)
                {
                    multipleChoiceButtons[i].gameObject.SetActive(true);
                    buttonTexts[i].text = options[i].answerText;
                    int index = i;
                    multipleChoiceButtons[i].onClick.RemoveAllListeners();
                    multipleChoiceButtons[i].onClick.AddListener(() => SelectAnswer(index));
                    multipleChoiceButtons[i].GetComponent<Image>().color = defaultButtonColor;
                }
                else
                {
                    multipleChoiceButtons[i].gameObject.SetActive(false);
                }
            }

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => StartCoroutine(ConfirmAnswer()));

            yield return new WaitUntil(() => answerConfirmed || timerExpired);
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
                    var jugadorDict = (Dictionary<string, object>)FormularioManager.Instance.formulario.fields[jugadorKey];
                    var escenaDict = (Dictionary<string, object>)jugadorDict[escenaKey];
                    escenaDict[questionKey] = "Fuera de tiempo";
                    jugadorDict[escenaKey] = escenaDict;
                    FormularioManager.Instance.UpdateAnswer(jugadorKey, jugadorDict);
                    FormularioManager.Instance.SaveFormulario();

                    Debug.Log($"⏰ Tiempo agotado. Registrada respuesta automática.");
                }

                multipleChoicePanelChild.SetActive(false);
            }

            // 🔊 Reproducir audio si se confirmó (no si fue por timeout)
            if (answerConfirmed && selectedAnswerIndex >= 0)
            {
                string country = PlayerPrefs.GetString("SelectedCountry", "Chile");
                AudioClip selectedClip = options[selectedAnswerIndex].GetAudioByCountry(country);
                if (requireAggressor)
                {
                    agressor.SetActive(true);
                }
                if (selectedClip != null)
                {
                    audioSource.clip = selectedClip;
                    audioSource.Play();
                    yield return new WaitWhile(() => audioSource.isPlaying);
                }
            }

        // ✅ Ahora sí desactivar el panel completo y continuar
            multipleChoicePanel.SetActive(false);

     

            onQuestionAnswered?.Invoke();
       }

        private IEnumerator StartTimer()
        {
            timerExpired = false;

            while (timeRemaining > 0 && !answerConfirmed)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = Mathf.Ceil(timeRemaining).ToString();
                yield return null;
            }

            if (!answerConfirmed && timeRemaining <= 0)
            {
                timerExpired = true;
                confirmButton.interactable = false;
            }
        }

    private void SelectAnswer(int index)
    {
        selectedAnswerIndex = index;
        confirmButton.interactable = true;
        
        // Restaurar colores
        foreach (Button button in multipleChoiceButtons)
        {
            button.GetComponent<Image>().color = defaultButtonColor;
        }
        
        // Cambiar color del botón seleccionado
        multipleChoiceButtons[selectedAnswerIndex].GetComponent<Image>().color = selectedButtonColor;
    
    }

    private IEnumerator ConfirmAnswer()
    {
        answerConfirmed = true;

        string selectedAnswer = options[selectedAnswerIndex].answerText;
        string selectedScenario = PlayerPrefs.GetString("SelectedScenario", "Faena");
        string country = PlayerPrefs.GetString("SelectedCountry", "Chile");

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
            escenaDict[questionKey] = selectedAnswer;

            jugadorDict[escenaKey] = escenaDict;
            FormularioManager.Instance.UpdateAnswer(jugadorKey, jugadorDict);
            FormularioManager.Instance.SaveFormulario();
        }

        // 🔸 Solo ocultar el hijo aquí
        multipleChoicePanelChild.SetActive(false);

        yield return null;
    }

}

[System.Serializable]
public class AnswerOption
{
    public string answerText;
    public List<AnswerAudioData> countryAudios = new();

    public AudioClip GetAudioByCountry(string country)
    {
        string key = country.ToLower();
        foreach (var entry in countryAudios)
        {
            if (entry.countryKey.ToLower() == key)
                return entry.audioClip;
        }

        return null;
    }
}

[System.Serializable]
public class AnswerAudioData
{
    public string countryKey;
    public AudioClip audioClip;
}