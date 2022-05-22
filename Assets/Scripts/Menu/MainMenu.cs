using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    bool onStart;
    public void OnStartButton()
    {
        if (onStart)
        {
            return;
        }
        onStart = true;
        GameDataManager.Instance.IsOnlineBattle = false;
        FadeManager.Instance.LoadScene("Game", 1f);
    }

    public void OnOnlineStartButton()
    {
        if (onStart)
        {
            return;
        }
        onStart = true;
        GameDataManager.Instance.IsOnlineBattle = true;
        FadeManager.Instance.LoadScene("Online", 1f);
    }
}
