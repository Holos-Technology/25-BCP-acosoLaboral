using System.Collections.Generic;
using UnityEngine;

public class SetupCredits : MonoBehaviour
{
    [SerializeField] List<GameObject> SpanishObjects;
    [SerializeField] List<GameObject> EnglishOj;
    [SerializeField] List<LocalizedText> localizedTexts;
    
    private void Start()
    {
        ApplyLocalizedText();
    }

    private void ApplyLocalizedText()
    {
        string lang = PlayerPrefs.GetString("language", "es");
        bool isEnglish = lang == "en";

        // Desactivar todos los objetos
        foreach (var obj in SpanishObjects)
            if (obj != null) obj.SetActive(false);

        foreach (var obj in EnglishOj)
            if (obj != null) obj.SetActive(false);

        // Activar los objetos según el idioma
        var activeList = isEnglish ? EnglishOj : SpanishObjects;
        foreach (var obj in activeList)
            if (obj != null) obj.SetActive(true);

        // Aplicar localización de texto
        foreach (var entry in localizedTexts)
        {
            if (entry.targetText == null) continue;
            entry.targetText.text = isEnglish ? entry.english : entry.spanish;
        }
    }
}
