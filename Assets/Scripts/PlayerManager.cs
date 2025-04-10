using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
      public static PlayerManager Instance { get; private set; }
    public List<SurveyData> playerSurveys = new List<SurveyData>();
    public List<SurveyIdentifier> surveyIdentifiers = new List<SurveyIdentifier>();

    void Awake()
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
    }

    private void Start()
    {
        StartCoroutine(LoadAllSurveyData());
        GoogleSheetLoader.OnJsonLoaded += () => StartCoroutine(LoadAllSurveyData());
    }

    IEnumerator LoadAllSurveyData()
    {
        yield return StartCoroutine(LoadJsonFile("dataCorporativo.json", DeserializeSurveyData));
        yield return StartCoroutine(LoadJsonFile("dataFaena.json", DeserializeSurveyData));

        // 🔄 Cargar identificadores después
        yield return StartCoroutine(LoadJsonFile("faena_f.json", DeserializeIdentifiers));
        yield return StartCoroutine(LoadJsonFile("faena_m.json", DeserializeIdentifiers));
        yield return StartCoroutine(LoadJsonFile("corp_m.json", DeserializeIdentifiers));
        yield return StartCoroutine(LoadJsonFile("corp_f.json", DeserializeIdentifiers));

        // 🔄 Asignar id_formulario a cada encuesta si existe coincidencia
        AssignFormIds();
    }

    IEnumerator LoadJsonFile(string fileName, Action<string> callback)
    {
        yield return StartCoroutine(FileManager.LoadJson(fileName, (json) =>
        {
            if (!string.IsNullOrEmpty(json))
            {
                callback(json);
            }
            else
            {
                Debug.LogWarning($"⚠️ {fileName} está vacío o no existe.");
            }
        }));
    }

    public SurveyData GetSurveyDataById(string id)
    {
        return playerSurveys.FirstOrDefault(s => s.id == id);
    }

    public List<string> GetUniqueIdentifiersAndSurveyed()
    {
        return playerSurveys
            .Where(s => s.encuestado)
            .Select(s => s.id)
            .Distinct()
            .ToList();
    }

    void DeserializeSurveyData(string json)
    {
        try
        {
            List<Dictionary<string, string>> rawData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

            foreach (var entry in rawData)
            {
                if (!entry.ContainsKey("id") || string.IsNullOrWhiteSpace(entry["id"])) continue;

                string id = entry["id"].Trim();
                bool encuestado = entry.ContainsKey("encuestado") && bool.TryParse(entry["encuestado"].Trim(), out bool enc) ? enc : false;
                string nombre = entry.GetValueOrDefault("Nombre", "Desconocido").Trim();
                string apellido = entry.GetValueOrDefault("Apellido", "N/A").Trim();
                string rut = entry.GetValueOrDefault("RUT", "N/A").Trim();
                int edad = int.TryParse(entry.GetValueOrDefault("Edad", "-1"), out int e) ? e : -1;
                string sexo = entry.GetValueOrDefault("Sexo", "N/A").Trim();
                string faena = entry.GetValueOrDefault("Faena", "N/A").Trim();
                string cargo = entry.GetValueOrDefault("Cargo", "N/A").Trim();
                int antiguedadCargo = int.TryParse(entry.GetValueOrDefault("Antiguedad en el cargo", "-1"), out int ac) ? ac : -1;
                int antiguedadEmpresa = int.TryParse(entry.GetValueOrDefault("Antiguedad en la empresa", "-1"), out int ae) ? ae : -1;
                string empresaMandante = entry.GetValueOrDefault("Empresa mandante", "N/A").Trim();
                string empresaSubcontratista = entry.GetValueOrDefault("Empresa Subcontratista/Proveedor", "N/A").Trim();
                string region = entry.GetValueOrDefault("Region", "N/A").Trim();
                string pais = entry.GetValueOrDefault("Pais", "N/A").Trim();

                SurveyData survey = new SurveyData(id, encuestado, nombre, apellido, rut, edad, sexo, faena, cargo, antiguedadCargo, antiguedadEmpresa, empresaMandante, empresaSubcontratista, region, pais, "");

                if (encuestado)
                    playerSurveys.Add(survey);
            }

            Debug.Log($"✅ Datos cargados. Encuestados válidos: {playerSurveys.Count}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error al leer archivo de datos: {e.Message}");
            InitializeEmptyData();
        }
    }

    void DeserializeIdentifiers(string json)
    {
        try
        {
            var identifiers = JsonConvert.DeserializeObject<List<SurveyIdentifier>>(json);
            surveyIdentifiers.AddRange(identifiers);
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error al deserializar identificadores: {e.Message}");
        }
    }

    void AssignFormIds()
    {
        for (int i = 0; i < playerSurveys.Count; i++)
        {
            var survey = playerSurveys[i];
            var match = surveyIdentifiers.FirstOrDefault(s => s.identificador == survey.rut);
            if (match.identificador != null)
            {
                survey.id_formulario = match.id_formulario;
                playerSurveys[i] = survey;
            }
        }

        Debug.Log("✅ id_formulario asignado a los encuestados cargados.");
    }

    void InitializeEmptyData()
    {
        playerSurveys.Clear();
        SurveyData emptyData = new SurveyData("N/A", false, "Desconocido", "N/A", "N/A", -1, "N/A", "N/A", "N/A", -1, -1, "N/A", "N/A", "N/A", "N/A", "N/A");
        playerSurveys.Add(emptyData);
    }
}
[Serializable]
public struct SurveyData
{
    public string id;
    public bool encuestado;
    public string nombre;
    public string apellido;
    public string rut;
    public int edad;
    public string sexo;
    public string faena;
    public string cargo;
    public int antiguedadCargo;
    public int antiguedadEmpresa;
    public string empresaMandante;
    public string empresaSubcontratista;
    public string region;
    public string pais;
    public string id_formulario; 
    
    public SurveyData(
        string id, bool encuestado, string nombre, string apellido, string rut, 
        int edad, string sexo, string faena, string cargo, 
        int antiguedadCargo, int antiguedadEmpresa, string empresaMandante, 
        string empresaSubcontratista, string region, string pais, string idFormulario)
    {
        this.id = id;
        this.encuestado = encuestado;
        this.nombre = nombre;
        this.apellido = apellido;
        this.rut = rut;
        this.edad = edad;
        this.sexo = sexo;
        this.faena = faena;
        this.cargo = cargo;
        this.antiguedadCargo = antiguedadCargo;
        this.antiguedadEmpresa = antiguedadEmpresa;
        this.empresaMandante = empresaMandante;
        this.empresaSubcontratista = empresaSubcontratista;
        this.region = region;
        this.pais = pais;
        this.id_formulario = idFormulario;
    }
}
[Serializable]
public struct SurveyIdentifier
{
    public string identificador;
    public string id_formulario;

    public SurveyIdentifier(string identificador, string idFormulario)
    {
        this.identificador = identificador;
        this.id_formulario = idFormulario;
    }
}