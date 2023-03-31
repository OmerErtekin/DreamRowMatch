using System.Collections;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    #region Components
    public static MenuUIController Instance;
    [SerializeField] private LevelViewer levelViewer;
    #endregion
    #region Variables
    #endregion
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("DidComeFromGame", 0) == 1)
        {
            IngameLoadingScreen.Instance.HideLoadingScreen(true);
            StartCoroutine(CameFromMenuRoutine());
        }
        else
        {
            IngameLoadingScreen.Instance.HideLoadingScreen(false);
        }
        PlayerPrefs.SetInt("DidComeFromGame", 0);
    }

    private IEnumerator CameFromMenuRoutine()
    {
        yield return new WaitForSeconds(1);
        if (PlayerPrefs.GetInt("HasNewHighScore", 0) == 1)
        {

        }
        else
        {
            ShowLevelViewer();
        }
    }

    public void ShowLevelViewer()
    {
        levelViewer.ShowLevelViewer();
    }
}
