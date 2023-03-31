using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InnerButtonImage : MonoBehaviour
{
    [SerializeField] private Button buttonToSucsribe;
    [SerializeField] private Vector3 clickedScale;
    private Vector3 normalScale;


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
