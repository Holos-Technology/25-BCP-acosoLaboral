using System.Collections;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{

    [Header("Characters por país")]
    [SerializeField] private GameObject chileCharacter;
    [SerializeField] private GameObject peruCharacter;
    [SerializeField] private GameObject argentinaCharacter;

  void Start()
    {
        ActivateCharacter();
    }

    void ActivateCharacter()
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "Chile"); // Chile por defecto

        GameObject selectedCharacter = null;

        switch (selectedCountry)
        {
            case "Chile":
                selectedCharacter = chileCharacter;
                break;
            case "Perú":
            case "Peru":
                selectedCharacter = peruCharacter;
                break;
            case "Argentina":
                selectedCharacter = argentinaCharacter;
                break;
            default:
                Debug.LogWarning("🌍 País no reconocido en PlayerPrefs: " + selectedCountry);
                break;
        }

        if (selectedCharacter == null)
        {
            Debug.LogWarning("⚠️ No se encontró personaje válido para el país seleccionado.");
            return;
        }

        selectedCharacter.SetActive(true);
        StartCoroutine(ResetOVRBodyNextFrame(selectedCharacter));
        DestroyUnusedCharacters(selectedCharacter);
    }
    void DestroyUnusedCharacters(GameObject activeCharacter)
    {
        if (chileCharacter != null && chileCharacter != activeCharacter)
            Destroy(chileCharacter);

        if (peruCharacter != null && peruCharacter != activeCharacter)
            Destroy(peruCharacter);

        if (argentinaCharacter != null && argentinaCharacter != activeCharacter)
            Destroy(argentinaCharacter);
    }

    private IEnumerator ResetOVRBodyNextFrame(GameObject character)
    {
        yield return null;

        var ovrBody = character.GetComponent<OVRBody>();
        if (ovrBody != null)
        {
            ovrBody.enabled = false;
            yield return null;
            ovrBody.enabled = true;
            Debug.Log($"✅ OVRBody reiniciado en: {character.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ El personaje {character.name} no tiene OVRBody.");
        }
    }
}
