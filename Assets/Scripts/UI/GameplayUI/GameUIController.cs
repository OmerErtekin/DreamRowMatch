using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    private IEnumerator Start()
    {
        IngameLoadingScreen.Instance.ShowLoadingScreen(false);
        yield return new WaitForFixedUpdate();
        IngameLoadingScreen.Instance.HideLoadingScreen(true);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.G))
        {
            ReturnToMainMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        PlayerPrefs.SetInt("DidComeFromGame", 1);
        IngameLoadingScreen.Instance.ReturnMainLevel();
    }

}
