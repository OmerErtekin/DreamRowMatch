using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    #region Components
    public static GameUIController Instance;
    [SerializeField] private TMP_Text moveText, highScoreText,currentScoreText;
    [SerializeField] private LevelEndScreen levelEndScreen;
    #endregion

    #region Variables
    private LevelData currentLevelData;
    private int highScore,currentScore;
    private bool didBeatHighScore = false;
    private bool isGameFinished = false;
    public bool IsGameFinished => isGameFinished;
    public int CurrentScore => currentScore;
    public int HighScore => highScore;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        IngameLoadingScreen.Instance.HideLoadingScreen(true);
        GridCreator.OnGridCreated += InitializeGameUI;
    }

    private void InitializeGameUI(LevelData levelData)
    {
        currentLevelData = levelData;
        moveText.text = currentLevelData.moveCount.ToString();
        highScore = PlayerPrefs.GetInt($"HighScore_Level{currentLevelData.levelNumber}");
        currentScoreText.text = "0";
        highScoreText.text = highScore.ToString();
    }

    public void UpdateMoveText(int moveCount)
    {
        UpdateTextWithAnim(moveText, moveCount.ToString());
    }

    public void UpdateCurrentScore(int additionalScore)
    {
        currentScore += additionalScore;
        UpdateTextWithAnim(currentScoreText,currentScore.ToString());
        if(currentScore > highScore)
        {
            didBeatHighScore = true;
            highScore = currentScore;
            UpdateHighScore(highScore);
        }
    }

    public void UpdateHighScore(int score)
    {
        UpdateTextWithAnim(highScoreText, score.ToString());
    }

    private void UpdateTextWithAnim(TMP_Text textRef,string text)
    {
        textRef.DOKill();
        textRef.transform.DOScale(1, 0.25f).From(0.75f).SetEase(Ease.OutBack);
        textRef.text = text;
    }

    public void FinishTheGame(GameEndType type)
    {
        if(isGameFinished) return;

        isGameFinished = true;
        levelEndScreen.ShowLevelEndScreen(type);
    }

    public void ReturnToMainMenu()
    {
        if(didBeatHighScore)
            PlayerPrefs.SetInt($"HighScore_Level{currentLevelData.levelNumber}",highScore);

        PlayerPrefs.SetInt("HasNewHighScore",didBeatHighScore ? 1 : 0);

        IngameLoadingScreen.Instance.LoadMenuScene();
    }
}
