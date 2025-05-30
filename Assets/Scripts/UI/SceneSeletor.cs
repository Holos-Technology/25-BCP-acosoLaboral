using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSeletor : MonoBehaviour
{
    [SerializeField] private StartingCanvasManager StartingCanvasManager;
    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonConfirm;

    [Header("Scenery")]
    [SerializeField] private TMP_Dropdown sceneDropdawn;
    [SerializeField] private Dictionary<string,int> scenes = new Dictionary<string, int>();
    string lenguage;
    int intScene;

    [Header("ID")]
    [SerializeField] private TMP_Dropdown idDropdawn;
    [SerializeField] private TMP_InputField searchInput;
    private List<string> allIdentifiers = new List<string>();

    [SerializeField] private int lobbyIndex;
    [SerializeField] private int corporativeIndex;
    [SerializeField] private int casinoIndex;
    [SerializeField] private bool debugMode = true;


    [Header("Gender")]
    [SerializeField] private Button[] genderButtons;
    private string selectedGender = "";

    private void Awake()
    {
        scenes.Add("Corporativo", 6);
        scenes.Add("Faena", 5);

        sceneDropdawn.onValueChanged.AddListener(delegate { OnDropdownSceneChanged(sceneDropdawn.value); });
        
        searchInput.onValueChanged.AddListener(FilterIdentifiers);

        buttonConfirm.onClick.AddListener(LoadSelectedScene);
        buttonBack.onClick.AddListener(StartingCanvasManager.PreviousPanel);

        sceneDropdawn.onValueChanged.AddListener(_ => ValidateConfirmButton());
        idDropdawn.onValueChanged.AddListener(_ => ValidateConfirmButton());

        foreach (Button btn in genderButtons)
        {
            btn.onClick.AddListener(() => ClickedGender(btn));
        }
    }

    private void OnEnable()
    {
        lenguage = PlayerPrefs.GetString("SelectedLenguage");

        buttonConfirm.interactable = false;

        DropdawnSceneInit();

        PopulateIdentifierDropdown();
    }

    private void DropdawnSceneInit()
    {
        sceneDropdawn.ClearOptions();

        List<string> options = new List<string>();

        if (lenguage == "Spanish")
        {
            if (scenes.ContainsKey("Corporativo")) options.Add("Corporativo");
            if (scenes.ContainsKey("Faena")) options.Add("Faena");
        }
        else if (lenguage == "English")
        {
            if (scenes.ContainsKey("Faena")) options.Add("Faena");
        }

        sceneDropdawn.AddOptions(options);

        OnDropdownSceneChanged(sceneDropdawn.value);
    }

    private void OnDropdownSceneChanged(int index)
    {
        string key = sceneDropdawn.options[index].text;

        if (scenes.TryGetValue(key, out int value))
        {
            intScene = value;
        }
    }

    private void PopulateIdentifierDropdown()
    {
        idDropdawn.ClearOptions();
        allIdentifiers.Clear();

        allIdentifiers.Add("");

        if (PlayerManager.Instance != null)
        {
            List<string> uniqueIds = PlayerManager.Instance.GetUniqueIdentifiersAndSurveyed();
            if (uniqueIds.Count == 0)
                allIdentifiers.Add("BCH Program");
            else
                allIdentifiers.AddRange(uniqueIds);
        }
        else
        {
            allIdentifiers.Add("BCH Program");
        }

        idDropdawn.AddOptions(allIdentifiers);
        idDropdawn.value = 0;
    }

    private void FilterIdentifiers(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            idDropdawn.ClearOptions();
            idDropdawn.AddOptions(allIdentifiers);
            idDropdawn.value = 0;
            return;
        }

        List<string> filtered = allIdentifiers
                                .Where(id => id.Contains(text, System.StringComparison.OrdinalIgnoreCase))
                                .ToList();

        if (filtered.Count == 0 || filtered[0] != "")
            filtered.Insert(0, "");

        idDropdawn.ClearOptions();
        idDropdawn.AddOptions(filtered);
        idDropdawn.value = 0;
    }
    private void ValidateConfirmButton()
    {
        string selectedScene = sceneDropdawn.options[sceneDropdawn.value].text;
        string selectedId = idDropdawn.options[idDropdawn.value].text;

        bool isValid = 
            !string.IsNullOrEmpty(selectedScene) &&
            !string.IsNullOrEmpty(selectedId) &&
            !string.IsNullOrEmpty(selectedGender); ;

        buttonConfirm.interactable = isValid;
    }

    private void ButtonConfirm()
    {
        SceneManager.LoadScene(intScene);
    }

    private void FillFormularioData(SurveyData surveyData)
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry");

        void SetFieldValue(string key, object value)
        {
            var fields = FormularioManager.Instance.formulario.fields;
            if (fields.ContainsKey(key)) fields[key] = value;
            else fields.Add(key, value);
        }

        if (string.IsNullOrEmpty(surveyData.id))
        {
            FormularioManager.Instance.formulario.studentId = "";
            FormularioManager.Instance.formulario.studentName = "Desconocido";
            FormularioManager.Instance.formulario.instructorName = "Pia Lineros";
            FormularioManager.Instance.formulario.corporationName = "";

            SetFieldValue("Edad", "");
            SetFieldValue("Faena", "");
            SetFieldValue("Cargo", "");
            SetFieldValue("Antiguedad en el Cargo (meses)", "");
            SetFieldValue("Antiguedad en la Empresa (meses)", "");
            SetFieldValue("Empresa Subcontratista/Proveedor", "");
            SetFieldValue("Region", "");
            SetFieldValue("Pais", selectedCountry);
            SetFieldValue("Sexo", selectedGender);
            SetFieldValue("id_formulario", "");
            FormularioManager.Instance.SaveFormulario();
            return;
        }

        FormularioManager.Instance.formulario.studentId = surveyData.rut;
        FormularioManager.Instance.formulario.instructorName = "Pia Lineros";
        FormularioManager.Instance.formulario.corporationName = surveyData.empresaMandante;

        string extractedName = surveyData.id.Split(':').FirstOrDefault()?.Trim() ?? "Desconocido";
        FormularioManager.Instance.formulario.studentName = extractedName;

        SetFieldValue("Edad", surveyData.edad);
        SetFieldValue("Faena", surveyData.faena);
        SetFieldValue("Cargo", surveyData.cargo);
        SetFieldValue("Antiguedad en el Cargo (meses)", surveyData.antiguedadCargo);
        SetFieldValue("Antiguedad en la Empresa (meses)", surveyData.antiguedadEmpresa);
        SetFieldValue("Empresa Subcontratista/Proveedor", surveyData.empresaSubcontratista);
        SetFieldValue("Region", surveyData.region);
        SetFieldValue("Pais", selectedCountry);
        SetFieldValue("Sexo", selectedGender);
        SetFieldValue("id_formulario", surveyData.id_formulario);

        FormularioManager.Instance.SaveFormulario();
    }

    void ClickedGender(Button btn)
    {
        selectedGender = GetGenderName(btn.name);
        ValidateConfirmButton();
    }

    string GetGenderName(string buttonName)
    {
        switch (buttonName)
        {
            case "Male": return "Masculino";
            case "Female": return "Femenino";
            default: return "";
        }
    }

    private void LoadSelectedScene()
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry");
        string selectedScenario = sceneDropdawn.options[sceneDropdawn.value].text;
        PlayerPrefs.SetString("SelectedScenario", selectedScenario);
        PlayerPrefs.SetString("SelectedGender", selectedGender); 
        PlayerPrefs.Save();

        FormularioManager.Instance.formulario.fields["Pais"] = selectedCountry;
        FormularioManager.Instance.formulario.fields["Sexo"] = selectedGender;

        string selectedId = idDropdawn.options[idDropdawn.value].text;
        if (PlayerManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(selectedId))
            {
                SurveyData surveyData = PlayerManager.Instance.GetSurveyDataById(selectedId);
                FillFormularioData(surveyData);

                Debug.Log($"🌍 País seleccionado: {selectedCountry}");
                Debug.Log($"🎭 Escenario seleccionado: {selectedScenario}");
                Debug.Log($"🔍 DebugMode: {debugMode}");
                FormularioManager.Instance.SaveFormulario(); // 🔹 Guardar JSON solo al confirmar

                if (debugMode)
                {
                    if (selectedScenario == "Faena")
                    {
                        Debug.Log($"🎰 Cargando escena de Casino (Index: {casinoIndex})");
                        SceneManager.LoadScene(casinoIndex);
                    }
                    else if (selectedScenario == "Corporativo")
                    {
                        Debug.Log($"🏢 Cargando escena Corporativa (Index: {corporativeIndex})");
                        SceneManager.LoadScene(corporativeIndex);
                    }
                    else
                    {
                        Debug.LogError("🚨 Error: Escenario desconocido en modo Debug.");
                    }
                }
                else
                {
                    Debug.Log($"🎮 Cargando escena de Lobby (Index: {lobbyIndex})");
                    SceneManager.LoadScene(intScene);
                }
            }
        }
        else
        {
            if (debugMode)
            {
                if (selectedScenario == "Faena")
                {
                    Debug.Log($"🎰 Cargando escena de Casino (Index: {casinoIndex})");
                    SceneManager.LoadScene(casinoIndex);
                }
                else if (selectedScenario == "Corporativo")
                {
                    Debug.Log($"🏢 Cargando escena Corporativa (Index: {corporativeIndex})");
                    SceneManager.LoadScene(corporativeIndex);
                }
                else
                {
                    Debug.LogError("🚨 Error: Escenario desconocido en modo Debug.");
                }
            }
            else
            {
                Debug.Log($"🎮 Cargando escena de Lobby (Index: {lobbyIndex})");
                SceneManager.LoadScene(intScene);
            }
        }

        void SetFieldValue(string key, object value)
        {
            if (FormularioManager.Instance.formulario.fields.ContainsKey(key))
            {
                FormularioManager.Instance.formulario.fields[key] = value; 
            }
            else
            {
                FormularioManager.Instance.formulario.fields.Add(key, value); 
            }
        }
    }
}
