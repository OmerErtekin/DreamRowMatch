using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Celebration : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<Transform> objectsToScale;
    #endregion
    #region Components
    #endregion
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            CelebrateHighestScore();
        if (Input.GetKeyDown(KeyCode.S))
            HideCelebration();
    }

    public void CelebrateHighestScore()
    {
        gameObject.SetActive(true);
        StartCoroutine(ScaleUpRoutine());
    }

    private void HideCelebration()
    {
        StartCoroutine(ScaleDownRoutine());
    }

    private IEnumerator ScaleUpRoutine()
    {
        for(int i = 0;i < objectsToScale.Count; i++)
        {
            objectsToScale[i].DOScale(1, 1f).From(0).SetTarget(this).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(3.5f);
        HideCelebration();
    }

    private IEnumerator ScaleDownRoutine()
    {
        for (int i = objectsToScale.Count -1; i >= 0; i--)
        {
            objectsToScale[i].DOScale(0, 1f).From(1).SetTarget(this).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1.25f);
        MenuUIController.Instance.ShowLevelViewer(true);
        gameObject.SetActive(false);
    }
}
