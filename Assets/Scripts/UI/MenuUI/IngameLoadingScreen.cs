using DG.Tweening;
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
    public bool DidComeFromGameplay { get; private set; } = false;
    #endregion

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    public void LoadGameplayLevel()
    {
        DidComeFromGameplay = false;
        sceneIndex = 2;
        ShowLoadingScreen(true);
        Invoke(nameof(LoadLevel), 2);
    }

    public void LoadMenuScene()
    {
        DidComeFromGameplay = true;
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
