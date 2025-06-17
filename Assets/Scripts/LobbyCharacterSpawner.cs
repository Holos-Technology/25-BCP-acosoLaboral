using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CountryCharacters
    {
        public string countryName;
        public GameObject corporativo;
        public GameObject faena;
    }

    [Header("Characters por país")]
    [SerializeField] private List<CountryCharacters> charactersPorPais;

    [Header("Escenarios")]
    [SerializeField] private GameObject faenaScenario;
    [SerializeField] private GameObject corporativoScenario;

    void Awake()
    {
        foreach (var entry in charactersPorPais)
        {
            if (entry == null)
            {
                Debug.LogError("¡Una entrada en charactersPorPais es null! Revisa el Inspector.");
                continue;
            }

            if (entry.corporativo == null)
                Debug.LogWarning($"[{entry.countryName}] no tiene prefab de Corporativo");

            if (entry.faena == null)
                Debug.LogWarning($"[{entry.countryName}] no tiene prefab de Faena");

            entry.corporativo?.SetActive(true);
            entry.faena?.SetActive(true);
        }

        DisableAllCharacters();
    }

    void Start()
    {
        ActivateCharacter();
    }

    void ActivateCharacter()
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "");
        string selectedScenario = PlayerPrefs.GetString("SelectedScenario", "");

        // Desactivar todos
        DisableAllCharacters();

        // Activar escenario
        corporativoScenario.SetActive(selectedScenario == "Corporativo");
        faenaScenario.SetActive(selectedScenario == "Faena");

        // Activar personaje
        GameObject selectedCharacter = GetSelectedCharacter(selectedCountry, selectedScenario);

        if (selectedCharacter != null)
        {
            ActivateAndInitTracking(selectedCharacter);
        }
        else
        {
            Debug.LogWarning("No se encontró un personaje válido para la selección.");
        }
    }

    void DisableAllCharacters()
    {
        foreach (var entry in charactersPorPais)
        {
            entry.corporativo?.SetActive(false);
            entry.faena?.SetActive(false);
        }
    }

    GameObject GetSelectedCharacter(string country, string scenario)
    {
        foreach (var entry in charactersPorPais)
        {
            if (entry.countryName.Equals(country, System.StringComparison.OrdinalIgnoreCase))
            {
                return scenario == "Corporativo" ? entry.corporativo : entry.faena;
            }
        }
        return null;
    }

    void ActivateAndInitTracking(GameObject character)
    {
        character.SetActive(true);
        StartCoroutine(ResetOVRBodyNextFrame(character));
    }

    IEnumerator ResetOVRBodyNextFrame(GameObject character)
    {
        while (!OVRManager.isHmdPresent || OVRInput.GetActiveController() == OVRInput.Controller.None)
        {
            yield return null;
        }

        var ovrBody = character.GetComponent<OVRBody>();
        if (ovrBody != null)
        {
            ovrBody.enabled = false;
            yield return null;
            ovrBody.enabled = true;

            Debug.Log($"OVRBody reiniciado correctamente en: {character.name}");
        }
        else
        {
            Debug.LogWarning($"El personaje {character.name} no tiene OVRBody.");
        }
    }
}
