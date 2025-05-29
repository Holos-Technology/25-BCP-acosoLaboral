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

    private void Awake()
    {
        scenes.Add("Corporativo", 6);
        scenes.Add("Faena", 5);

        sceneDropdawn.onValueChanged.AddListener(delegate { OnDropdownSceneChanged(sceneDropdawn.value); });
        
        searchInput.onValueChanged.AddListener(FilterIdentifiers);

        buttonConfirm.onClick.AddListener(ButtonConfirm);
        buttonBack.onClick.AddListener(StartingCanvasManager.PreviousPanel);

        sceneDropdawn.onValueChanged.AddListener(_ => ValidateConfirmButton());
        idDropdawn.onValueChanged.AddListener(_ => ValidateConfirmButton());
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

        bool isValid = !string.IsNullOrEmpty(selectedScene) && !string.IsNullOrEmpty(selectedId);

        buttonConfirm.interactable = isValid;
    }

    private void ButtonConfirm()
    {
        SceneManager.LoadScene(intScene);
    }
}
