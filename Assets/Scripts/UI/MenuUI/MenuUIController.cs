using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    #region Components
    public static MenuUIController Instance;
    [SerializeField] private CanvasGroup levelsButton;
    [SerializeField] private LevelViewer levelViewer;
    [SerializeField] private Celebration celebration;
    #endregion

    #region Variables
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (IngameLoadingScreen.Instance.DidComeFromGameplay)
        {
            IngameLoadingScreen.Instance.HideLoadingScreen(true);
            StartCoroutine(CameFromMenuRoutine());
        }
    }

    private IEnumerator CameFromMenuRoutine()
    {
        levelsButton.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("HasNewHighScore", 0) == 1)
        {
            yield return new WaitForSeconds(1);
            celebration.CelebrateHighestScore();
        }
        else
        {
            yield return new WaitForSeconds(1);
            ShowLevelViewer(false);
        }
    }

    public void ShowLevelViewer(bool willUnlockNextLevel)
    {
        levelsButton.DOFade(0, 0.5f).SetTarget(this);
        levelViewer.ShowLevelViewer(willUnlockNextLevel);
    }

    public void HideLevelViewer()
    {
        levelsButton.gameObject.SetActive(true);
        levelsButton.DOFade(1, 0.5f).SetTarget(this);
        levelViewer.HideLevelViewer();
    }
}
