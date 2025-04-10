using UnityEngine;
using UnityEngine.SceneManagement;

public class SpecialLobbyLoad : MonoBehaviour
{
    [SerializeField] private int faenaIndex = 1; // Índice de la escena Faena
    [SerializeField] private int corporativeIndex = 2; // Índice de la escena Corporativa
    [SerializeField] private int defaultLobbyIndex = 0; // Índice del Lobby por defecto

    public void OnLoadScene()
    {
        // Recuperamos el escenario seleccionado
        string selectedScenario = PlayerPrefs.GetString("SelectedScenario", "Lobby");

        // Determinamos el índice de la escena según el escenario guardado
        int sceneIndex = defaultLobbyIndex;

        if (selectedScenario == "Faena")
        {
            sceneIndex = faenaIndex;
        }
        else if (selectedScenario == "Corporativo")
        {
            sceneIndex = corporativeIndex;
        }

        // Imprimir información para depuración
        Debug.Log($"🎭 Escenario seleccionado: {selectedScenario}");
        Debug.Log($"🎮 Cargando escena con índice: {sceneIndex}");

        // Cargar la escena correspondiente
        SceneManager.LoadScene(sceneIndex);
    }
}
