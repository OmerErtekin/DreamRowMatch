using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class LevelDownloader : MonoBehaviour
{
    #region Components
    [SerializeField] private UrlData urlData;
    [SerializeField] private Image fillbarImage;
    #endregion

    IEnumerator Start()
    {
        if(PlayerPrefs.GetInt("IsDownloadedLevels",0) == 1 || urlData.urls.Count == 0)
        {
            LoadLevelWithFillBar();
            yield break;
        }

        UnityWebRequest www = UnityWebRequest.Get(urlData.urls[0]);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error On Internet Connection. Will not be downloaded!");
            LoadLevelWithFillBar();
            yield break;
        }

        StartCoroutine(DownloadAndSaveLevels());
    }

    private IEnumerator DownloadAndSaveLevels()
    {
        fillbarImage.DOFillAmount(0.5f, 0.5f).SetTarget(this);

        UnityWebRequest www = UnityWebRequest.Get(urlData.urls[0]);

        if (!Directory.Exists(urlData.filePath))
        {
            Directory.CreateDirectory(urlData.filePath);
        }

        foreach (string url in urlData.urls)
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
            string filePath = Path.Combine(urlData.filePath, fileName);

            File.WriteAllText(filePath, www.downloadHandler.text);

            Debug.Log("File downloaded and saved: " + filePath);
        }

        PlayerPrefs.SetInt("IsDownloadedLevels", 1);
        LoadLevelWithFillBar();
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    private void LoadLevelWithFillBar()
    {
        fillbarImage.DOKill();
        fillbarImage.DOFillAmount(1, 1f).OnComplete(()=> SceneManager.LoadScene(1)).SetTarget(this);
    }
}