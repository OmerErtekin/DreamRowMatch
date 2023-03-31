using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelViewer : MonoBehaviour
{
    #region Components
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private LevelReader levelReader;
    #endregion
    #region Variables
    [SerializeField] private Transform contentTransform;
    [SerializeField] private GameObject levelBarPrefab;
    [SerializeField] private int visibleBarCount = 5;
    private List<LevelBar> levelBars = new();
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            TryUnlockNextLevel();
    }

    public void ShowLevelViewer(bool willUnlockNextLevel = false)
    {
        gameObject.SetActive(true);
        if (levelBars.Count == 0)
        {
            GenerateLevelBars();
        }
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, 0.5f).SetTarget(this).From(0);
        if (willUnlockNextLevel)
        {
            Invoke(nameof(TryUnlockNextLevel), 1);
        }
        StartCoroutine(SetScrollBarPosForIndex(PlayerPrefs.GetInt("SelectedLevel", 1)));
    }

    public void HideLevelViewer()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, 0.5f).SetTarget(this).OnComplete(() => gameObject.SetActive(false));
    }

    public void TryUnlockNextLevel()
    {
        if (PlayerPrefs.GetInt("MaxLevel", 1) > PlayerPrefs.GetInt("SelectedLevel", 1) || PlayerPrefs.GetInt("MaxLevel", 1) >= levelBars.Count)
        {
            return;
        }
        StartCoroutine(UnlockRoutine());
    }


    private void GenerateLevelBars()
    {
        for (int i = 0; i < levelReader.Levels.Count; i++)
        {
            levelBars.Add(Instantiate(levelBarPrefab, contentTransform).GetComponent<LevelBar>());
            levelBars[i].InitializeLevelBar(levelReader.Levels[i]);
        }
        StartCoroutine(SetScrollBarPosForIndex(PlayerPrefs.GetInt("SelectedLevel", 1)));
    }

    private IEnumerator SetScrollBarPosForIndex(int index, float tweenDuration = 0)
    {
        yield return new WaitForFixedUpdate();
        if (index < 4)
            scrollBar.value = 1;
        if (index > levelReader.Levels.Count - visibleBarCount)
            scrollBar.value = 0;

        var value = 1 - ((float)(index - 1) / (levelReader.Levels.Count - visibleBarCount + 1 - 1 / visibleBarCount));
        if (tweenDuration == 0)
            scrollBar.value = value;
        else
        {
            var currentValue = scrollBar.value;
            DOTween.To(() => currentValue, x => currentValue = x, value, tweenDuration)
               .OnUpdate(() =>
               {
                   scrollBar.value = currentValue;
               });
        }
    }

    private IEnumerator UnlockRoutine()
    {
        scrollRect.vertical = false;
        int nextIndex = PlayerPrefs.GetInt("MaxLevel", 1);
        StartCoroutine(SetScrollBarPosForIndex(nextIndex + 1, 0.5f));

        yield return new WaitForSeconds(0.5f);
        levelBars[nextIndex].UnlockLevel();

        yield return new WaitForSeconds(1);
        scrollRect.vertical = true;
    }
}
