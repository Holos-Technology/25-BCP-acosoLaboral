using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
public class TestSymbio : MonoBehaviour
{
    private string apiServer = "https://api-test.symbioxr.com";  // 🔹 Servidor de la API
    private string loginEndpoint = "/login";  // 🔹 Endpoint de login

    private string email = "jramirez@holosglobal.com";  // 🔹 Usuario de login
    private string password = "Holos1234.";  // 🔹 Contraseña de login
    private string authToken = "";  // 🔹 Token de autenticación de SymbioXR

    void Start()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        string loginUrl = $"{apiServer}{loginEndpoint}";
        var loginData = new { email, password };
        string loginJson = JsonConvert.SerializeObject(loginData);

        using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(loginJson);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"🔄 Intentando autenticación en {loginUrl}...");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                if (responseJson.ContainsKey("token"))
                {
                    authToken = responseJson["token"].ToString();
                    Debug.Log($"✅ Token obtenido correctamente: {authToken}");
                }
                else
                {
                    Debug.LogError($"❌ No se encontró el token en la respuesta: {request.downloadHandler.text}");
                }
            }
            else
            {
                Debug.LogError($"❌ Error al autenticarse: {request.error}\nRespuesta: {request.downloadHandler.text}");
            }
        }
    }
}

