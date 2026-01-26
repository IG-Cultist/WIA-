/// ------------------------------
/// テキストマネージャー
/// Author:Nishiura Date:25/11/26
/// ------------------------------
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    // 説明文リスト
    [SerializeField] List<Text> textList;

    // クリック助長テキスト
    [SerializeField] Text clickToNext;

    int cnt;

    bool isFinish = false;

    // 現在のシーン名
    string[] nowSceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 現在のシーン名を_で分割する
        nowSceneName = SceneManager.GetActiveScene().name.Split("_");
        // 文字生成を開始
        InvokeRepeating("SpawnMessage", 1f, 2f);

        if (RoomModel.Instance)
        {//通信用
            RoomModel.Instance.OnWaitSyn += this.OnWaitSyn;
            RoomModel.Instance.OnSameStarted += this.OnSameStarted;
        }
        else
        {

        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isFinish)
        {
            if (RoomModel.Instance)
            {
                Ready();
            }
            else
            {
                Initiate.DoneFading();
                Initiate.Fade("Stage_" + nowSceneName[2] + "", Color.black, 1.0f);   // フェード時間1秒
            }
        }
        if (Input.GetMouseButtonDown(0) && !isFinish)
        {
            CancelInvoke();
            isFinish = true;

            clickToNext.GetComponent<Text>().color = new Color(0f, 0f, 0f, 1f);

            foreach (Text text in textList)
            {
                text.GetComponent<Text>().color = new Color(0f, 0f, 0f, 1f);
            }
        }
    }

    private void OnDisable()
    {
        RoomModel.Instance.OnWaitSyn -= this.OnWaitSyn;
        RoomModel.Instance.OnSameStarted -= this.OnSameStarted;
    }

    /// <summary>
    /// テキスト表示処理
    /// </summary>
    void SpawnMessage()
    {
        if (isFinish)
        {
            CancelInvoke();
            clickToNext.GetComponent<Text>().DOFade(1f, 1f);
        }
        else
        {
            textList[cnt].GetComponent<Text>().DOFade(1f, 1f);
            cnt++;

            // 次の説明文がない場合、ループを終了する
            if (textList.Count <= cnt) isFinish = true;
        }
    }

    //////////////
    /// 通信用 ///
    //////////////

    /// <summary>
    /// オンラインで開始の準備をする
    /// </summary>
    async void Ready()
    {
        await RoomModel.Instance.WaitAsync();
    }

    void OnWaitSyn()
    {
        Debug.Log("プレイヤーを待っています…");
    }

    void OnSameStarted()
    {
        Initiate.DoneFading();
        if(UnityEngine.XR.XRSettings.isDeviceActive)
        {//VRの場合
            Initiate.Fade("Stage_" + nowSceneName[3], Color.black, 1.0f);   // フェード時間1秒
        }
        else
        {//VRでないとき
            Initiate.Fade("Stage_" + nowSceneName[3] + "", Color.black, 1.0f);   // フェード時間1秒
        }
    }
}
