using System.Collections;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{

    [Header("Characters por país")]
    [SerializeField] private GameObject chileCharacter;
    [SerializeField] private GameObject peruCharacter;
    [SerializeField] private GameObject argentinaCharacter;
    [SerializeField] private GameObject australiaCharacter;

  void Start()
    {
        ActivateCharacter();
    }

    void ActivateCharacter()
    {
        string selectedCountry = PlayerPrefs.GetString("SelectedCountry", "");

        GameObject selectedCharacter = null;

        switch (selectedCountry)
        {
            case "Chile":
                selectedCharacter = chileCharacter;
                break;
            case "Perú":
                selectedCharacter = peruCharacter;
                break;
            case "Peru":
                selectedCharacter = peruCharacter;
                break;
            case "Argentina":
                selectedCharacter = argentinaCharacter;
                break;
            case "Australia":
                selectedCharacter = australiaCharacter;
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

        // Activar el personaje correcto
        selectedCharacter.SetActive(true);
       // StartCoroutine(ResetOVRBodyNextFrame(selectedCharacter));

        // Destruir los personajes no seleccionados
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
        
        if (australiaCharacter != null && australiaCharacter != activeCharacter)
            Destroy(australiaCharacter);
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
