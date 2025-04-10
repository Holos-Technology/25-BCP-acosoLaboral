using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class FormularioManager : MonoBehaviour
{
   public static FormularioManager Instance { get; private set; }

    public FormularioData formulario;
    private string apiServer = "https://api-test.symbioxr.com"; // 🔹 Servidor de la API
    private string loginEndpoint = "/login"; // 🔹 Endpoint de login
    private string experienceEndpoint = "/experience"; // 🔹 Endpoint para enviar datos
    private string email = "virtualbrainvr@gmail.com"; // 🔹 Usuario de login
    private string password = "Holos1234."; // 🔹 Contraseña de login
    private string authToken = ""; // 🔹 Token de autenticación de SymbioXR
    public UnityEvent onFormularioEnviado = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeFormulario();
    }

    void InitializeFormulario()
    {
        formulario = new FormularioData
        {
            date = System.DateTime.Now.ToString("dd-MM-yyyy"),
            time = System.DateTime.Now.ToString("HH:mm:ss"),
            simulatorName = "AL",
            simulatorVersion = "1.0",
            platform = "VR",
            instructorName = "Pia Lineros",
            studentId = "4123",
            corporationName = "ENAEX",
            timeSpend = "15",
            studentName = "",
            finalNote = 0,
            fields = new Dictionary<string, object>()
        };
    }

    public void UpdateAnswer(string question, object answer)
    {
        if (formulario.fields == null)
        {
            formulario.fields = new Dictionary<string, object>();
        }

        if (formulario.fields.ContainsKey(question))
        {
            formulario.fields[question] = answer;
        }
        else
        {
            formulario.fields.Add(question, answer);
        }
    }

    public void SaveFormulario()
    {
        if (formulario.fields == null || formulario.fields.Count == 0)
        {
            Debug.LogWarning("⚠️ No hay datos en 'fields', asegurando estructura antes de guardar.");
            formulario.fields = new Dictionary<string, object>
            {
                { "Edad", "" },
                { "Sexo", "" },
                { "Faena", "" },
                { "Cargo", "" },
                { "Antiguedad en el Cargo (meses)", 0 },
                { "Antiguedad en la Empresa (meses)", 0 },
                { "Empresa Subcontratista/Proveedor", "" },
                { "Region", "" },
                { "Pais", "" },
                { "id_formulario", "" }
            };
        }
        formulario.activityId = GetActivityId();
        string escenario = PlayerPrefs.GetString("SelectedScenario", "Faena");
        string jugadorMasculinoKey = $"Jugador Masculino {escenario}";
        string jugadorFemeninoKey = $"Jugador Femenino {escenario}";

        var originalFields = formulario.fields;
        var orderedFields = new Dictionary<string, object>();

        // 🔹 Primero agregar todos los campos que NO son de jugador
        foreach (var kvp in originalFields)
        {
            if (kvp.Key != jugadorMasculinoKey && kvp.Key != jugadorFemeninoKey)
            {
                orderedFields[kvp.Key] = kvp.Value;
            }
        }

        // 🔹 Luego agregar los campos de jugador al final (si existen)
        if (originalFields.ContainsKey(jugadorMasculinoKey))
            orderedFields[jugadorMasculinoKey] = originalFields[jugadorMasculinoKey];

        if (originalFields.ContainsKey(jugadorFemeninoKey))
            orderedFields[jugadorFemeninoKey] = originalFields[jugadorFemeninoKey];

        // Reemplazar en el formulario
        formulario.fields = orderedFields;

        // 🔹 Guardar como JSON
        string json = JsonConvert.SerializeObject(formulario, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/formulario.json", json);
        Debug.Log($"✅ Formulario guardado correctamente en: {Application.persistentDataPath}");

    }

    public void SendFormulario()
    {
        formulario.activityId = GetActivityId();

        if (string.IsNullOrEmpty(formulario.activityId))
        {
            Debug.LogError("❌ No se pudo determinar el activityId.");
            return;
        }

        Debug.Log("🔄 Iniciando autenticación para enviar formulario...");
        StartCoroutine(LoginAndSendForm());
    }

    private IEnumerator LoginAndSendForm()
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
                    formulario.token = authToken; // ✅ Guardar el token en el formulario
                    DateTime horaInicio;
                    if (DateTime.TryParseExact(formulario.time, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out horaInicio))
                    {
                        TimeSpan tiempoTranscurrido = DateTime.Now - horaInicio;
                        formulario.timeSpend = $"{(int)tiempoTranscurrido.TotalSeconds}";
                    }
                    else
                    {
                        Debug.LogWarning("❌ No se pudo convertir 'formulario.time' a DateTime.");
                    }
                    SaveFormulario(); // ✅ Guardar el JSON con el token actualizado
                    Debug.Log($"✅ Token obtenido correctamente: {authToken}");
                    yield return StartCoroutine(SendFormData());
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

    private IEnumerator SendFormData()
    {
        string apiUrl = $"{apiServer}{experienceEndpoint}";

        // Validar JSON antes de enviarlo
        string json = JsonConvert.SerializeObject(formulario, Formatting.Indented);
        Debug.Log($"📤 JSON a enviar:\n{json}");

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

            Debug.Log($"🚀 Enviando formulario a {apiUrl}...");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {                
                onFormularioEnviado.Invoke(); // 🔹 Disparar el evento cuando el formulario se envía correctamente
                Debug.Log($"✅ Formulario enviado correctamente a SymbioXR: {request.downloadHandler.text}");
            }
            else
            {                
                onFormularioEnviado.Invoke(); // 🔹 Disparar el evento cuando el formulario se envía correctamente
                Debug.LogError($"❌ Error al enviar formulario: {request.error}\nRespuesta: {request.downloadHandler.text}");
            }
        }
    }

    private string GetActivityId()
    {
        string selectedScenario = PlayerPrefs.GetString("SelectedScenario", "Faena");

        if (selectedScenario == "Faena")
            return "MT7uVslD8q16wZuSYFZ3"; // 📌 ID para Faena
        else if (selectedScenario == "Corporativo")
            return "UHMsiybmN4tyxJGQxyvi"; // 📌 ID para Corporativo

        return ""; // 📌 Si el escenario no es válido
    }
}
[Serializable]
public class FormularioData
{
    public string token = "";
    public string activityId = "";
    public string date = "";
    public string time = "";
    public string simulatorName = "";
    public string simulatorVersion = "";
    public string platform = "";
    public string instructorName = "";
    public string studentId = "";
    public string corporationName = "";
    public string timeSpend = "0f";
    public string studentName = "";
    public int finalNote = 0;
    public Dictionary<string, object> fields = new Dictionary<string, object>();
    public Dictionary<string, object> extrafields = new Dictionary<string, object>();
}