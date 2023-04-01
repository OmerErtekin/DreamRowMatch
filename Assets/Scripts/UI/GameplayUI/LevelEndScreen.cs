using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class LevelEndScreen : MonoBehaviour
{
    #region Components
    [SerializeField] private CanvasGroup endCanvasGroup,homeButtonCanvasGroup;
    [SerializeField] private TMP_Text currentScoreText, highScoreText;
    [SerializeField] private Animator endScreenAnimator;
    #endregion

    #region Variables
    [SerializeField] private RectTransform noMoreMatchParent, outOfMoveParent;
    #endregion

    public void ShowLevelEndScreen(GameEndType endType)
    {
        gameObject.SetActive(true);
        StartCoroutine(LevelEndScreenRoutine(endType));
    }

    private IEnumerator LevelEndScreenRoutine(GameEndType endType)
    {
        //Activate gameobject to block raycasts. Then wait a little and show the screen
        yield return new WaitForSeconds(0.5f);

        endCanvasGroup.DOFade(1, 0.5f).From(0).SetTarget(this);
        yield return new WaitForSeconds(0.5f);

        noMoreMatchParent.gameObject.SetActive(endType == GameEndType.NoMoreMatch);
        outOfMoveParent.gameObject.SetActive(endType == GameEndType.OutOfMove);
        endScreenAnimator.enabled = true;
        yield return new WaitForSeconds(1f);

        highScoreText.text = $"High Score : {GameUIController.Instance.HighScore}";
        currentScoreText.text = $"Current Score : {GameUIController.Instance.CurrentScore}";
        highScoreText.DOFade(1, 0.5f).From(0).SetTarget(this);
        currentScoreText.DOFade(1,0.5f).From(0).SetTarget(this);
        homeButtonCanvasGroup.DOFade(1, 0.5f).From(0).SetTarget(this);
    }
}
