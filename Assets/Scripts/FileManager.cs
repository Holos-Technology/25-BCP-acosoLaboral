using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


public class FileManager 
{
    private static string persistentPath = Application.persistentDataPath;
    private static string streamingPath = Application.streamingAssetsPath;

    public static string GetJsonPath(string fileName)
    {
        string persistentFile = Path.Combine(persistentPath, fileName);
        string streamingFile = Path.Combine(streamingPath, fileName);

        return File.Exists(persistentFile) ? persistentFile : streamingFile;
    }

    public static bool IsJsonInPersistentPath(string fileName)
    {
        return File.Exists(Path.Combine(persistentPath, fileName));
    }

    public static IEnumerator CopyJsonToPersistentIfNeeded(string fileName)
    {
        string persistentFile = Path.Combine(persistentPath, fileName);
        string streamingFile = Path.Combine(streamingPath, fileName);

        if (!File.Exists(persistentFile))
        {
            Debug.Log($"🔄 Copiando {fileName} desde StreamingAssets a persistentDataPath...");

            if (Application.platform == RuntimePlatform.Android)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(streamingFile))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        File.WriteAllText(persistentFile, request.downloadHandler.text);
                        Debug.Log($"✅ Copia completa en: {persistentFile}");
                    }
                    else
                    {
                        Debug.LogError($"❌ Error copiando {fileName}: {request.error}");
                    }
                }
            }
            else
            {
                if (File.Exists(streamingFile))
                {
                    File.Copy(streamingFile, persistentFile);
                    Debug.Log($"✅ Copia completa en: {persistentFile}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Archivo {fileName} no encontrado en StreamingAssets.");
                }
            }
        }
    }

    public static IEnumerator LoadJson(string fileName, System.Action<string> callback)
    {
        string path = GetJsonPath(fileName);

        if (Application.platform == RuntimePlatform.Android && path == Path.Combine(streamingPath, fileName))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback(request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError($"❌ Error al leer {fileName}: {request.error}");
                    callback(null);
                }
            }
        }
        else
        {
            callback(File.Exists(path) ? File.ReadAllText(path) : null);
        }
    }

    public static void SaveJson(string jsonData, string fileName)
    {
        string path = Path.Combine(persistentPath, fileName);
        File.WriteAllText(path, jsonData);
        Debug.Log($"✅ JSON guardado en: {path}");
    }
}
