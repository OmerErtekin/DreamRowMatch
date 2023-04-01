using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InnerButtonImage : MonoBehaviour
{
    #region Variables    
    [SerializeField] private Vector3 clickedScale;
    private Vector3 normalScale;
    #endregion

    #region Components
    [SerializeField] private Button buttonToSucsribe;
    #endregion

    private void Start()
    {
        normalScale = transform.localScale;
        buttonToSucsribe.onClick.AddListener(ScaleTheButton);  
    }

    private void ScaleTheButton()
    {
        transform.DOKill();
        transform.DOScale(normalScale, 0.75f).From(clickedScale).SetTarget(this).SetUpdate(true);
    }
}
