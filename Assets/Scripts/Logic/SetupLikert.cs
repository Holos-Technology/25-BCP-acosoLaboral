using System;
using System.Collections.Generic;
using UnityEngine;

public class SetupLikert : MonoBehaviour
{   
    [SerializeField] private List<LocalizedText> feelings;


    private void Start()
    {
        ApplyLocalizedText();
    }

    private void ApplyLocalizedText()
    {
        string lang = PlayerPrefs.GetString("language", "es");
        bool isEnglish = lang == "en";

        foreach (var entry in feelings)
        {
            if (entry.targetText == null) continue;
            entry.targetText.text = isEnglish ? entry.english : entry.spanish;
        }
    }
}
