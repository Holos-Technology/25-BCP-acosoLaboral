using UnityEngine;
using UnityEngine.UI;
using static CountrySelector;

public class LenguageSelector : MonoBehaviour
{
    [SerializeField] private Button[] ButtonsLenguage;
    [SerializeField] private StartingCanvasManager StartingCanvasManager;
    [SerializeField] private CountrySelector countrySelector;
    [SerializeField] private Button ButtonConfirm;
    string lenguage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        ButtonsLenguage[0].onClick.AddListener(() => {
            countrySelector.GenerateButtons(Language.Spanish);
            lenguage = Language.Spanish.ToString();
            ConfirmButtonValidate();
            Debug.Log(lenguage);
        });

        ButtonsLenguage[1].onClick.AddListener(() => {
            countrySelector.GenerateButtons(Language.English);
            lenguage = Language.English.ToString();
            ConfirmButtonValidate();
            Debug.Log(lenguage);
        });

        ButtonConfirm.onClick.AddListener(() => {
            Debug.Log(lenguage);
            PlayerPrefs.SetString("SelectedLenguage", lenguage);
            StartingCanvasManager.NextPanel();
        });
    }

    private void OnEnable()
    {
        ButtonConfirm.interactable = false;
    }

    private void ConfirmButtonValidate()
    {
        if (ButtonConfirm.interactable==false)
        {
            ButtonConfirm.interactable = true;
        }
    }
}
