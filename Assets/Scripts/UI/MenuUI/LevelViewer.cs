using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelViewer : MonoBehaviour
{
    #region Components
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Scrollbar scrollBar;
    private LevelReader levelReader;
    #endregion
    #region Variables
    [SerializeField] private Transform contentTransform;
    [SerializeField] private GameObject levelBarPrefab;
    [SerializeField] private int visibleBarCount = 5;
    private List<LevelBar> levelBars = new();
    #endregion

    private void Start()
    {
        levelReader = GetComponent<LevelReader>();
        GenerateLevelBars();
    }
    public void ShowLevelViewer()
    {
        gameObject.SetActive(true);
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, 0.5f).SetTarget(this).From(0);
    }

    public void HideLevelViewer()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, 0.5f).SetTarget(this).OnComplete(() => gameObject.SetActive(false));
    }

    private void GenerateLevelBars()
    {
        for (int i = 0; i < levelReader.Levels.Count; i++)
        {
            levelBars.Add(Instantiate(levelBarPrefab, contentTransform).GetComponent<LevelBar>());
            levelBars[i].InitializeLevelBar(levelReader.Levels[i]);
        }

        SetScrollBarPosForIndex(PlayerPrefs.GetInt("SelectedLevel", 0));
    }

    private void SetScrollBarPosForIndex(int index)
    {
        if (index < 4)
            scrollBar.value = 1;
        if (index > levelReader.Levels.Count - visibleBarCount)
            scrollBar.value = 0;

        scrollBar.value = 1 - ((float)(index - 1) / (levelReader.Levels.Count - visibleBarCount + 1 - 1 / visibleBarCount));
    }
}
