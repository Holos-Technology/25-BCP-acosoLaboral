using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountrySelector : MonoBehaviour
{
    public enum Language
    {
        Spanish,
        English
    }

    [SerializeField] private Transform contentParent;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Button ButtonConfirm;
    [SerializeField] private Button buttonBack;
    [SerializeField] private StartingCanvasManager StartingCanvasManager;

    string CountryPlayerPref;

    private void Awake()
    {
        ButtonConfirm.onClick.AddListener(() =>
        {
            CountryConfirm();
            StartingCanvasManager.NextPanel();
        });

        buttonBack.onClick.AddListener(StartingCanvasManager.PreviousPanel);
    }

    private void OnEnable()
    {
        ButtonConfirm.interactable = false;
    }

    public void GenerateButtons(Language lang)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        switch (lang)
        {
            case Language.Spanish:
                CreateButton("Argentina", () => CountrySelection("Argentina"));
                CreateButton("Perú", () => CountrySelection("Perú"));
                CreateButton("Chile", () => CountrySelection("Chile"));
                break;

            case Language.English:
                CreateButton("Australia", () => CountrySelection("Australia"));
                break;
        }
    }

    private void CreateButton(string label, UnityAction onClick)
    {
        Button btn = Instantiate(buttonPrefab, contentParent);
        btn.GetComponentInChildren<TMPro.TMP_Text>().text = label;
        btn.onClick.AddListener(onClick);
    }

    private void CountrySelection(string country)
    {
        CountryPlayerPref = country;

        if (ButtonConfirm.interactable ==false)
        {
            ButtonConfirm.interactable = true;
        }

        Debug.Log(CountryPlayerPref);
    }

    private void CountryConfirm()
    {
        PlayerPrefs.SetString("SelectedCountry", CountryPlayerPref);
    }
}
