using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUrlData", menuName = "ScriptableObjects/UrlData", order = 1)]
public class UrlData : ScriptableObject
{
    public string filePath;
    public List<string> urls = new List<string>();
}
