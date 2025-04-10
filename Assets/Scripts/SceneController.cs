using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneAsync(index));
    }
    private IEnumerator LoadSceneAsync(int index)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;

        // Esperar hasta que la carga esté completa
        while (!asyncOperation.isDone)
        {
            // Cuando el progreso está en 0.9 significa que la escena está lista
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
