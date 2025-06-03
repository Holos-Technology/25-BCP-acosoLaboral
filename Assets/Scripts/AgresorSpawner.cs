using UnityEngine;

public class AgresorSpawner : MonoBehaviour
{
    [Header("Prefabs de Agresores por País y Escenario")]
    public GameObject chileCorporativo;
    public GameObject chileFaena;
    public GameObject argentinaCorporativo;
    public GameObject argentinaFaena;
    public GameObject peruCorporativo;
    public GameObject peruFaena;
    public GameObject australiaCorporativo;
    public GameObject australiaFaena;

    private void Start()
    {
        SpawnAgresor();
    }

    private void SpawnAgresor()
    {
        string country = PlayerPrefs.GetString("SelectedCountry", "Chile");
        string scenario = PlayerPrefs.GetString("SelectedScenario", "Faena");

        GameObject prefabToSpawn = null;

        switch (country)
        {
            case "Chile":
                prefabToSpawn = scenario == "Corporativo" ? chileCorporativo : chileFaena;
                break;
            case "Argentina":
                prefabToSpawn = scenario == "Corporativo" ? argentinaCorporativo : argentinaFaena;
                break;
            case "Perú":
            case "Peru":
                prefabToSpawn = scenario == "Corporativo" ? peruCorporativo : peruFaena;
                break;
            case "Australia":
                prefabToSpawn = scenario == "Corporativo" ? australiaCorporativo : australiaFaena;
                break;
            default:
                Debug.LogWarning($"País no reconocido ({country}), usando Chile/Faena por defecto.");
                prefabToSpawn = chileFaena;
                break;
        }

        if (prefabToSpawn != null)
        {
            Transform spawnPoint = SceneGameManager.Instance.defaultAggressorPosition;
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, parent: transform);
        }
        else
        {
            Debug.LogError("❌ No se encontró un prefab válido para el agresor.");
        }
    }
}
