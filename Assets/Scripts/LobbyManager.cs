using System.Collections;
using Oculus.Movement.AnimationRigging;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    
    [SerializeField] private GameObject FaenaScenario;
    [SerializeField] private GameObject CorporativoScenario;

    [SerializeField] private GameObject AustFaena;
    [SerializeField] private GameObject AustCorporativo;
    [SerializeField] private GameObject ArgentinaFaena;
    [SerializeField] private GameObject ArgentinaCorporativo;
    [SerializeField] private GameObject PeruFaena;
    [SerializeField] private GameObject PeruCorporativo;
    [SerializeField] private GameObject ChileFaena;
    [SerializeField] private GameObject ChileCorporativo;

    private Transform player;
    
    [SerializeField] private SceneController sceneManager;
    [SerializeField] private int faena;
    [SerializeField] private int corporativo;
    public void LoadScenario()
    {
        string escenario = PlayerPrefs.GetString("SelectedScenario", "Faena");
        if (escenario == "Faena")
            FaenaScenario.SetActive(true);
        else
            CorporativoScenario.SetActive(true);
    }

    public void LoadCharacter()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        string escenario = PlayerPrefs.GetString("SelectedScenario", "Faena");
        string country = PlayerPrefs.GetString("SelectedCountry", "Chile");

        GameObject prefabToSpawn = null;

        if (escenario == "Faena")
        {
            switch (country)
            {
                case "Chile": prefabToSpawn = ChileFaena; break;
                case "Argentina": prefabToSpawn = ArgentinaFaena; break;
                case "Perú":
                case "Peru": prefabToSpawn = PeruFaena; break;
                case "Australia": prefabToSpawn = AustFaena; break;
            }
        }
        else
        {
            switch (country)
            {
                case "Chile": prefabToSpawn = ChileCorporativo; break;
                case "Argentina": prefabToSpawn = ArgentinaCorporativo; break;
                case "Perú":
                case "Peru": prefabToSpawn = PeruCorporativo; break;
                case "Australia": prefabToSpawn = AustCorporativo; break;
            }
        }

        if (prefabToSpawn != null)
        {
            GameObject character = Instantiate(prefabToSpawn, player.position, Quaternion.identity);
            character.transform.SetParent(player);
            StartCoroutine(ResetOVRBodyAndRetargeting(character));
        }
        else
        {
            Debug.LogWarning("No se encontró un prefab válido para el país/escenario seleccionado.");
        }
    }

    private IEnumerator ResetOVRBodyAndRetargeting(GameObject character)
    {
        float timeout = 10f;
        float elapsed = 0f;

        OVRBody ovrBody = null;
        RetargetingLayer retargetLayer = null;

        // Espera hasta que ambos existan o se acabe el timeout
        while ((ovrBody == null || retargetLayer == null) && elapsed < timeout)
        {
            if (ovrBody == null)
                ovrBody = character.GetComponentInChildren<OVRBody>(true); // puede estar desactivado inicialmente
            if (retargetLayer == null)
                retargetLayer = character.GetComponentInChildren<RetargetingLayer>(true);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (ovrBody != null)
        {
            ovrBody.enabled = false;
            yield return null;
            ovrBody.enabled = true;
            Debug.Log($"✅ OVRBody reiniciado en: {character.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró OVRBody en {character.name}.");
        }

        if (retargetLayer != null)
        {
            retargetLayer.enabled = false;
            yield return null;
            retargetLayer.enabled = true;
            Debug.Log($"✅ OVRSkeletonRetargetingLayer reiniciado en: {character.name}");
        }
        else
        {
            Debug.Log("ℹ️ El personaje no tiene OVRSkeletonRetargetingLayer.");
        }
    }

    public void LoadNextScene()
    {        
        string escenario = PlayerPrefs.GetString("SelectedScenario", "Faena");

        if (escenario == "Faena")
        {
            sceneManager.LoadScene(faena);
        }
        else
        {
            sceneManager.LoadScene(corporativo);
        }
    }

}
