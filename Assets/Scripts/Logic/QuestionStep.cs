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
    [SerializeField] private AudioClip initialAudioEnglishClip;
    [Header("Answer Options")]
    [SerializeField] private List<AnswerOption> options = new List<AnswerOption>();

    [Header("Settings")]
    [SerializeField] private float maxTime = 30f;
    [SerializeField] private bool requireAggressor = false;

    [SerializeField] private GameObject agressor;
    private List<int> selectedAnswerIndices = new(); // reemplaza selectedAnswerIndex si múltiple

    private bool answerConfirmed = false;
    private string selectedCountry;
    [SerializeField]  string questionString = "Selecciona una opción:"; // ✅ Pregunta por defecto
    [SerializeField] private string englishQuestionString = "Select an option:";

    [Header("Events")]
    public UnityEvent onQuestionAnswered;

    public UnityEvent onStartStep;
    
        
    private Color defaultButtonColor = new Color(0.498f, 0.498f, 0.498f, 1f);
    private Color selectedButtonColor = Color.green;
    
    float timeRemaining;
    private bool timerExpired = false;

    [SerializeField] private string questionJsonData = "";
    [SerializeField] private string questionSceneJsonData = "";
    
    private TMP_Text confirmButtonText;
    [SerializeField] private TMP_Text timeleftText;
    [SerializeField] private bool allowMultipleSelection = false;


       public void InitializeQuestion()
    {
        if (FormularioManager.Instance == null)
        {
            Debug.LogWarning("⚠️ No hay instancia de FormularioManager.");
            return;
        }

        RegisterEmptyQuestion();
    }

    private void RegisterEmptyQuestion()
    {
        string selectedScenario = PlayerPrefs.GetString("SelectedScenario", "Faena");
        string currentGender = isMaleQuestion ? "Masculino" : "Femenino";
        string jugadorKey = $"Jugador {currentGender} {selectedScenario}";

        string escenaKey = !string.IsNullOrEmpty(questionSceneJsonData) ? questionSceneJsonData : "Escena Desconocida";
        string questionKey = !string.IsNullOrEmpty(questionJsonData) ? questionJsonData : "Pregunta Desconocida";

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

        if (confirmButtonText == null)
            confirmButtonText = confirmButton.GetComponentInChildren<TMP_Text>();

        if (confirmButtonText != null)
            confirmButtonText.text = selectedCountry == "Australia" ? "Confirm" : "Confirmar";
        if(timeleftText.text != null)
            timeleftText.text = selectedCountry == "Australia" ? "Time left" : "Tiempo restante";

        timeRemaining = maxTime;
        answerConfirmed = false;
        selectedAnswerIndices.Clear();

        questionText.text = selectedCountry == "Australia" ? englishQuestionString : questionString;

        AudioClip questioClip = selectedCountry == "Australia" ? initialAudioEnglishClip : initialAudioClip;
        if (questioClip != null)
        {
            audioSource.clip = questioClip;
            audioSource.Play();
        }
        
        confirmButton.interactable = false;
        multipleChoicePanel.SetActive(true);
        multipleChoicePanelChild.SetActive(true);
        StartCoroutine(StartTimer());

        for (int i = 0; i < multipleChoiceButtons.Length; i++)
        {
            if (i < options.Count)
            {
                multipleChoiceButtons[i].gameObject.SetActive(true);
                buttonTexts[i].text = selectedCountry == "Australia" 
                    ? options[i].englishAnswerText 
                    : options[i].answerText;

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

                Debug.Log("⏰ Tiempo agotado. Registrada respuesta automática.");
            }

            multipleChoicePanelChild.SetActive(false);
        }

        if (answerConfirmed && selectedAnswerIndices.Count > 0 && !allowMultipleSelection)
        {
            AudioClip selectedClip = options[selectedAnswerIndices[0]].GetAudioByCountry(selectedCountry);

            if (requireAggressor)
                agressor.SetActive(true);

            if (selectedClip != null)
            {
                audioSource.clip = selectedClip;
                audioSource.Play();
                yield return new WaitWhile(() => audioSource.isPlaying);
            }
        }

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
        if (allowMultipleSelection)
        {
            if (selectedAnswerIndices.Contains(index))
            {
                selectedAnswerIndices.Remove(index);
                multipleChoiceButtons[index].GetComponent<Image>().color = defaultButtonColor;
            }
            else
            {
                selectedAnswerIndices.Add(index);
                multipleChoiceButtons[index].GetComponent<Image>().color = selectedButtonColor;
            }

            confirmButton.interactable = selectedAnswerIndices.Count > 0;
        }
        else
        {
            selectedAnswerIndices.Clear();
            selectedAnswerIndices.Add(index);
            confirmButton.interactable = true;

            for (int i = 0; i < multipleChoiceButtons.Length; i++)
            {
                multipleChoiceButtons[i].GetComponent<Image>().color =
                    i == index ? selectedButtonColor : defaultButtonColor;
            }
        }
    }

    private IEnumerator ConfirmAnswer()
    {
        answerConfirmed = true;

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

            if (allowMultipleSelection)
            {
                List<string> selectedAnswers = new();
                foreach (int idx in selectedAnswerIndices)
                {
                    selectedAnswers.Add(selectedCountry == "Australia"
                        ? options[idx].englishAnswerText
                        : options[idx].answerText);
                }

                escenaDict[questionKey] = string.Join(" / ", selectedAnswers);
            }
            else
            {
                escenaDict[questionKey] = selectedCountry == "Australia"
                    ? options[selectedAnswerIndices[0]].englishAnswerText
                    : options[selectedAnswerIndices[0]].answerText;
            }

            jugadorDict[escenaKey] = escenaDict;
            FormularioManager.Instance.UpdateAnswer(jugadorKey, jugadorDict);
            FormularioManager.Instance.SaveFormulario();
        }

        multipleChoicePanelChild.SetActive(false);
        yield return null;
    }

}

[System.Serializable]
public class AnswerOption
{
    public string answerText;
    public string englishAnswerText;
    public AudioClip peruvianAudio;
    public AudioClip chileanAudio;
    public AudioClip colombianAudio;
    public AudioClip argentinianAudio;
    public AudioClip australianAudio;

    public AudioClip GetAudioByCountry(string country)
    {
        return country switch
        {
            "Peru" or "Perú" => peruvianAudio,
            "Colombia" => colombianAudio,
            "Argentina" => argentinianAudio,
            "Australia" => australianAudio,
            _ => chileanAudio
        };
    }
}