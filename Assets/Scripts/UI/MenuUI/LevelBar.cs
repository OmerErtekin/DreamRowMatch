using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelBar : MonoBehaviour
{
    #region Components
    [SerializeField] private TMP_Text levelAndMoveText, highScoreText;
    [SerializeField] private CanvasGroup playGroup, lockedGroup;
    #endregion
    #region Variables
    private LevelData levelData;
    #endregion

    public void InitializeLevelBar(LevelData data)
    {
        levelData = data;
        levelAndMoveText.text = $"Level {levelData.levelNumber} - {levelData.moveCount} Moves";

        bool canPlay = PlayerPrefs.GetInt("MaxLevel", 1) >= levelData.levelNumber;
        playGroup.gameObject.SetActive(canPlay);
        lockedGroup.gameObject.SetActive(!canPlay);

        if (PlayerPrefs.HasKey($"HighScore_Level{levelData.levelNumber}"))
        {
            highScoreText.text = $"Highest Score {PlayerPrefs.GetInt($"HighScore_Level{levelData.levelNumber}")}";
        }
        else
        {
            highScoreText.text = "No Score";
        }
    }

    public void UnlockLevel()
    {
        playGroup.alpha = 0;
        playGroup.gameObject.SetActive(true);
        lockedGroup.DOFade(0, 1f).SetTarget(this).SetUpdate(true).OnComplete(()=> lockedGroup.gameObject.SetActive(false));
        playGroup.DOFade(1, 1f).SetTarget(this).SetUpdate(true);
        PlayerPrefs.SetInt("MaxLevel", levelData.levelNumber);
    }

    public void OnPlayButtonClicked()
    {
        PlayerPrefs.SetInt("SelectedLevel", levelData.levelNumber);
        IngameLoadingScreen.Instance.LoadGameplayLevel();
    }
}
