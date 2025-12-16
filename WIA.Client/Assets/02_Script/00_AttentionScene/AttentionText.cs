using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;  //DOTweenを使うときはこのusingを入れる

/// <summary>
/// ゲーム起動時表示テキストクラス
/// </summary>
public class AttentionText : MonoBehaviour
{
    [SerializeField] GameObject text;           //テキスト
    [SerializeField] GameObject image;    //プロダクト画像

    [SerializeField] float startTime;     //描画開始時間

    [SerializeField] GameObject nextLogo; //次に表示するロゴ

    private Text textColor;     //色変更用(テキスト)
    private Image imgColor;     //色変更用(画像)

    private float finishTime = 3.0f;     //表示時間
    private float timer;                 //繰り返す間隔

    [Header("スタートカラー")]
    [SerializeField]
    Color32 startColor = new Color32(29, 29, 29, 0);

    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);

    void Start()
    {
        if (text)
        {
            textColor = text.GetComponent<Text>();
            textColor.color = startColor;
        }
        if (image)
        {
            imgColor = image.GetComponent<Image>();
            imgColor.color = startColor;
        }

        timer = 0.0f;
    }

    void Update()
    {
        timer += Time.deltaTime;     //時間をカウントする

        if (Input.GetMouseButtonDown(0))
        {
            if (timer >= 1.0f) timer += 10f;
        }

        if (timer >= startTime)
        {
            if (text) textColor.color = Color.Lerp(textColor.color, new Color(1, 1, 1, 1), 2.0f * Time.deltaTime);      //テキスト
            if (imgColor) imgColor.color = Color.Lerp(imgColor.color, new Color(1, 1, 1, 1), 2.0f * Time.deltaTime);     //プロダクト画像

            if (timer >= startTime + finishTime)
            {
                if (text) textColor.color = Color.Lerp(textColor.color, new Color(0, 0, 0, -3), 2.0f * Time.deltaTime);
                if (imgColor) imgColor.color = Color.Lerp(imgColor.color, new Color(0, 0, 0, -3), 2.0f * Time.deltaTime);

                Invoke("StartTitleScene", 0.3f);
            }
        }
    }

    public void StartTitleScene()
    {
        // シーン遷移
        if (nextLogo != null) nextLogo.SetActive(true);

        if (nextLogo == null)
        {
            Initiate.DoneFading();

            //VRかどうかでシーン選別
            if(UnityEngine.XR.XRSettings.isDeviceActive) Initiate.Fade("VR_01_TitleScene",endColor,2.0f);
            else Initiate.Fade("01_TitleScene", endColor, 2.0f);

        }
    }
}
