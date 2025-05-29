using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Country set", menuName = "Acoso laboral/Scripter Set")]
public class SceneScriptable : ScriptableObject
{
    public string country;
    public string scenario;
    public string sceneName;
    public string language;
    public List<ScriptPartSO> steps;
}