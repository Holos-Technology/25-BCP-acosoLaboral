using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Country set", menuName = "Acoso laboral/Country set")]
public class SceneScriptable : ScriptableObject
{
    public string country;
    public string scenario;
    public string sceneName;
    public List<ScriptPartSO> steps;
}