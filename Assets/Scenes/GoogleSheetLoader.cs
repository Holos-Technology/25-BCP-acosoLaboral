using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class GoogleSheetLoader : MonoBehaviour
{
   
    private string csvUrlCorporativo = "https://docs.google.com/spreadsheets/d/15z3j9eaORZJe3gInmMBsaJwD_cCgr_9iQwG5a1v3Nxs/export?format=csv&gid=551139810";
    private string csvUrlFaena = "https://docs.google.com/spreadsheets/d/1J9EgHbviARZa9S7-2qKtbdNE6DkNUDf-Q3cb5gkzTWM/export?format=csv&gid=551139810";
    private string csvFaenaF = "https://docs.google.com/spreadsheets/d/1pDSXNBGnMIOQhDwUuiRhz8opRctRtpeLMCZGCsdzcWg/export?format=csv&gid=434018312";
    private string csvFaenaM = "https://docs.google.com/spreadsheets/d/1pDSXNBGnMIOQhDwUuiRhz8opRctRtpeLMCZGCsdzcWg/export?format=csv&gid=1885543387";
    private string csvCorpM = "https://docs.google.com/spreadsheets/d/1I82j0B3RNkHa-a6gZkMPNXd8PDD738gIHQKbOkDmZHo/export?format=csv&gid=1020775372";
    private string csvCorpF = "https://docs.google.com/spreadsheets/d/1I82j0B3RNkHa-a6gZkMPNXd8PDD738gIHQKbOkDmZHo/export?format=csv&gid=1020775372";
    public delegate void JsonLoadedEvent();
    public static event JsonLoadedEvent OnJsonLoaded;
   void Start()
    {
        StartCoroutine(DownloadAllCSVs());
    }

    IEnumerator DownloadAllCSVs()
    {
        yield return StartCoroutine(DownloadCSVAndConvertToJson(csvUrlCorporativo, "dataCorporativo.json", false));
        yield return StartCoroutine(DownloadCSVAndConvertToJson(csvUrlFaena, "dataFaena.json", false));

        if (!string.IsNullOrEmpty(csvFaenaF)) yield return StartCoroutine(DownloadCSVAndConvertToJson(csvFaenaF, "faena_f.json", true));
        if (!string.IsNullOrEmpty(csvFaenaM)) yield return StartCoroutine(DownloadCSVAndConvertToJson(csvFaenaM, "faena_m.json", true));
        if (!string.IsNullOrEmpty(csvCorpM)) yield return StartCoroutine(DownloadCSVAndConvertToJson(csvCorpM, "corp_m.json", true));
        if (!string.IsNullOrEmpty(csvCorpF)) yield return StartCoroutine(DownloadCSVAndConvertToJson(csvCorpF, "corp_f.json", true));

        Debug.Log("📢 JSON generado, notificando...");
        OnJsonLoaded?.Invoke(); // ✅ Asegura que siempre se llame
    }
    IEnumerator DownloadCSVAndConvertToJson(string url, string fileName, bool extractIdentifiers)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 10; // segundos máximos para esperar

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string csvData = request.downloadHandler.text;
                string jsonData = extractIdentifiers ? ExtractIdentifiersFromCsv(csvData) : ConvertCsvToJson(csvData);
                FileManager.SaveJson(jsonData, fileName);
            }
            else
            {
                Debug.LogError($"❌ Error al obtener {fileName}: {request.error}");
                Debug.LogWarning($"⚠️ Creando {fileName} con valores vacíos...");
                FileManager.SaveJson("[]", fileName);
            }

            // ⚠️ Importante: avanzar siempre
            yield return null;
        }
    }

    string ConvertCsvToJson(string csvText)
    {
        csvText = csvText.Replace("\r\n", "\n").Trim();
        string[] rows = csvText.Split('\n');
        if (rows.Length < 2) return "[]";

        string[] headers = ParseCsvLine(rows[0]);
        List<Dictionary<string, string>> jsonList = new List<Dictionary<string, string>>();

        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rows[i])) continue;

            string[] columns = ParseCsvLine(rows[i]);
            if (columns.Length != headers.Length) continue;

            Dictionary<string, string> rowDict = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length; j++)
            {
                rowDict[headers[j].Trim()] = columns[j].Trim();
            }
            jsonList.Add(rowDict);
        }

        return JsonConvert.SerializeObject(jsonList, Formatting.Indented);
    }

    string ExtractIdentifiersFromCsv(string csvText)
    {
        csvText = csvText.Replace("\r\n", "\n").Trim();
        string[] rows = csvText.Split('\n');
        if (rows.Length < 2) return "[]";

        string[] headers = ParseCsvLine(rows[0]);
        List<Dictionary<string, string>> jsonList = new List<Dictionary<string, string>>();

        // Buscar índices de "Seleccione Usuario" e "id_formulario"
        int userIndex = System.Array.IndexOf(headers, "Seleccione Usuario");
        int formIndex = System.Array.IndexOf(headers, "id_formulario");

        if (userIndex == -1 || formIndex == -1)
        {
            Debug.LogError("❌ No se encontraron las columnas 'Seleccione Usuario' o 'id_formulario' en el CSV.");
            return "[]";
        }

        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rows[i])) continue;

            string[] columns = ParseCsvLine(rows[i]);
            if (columns.Length <= userIndex || columns.Length <= formIndex) continue;

            string fullUser = columns[userIndex].Trim();
            string idFormulario = columns[formIndex].Trim();

            // 🔹 Extraer solo el número del usuario usando una expresión regular
            string userId = ExtractUserId(fullUser);

            Dictionary<string, string> rowDict = new Dictionary<string, string>
            {
                { "identificador", userId },
                { "id_formulario", idFormulario }
            };

            jsonList.Add(rowDict);
        }

        return JsonConvert.SerializeObject(jsonList, Formatting.Indented);
    }
    string ExtractUserId(string fullUser)
    {
        if (string.IsNullOrEmpty(fullUser)) return "N/A";

        // 🔹 Buscar el número después de ':'
        string[] parts = fullUser.Split(':');
        if (parts.Length > 1)
        {
            string extracted = parts[1].Trim();
        
            // 🔹 Remover caracteres que NO sean números o guiones
            extracted = Regex.Replace(extracted, @"[^0-9\-]", "");
        
            return extracted;
        }

        // 🔹 Si no hay ":", extraer solo los números y el guion si existe
        return Regex.Replace(fullUser, @"[^0-9\-]", "");
    }


    string[] ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        string pattern = "(?:\"([^\"]*)\")|([^,]*)";

        foreach (Match match in Regex.Matches(line, pattern))
        {
            result.Add(match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);
        }

        return result.ToArray();
    }
}
