using System.Collections;
using System.Collections.Generic;
using Tymski;
using UnityEngine;


[CreateAssetMenu(fileName = "Scene", menuName = "ScriptableObjects/Scenes", order = 1)]
public class Scenes : ScriptableObject
{
    public Level[] csharpLevels;
    public Level[] javaLevels;
}


[System.Serializable]
public class Level
{
    public string levelName;
    public SceneReference[] scenes;
}
