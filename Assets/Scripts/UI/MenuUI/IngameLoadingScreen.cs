using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameLoadingScreen : MonoBehaviour
{
    #region Components
    public static IngameLoadingScreen Instance;
    [SerializeField] private CanvasGroup canvasGroup;
    #endregion
    #region Variables
    private int sceneIndex;
    #endregion

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void LoadGameplayLevel()
    {
        sceneIndex = 2;
        ShowLoadingScreen(true);
        Invoke(nameof(LoadLevel), 2);
    }

    public void ReturnMainLevel()
    {
        sceneIndex = 1;
        ShowLoadingScreen(true);
        Invoke(nameof(LoadLevel), 2);
    }

    public void ShowLoadingScreen(bool willFade)
    {
        gameObject.SetActive(true);
        if (willFade)
        {
            canvasGroup.DOFade(1, 1f).SetTarget(this).From(0);
        }
        else
        {
            canvasGroup.alpha = 1;
        }
    }

    public void HideLoadingScreen(bool willFade)
    {
        if (willFade)
        {
            gameObject.SetActive(true);
            canvasGroup.DOFade(0, 1f).SetTarget(this).From(1).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
