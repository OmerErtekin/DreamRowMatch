using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class LevelDownloader : MonoBehaviour
{
    public List<string> urls;
    public string saveDirectory;

    IEnumerator Start()
    {
        if(PlayerPrefs.GetInt("IsDownloadedLevels",0) == 1 || urls.Count == 0)
            yield break;

        UnityWebRequest www = UnityWebRequest.Get(urls[0]);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error On Internet Connection. Will not be downloaded!");
            yield break;
        }

        StartCoroutine(DownloadAndSaveLevels());
    }

    private IEnumerator DownloadAndSaveLevels()
    {
        UnityWebRequest www = UnityWebRequest.Get(urls[0]);

        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        foreach (string url in urls)
        {
            Debug.Log("Downloading file: " + url);

            www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading file on : " + url + " ERROR : " + www.error);
                yield break;
            }

            string fileName = Path.GetFileName(url);
            string filePath = Path.Combine(saveDirectory, fileName);

            File.WriteAllText(filePath, www.downloadHandler.text);

            Debug.Log("File downloaded and saved: " + filePath);
        }

        PlayerPrefs.SetInt("IsDownloadedLevels", 1);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}