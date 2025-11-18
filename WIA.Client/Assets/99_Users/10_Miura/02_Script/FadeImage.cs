using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Player;

public class FadeImage : MonoBehaviour
{
    CameraManager cameraManager;
    void Start()
    {
        this.GetComponent<Image>().color = new Color(0,0,0,0);
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
    }

    /// <summary>
    /// フェードアウト処理
    /// </summary>
    public void FadeOut()
    {
        this.GetComponent<Image>().DOFade(1f,1f);
    }

    /// <summary>
    /// フェードイン処理
    /// </summary>
    public void FadeIn()
    {
        this.GetComponent<Image>().DOFade(0f, 3f);

        cameraManager.TurnOnPlayerCam();
    }
}