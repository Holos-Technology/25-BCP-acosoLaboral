using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "LikertStep", menuName = "SceneSteps/Likert")]
public class LikertStep : ScriptPartSO
{
    [TextArea]
    public string statement;
    public bool isMultiAnswer;
    public List<FeelingEntry> feelings;
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(name) || name.StartsWith("New ")) 
        {
            stepType = StepType.Likert;
        }
    }
    
    
}

[Serializable]
public class FeelingEntry
{
    public string name;
    public Sprite sprite;
}