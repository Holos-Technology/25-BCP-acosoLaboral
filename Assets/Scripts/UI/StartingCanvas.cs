using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartingCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown scenarioDropdown;
    [SerializeField] private TMP_Dropdown identifierDropdown;
    [SerializeField] private TMP_Dropdown instructorDropdown;
    [SerializeField] private Button startButton;
    [SerializeField] private Button[] countryButtons;
    [SerializeField] private Button[] genderButtons;
    [SerializeField] private TMP_InputField identifierSearchInput;

    private string selectedCountry = "";
    private string selectedGender = "";
    private string selectedScenario = "";
    private string selectedLanguage = "es";
    private List<string> originalIdentifiers = new List<string>();

    [SerializeField] private int lobbyIndex;
    [SerializeField] private int corporativeIndex;
    [SerializeField] private int casinoIndex;
    [SerializeField] private bool debugMode = true;

    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelLanguage;

    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Button confirmLanguageButton;
    [SerializeField] private Button backLanguageButton;

    void Start()
    {
        panelLanguage.SetActive(true);
        panelOptions.SetActive(false);

        confirmLanguageButton.onClick.AddListener(ConfirmLanguageSelection);
        backLanguageButton.onClick.AddListener(BackToLanguageSelection);

        languageDropdown.options.Clear();
        languageDropdown.AddOptions(new List<string> { "Español", "English" });

        startButton.interactable = false;
        startButton.onClick.AddListener(LoadSelectedScene);

        scenarioDropdown.onValueChanged.AddListener(delegate { OnScenarioChanged(); });
        identifierDropdown.onValueChanged.AddListener(delegate { ValidateFields(); });

        foreach (Button btn in countryButtons)
        {
            btn.onClick.AddListener(() => ClickedCountry(btn));
        }

        foreach (Button btn in genderButtons)
        {
            btn.onClick.AddListener(() => ClickedGender(btn));
        }

        PopulateIdentifierDropdown();

    }

    void ConfirmLanguageSelection()
    {
        selectedLanguage = languageDropdown.options[languageDropdown.value].text == "English" ? "en" : "es";
        PlayerPrefs.SetString("language", selectedLanguage);
        PlayerPrefs.Save();

        ApplyLanguageConfiguration();

        panelLanguage.SetActive(false);
        panelOptions.SetActive(true);
    }

    void BackToLanguageSelection()
    {
        panelOptions.SetActive(false);
        panelLanguage.SetActive(true);
    }

    void ApplyLanguageConfiguration()
    {
        selectedLanguage = PlayerPrefs.GetString("language", "es");

        foreach (Button btn in countryButtons)
        {
            if (btn.name == "Australia")
                btn.gameObject.SetActive(selectedLanguage == "en");
            else
                btn.gameObject.SetActive(selectedLanguage == "es");
        }

        scenarioDropdown.ClearOptions();

        if (selectedLanguage == "en")
        {
            scenarioDropdown.AddOptions(new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData(""),
                new TMP_Dropdown.OptionData("Task")
            });
        }
        else
        {
            scenarioDropdown.AddOptions(GetAllScenarioOptions());
        }

        scenarioDropdown.value = 0;
        selectedScenario = "";
    }


    void PopulateIdentifierDropdown()
    {
        identifierDropdown.ClearOptions();
        originalIdentifiers.Clear();

        List<string> options = new List<string> { "" }; // 🔹 Opción vacía por defecto

        if (debugMode)
        {
            options.Add("34748503"); // 🔹 Solo incluye este número en modo Debug
        }
        else if (PlayerManager.Instance != null)
        {
            List<string> uniqueIdentifiers = PlayerManager.Instance.GetUniqueIdentifiersAndSurveyed();

            if (uniqueIdentifiers.Count == 0)
            {
                options.Add("BCH Program"); // ➕ Agregar si no hay valores
            }
            else
            {
                options.AddRange(uniqueIdentifiers); // 🔹 Agregar los identificadores de encuestados
            }
        }
        else
        {
            options.Add("BCH Program"); // ➕ Agregar si no hay valores
        }

        identifierDropdown.AddOptions(options);
        identifierDropdown.value = 0;

        originalIdentifiers = new List<string>(options);
        identifierSearchInput.onValueChanged.RemoveAllListeners();
        identifierSearchInput.onValueChanged.AddListener(FilterIdentifierDropdown);
    }

    void FilterIdentifierDropdown(string searchText)
    {
        identifierDropdown.ClearOptions();

        List<string> filteredOptions = originalIdentifiers.FindAll(option =>
            option.ToLower().Contains(searchText.ToLower()) && option != "");

        filteredOptions.Insert(0, "");

        identifierDropdown.AddOptions(filteredOptions);
        identifierDropdown.value = 0;
    }

    void OnScenarioChanged()
    {
        string selectedText = scenarioDropdown.options[scenarioDropdown.value].text;
        selectedScenario = selectedLanguage == "en" && selectedText == "Task" ? "Faena" : selectedText;

        ValidateFields();
    }

    void ClickedCountry(Button btn)
    {
        selectedCountry = GetCountryName(btn.name);
        UpdateButtonInteractivity(countryButtons, btn);
        Debug.Log(selectedCountry);
        UpdateScenarioDropdownByCountry();
        ValidateFields();
    }


    void FillFormularioData(SurveyData surveyData)
    {
        if (string.IsNullOrEmpty(surveyData.id))
        {
            Debug.LogWarning("⚠️ SurveyData es null. Se usarán valores por defecto.");
            FormularioManager.Instance.formulario.studentId = "";
            FormularioManager.Instance.formulario.instructorName = "Pia Lineros";
            FormularioManager.Instance.formulario.studentName = "Desconocido";
            FormularioManager.Instance.formulario.corporationName = "";

            FormularioManager.Instance.UpdateAnswer("Edad", "");
            FormularioManager.Instance.UpdateAnswer("Faena", "");
            FormularioManager.Instance.UpdateAnswer("Cargo", "");
            FormularioManager.Instance.UpdateAnswer("Antiguedad en el Cargo (meses)", "");
            FormularioManager.Instance.UpdateAnswer("Antiguedad en la Empresa (meses)", "");
            FormularioManager.Instance.UpdateAnswer("Empresa Subcontratista/Proveedor", "");
            FormularioManager.Instance.UpdateAnswer("Region", "");
            FormularioManager.Instance.UpdateAnswer("Pais", selectedCountry);
            FormularioManager.Instance.UpdateAnswer("Sexo", selectedGender);
            FormularioManager.Instance.UpdateAnswer("id_formulario", "");
            FormularioManager.Instance.SaveFormulario();
            return;
        }

        FormularioManager.Instance.formulario.studentId = surveyData.rut;
        FormularioManager.Instance.formulario.instructorName = "Pia Lineros";
        string[] nameRutSplit = surveyData.id.Split(':');
        string extractedName = nameRutSplit.Length > 0 ? nameRutSplit[0].Trim() : "Desconocido";
        FormularioManager.Instance.formulario.studentName = extractedName;
        FormularioManager.Instance.formulario.corporationName = surveyData.empresaMandante;

        FormularioManager.Instance.UpdateAnswer("Edad", surveyData.edad);
        FormularioManager.Instance.UpdateAnswer("Faena", surveyData.faena);
        FormularioManager.Instance.UpdateAnswer("Cargo", surveyData.cargo);
        FormularioManager.Instance.UpdateAnswer("Antiguedad en el Cargo (meses)", surveyData.antiguedadCargo);
        FormularioManager.Instance.UpdateAnswer("Antiguedad en la Empresa (meses)", surveyData.antiguedadEmpresa);
        FormularioManager.Instance.UpdateAnswer("Empresa Subcontratista/Proveedor", surveyData.empresaSubcontratista);
        FormularioManager.Instance.UpdateAnswer("Region", surveyData.region);
        FormularioManager.Instance.UpdateAnswer("Pais", selectedCountry);
        FormularioManager.Instance.UpdateAnswer("Sexo", selectedGender);
        FormularioManager.Instance.UpdateAnswer("id_formulario", surveyData.id_formulario);

        FormularioManager.Instance.SaveFormulario();
    }

    void ClickedGender(Button btn)
    {
        selectedGender = GetGenderName(btn.name);
        UpdateButtonInteractivity(genderButtons, btn);
        ValidateFields();
    }

    void UpdateButtonInteractivity(Button[] buttonGroup, Button selectedButton)
    {
        foreach (Button btn in buttonGroup)
        {
            btn.interactable = true;
        }

        selectedButton.interactable = false;
    }

    string GetCountryName(string buttonName)
    {
        switch (buttonName)
        {
            case "Peru": return "Perú";
            case "Argentina": return "Argentina";
            case "Chile": return "Chile";
            case "Australia": return "Australia"; 
            default: return "";
        }
    }

    void UpdateScenarioDropdownByCountry()
    {
        scenarioDropdown.interactable = !string.IsNullOrEmpty(selectedCountry);

        if (selectedCountry == "Australia" && selectedLanguage == "en")
        {
            scenarioDropdown.ClearOptions();
            scenarioDropdown.AddOptions(new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData(""),
                new TMP_Dropdown.OptionData("Task")
            });
            scenarioDropdown.value = 0;
        }
        else
        {
            scenarioDropdown.ClearOptions();
            scenarioDropdown.AddOptions(GetAllScenarioOptions());
            scenarioDropdown.value = 0;
        }
    }

    List<TMP_Dropdown.OptionData> GetAllScenarioOptions()
    {
        return new List<TMP_Dropdown.OptionData>
    {
        new TMP_Dropdown.OptionData(""),
        new TMP_Dropdown.OptionData("Faena"),
        new TMP_Dropdown.OptionData("Corporativo")
    };
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

    void ValidateFields()
    {
        bool allFieldsFilled = scenarioDropdown.value != 0 &&
                               identifierDropdown.value != 0 &&
                               !string.IsNullOrEmpty(selectedCountry) &&
                               !string.IsNullOrEmpty(selectedGender);

        startButton.interactable = allFieldsFilled;
    }

    void LoadSelectedScene()
    {
        PlayerPrefs.SetString("SelectedCountry", selectedCountry);
        PlayerPrefs.SetString("SelectedScenario", selectedScenario);
        PlayerPrefs.SetString("SelectedGender", selectedGender); // ✅ Guardamos el género
        PlayerPrefs.Save();

        FormularioManager.Instance.formulario.fields["Pais"] = selectedCountry;
        FormularioManager.Instance.formulario.fields["Sexo"] = selectedGender;

        string selectedId = identifierDropdown.options[identifierDropdown.value].text;
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
                    SceneManager.LoadScene(lobbyIndex);
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
                SceneManager.LoadScene(lobbyIndex);
            }
        }

        void SetFieldValue(string key, object value)
        {
            if (FormularioManager.Instance.formulario.fields.ContainsKey(key))
            {
                FormularioManager.Instance.formulario.fields[key] = value; // 🔄 Reemplazar si existe
            }
            else
            {
                FormularioManager.Instance.formulario.fields.Add(key, value); // ➕ Agregar si no existe
            }
        }
    }
}
