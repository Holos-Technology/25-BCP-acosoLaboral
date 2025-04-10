using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [Header("Object Sequence")]
    public GameObject[] loadingObjects; // Arrastra los 4 objetos desde el Inspector
    private int currentObjectIndex = 0;
    private float objectDisplayTime = 5f;
    private bool isCsvLoaded = false; // Estado de carga del JSON

    // Evento para avisar cuando termina la carga y la secuencia de objetos
    public delegate void LoadingCompleteEvent();
    public static event LoadingCompleteEvent OnLoadingComplete;

    [SerializeField] SceneController sceneController;   
    void Start()
    {
        StartCoroutine(ShowObjectsSequence());

        // Suscribirse al evento de GoogleSheetLoader
        GoogleSheetLoader.OnJsonLoaded += HandleJsonLoaded;
    }

    private void HandleJsonLoaded()
    {
        isCsvLoaded = true; // Marca que la carga ha finalizado
    }

    IEnumerator ShowObjectsSequence()
    {
        while (currentObjectIndex < loadingObjects.Length)
        {
            yield return StartCoroutine(FadeInAndOut(loadingObjects[currentObjectIndex]));
            currentObjectIndex++;
        }

        yield return new WaitUntil(() => isCsvLoaded);

        Debug.Log("✅ Se completó la carga del JSON y la secuencia de objetos.");
        OnLoadingComplete?.Invoke();
        sceneController.LoadScene(1);
    }

    IEnumerator FadeInAndOut(GameObject obj)
    {
        // Desactivar todos los objetos primero
        foreach (var o in loadingObjects)
            o.SetActive(false);

        obj.SetActive(true);

        // Buscar componente Image en hijo (o en el mismo objeto si aplica)
        Image img = obj.GetComponentInChildren<Image>();
        if (img == null)
        {
            Debug.LogError("❌ No se encontró componente Image en " + obj.name);
            yield break;
        }

        float fadeDuration = 1f;
        Color originalColor = img.color;
        Color color = originalColor;
        color.a = 0;
        img.color = color;

        // Fade In
        float t = 0;
        while (t < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            img.color = color;
            t += Time.deltaTime;
            yield return null;
        }
        color.a = 1f;
        img.color = color;

        // Mantener visible
        yield return new WaitForSeconds(objectDisplayTime);

        // Fade Out
        t = 0;
        while (t < fadeDuration)
        {
            color.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            img.color = color;
            t += Time.deltaTime;
            yield return null;
        }
        color.a = 0f;
        img.color = color;

        obj.SetActive(false);
    }

    void OnDestroy()
    {
        GoogleSheetLoader.OnJsonLoaded -= HandleJsonLoaded;
    }
}
