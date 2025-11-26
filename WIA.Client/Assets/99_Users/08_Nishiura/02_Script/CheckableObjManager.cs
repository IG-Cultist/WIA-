/// ------------------------------
/// 調査可能オブジェクトマネージャー
/// Author:Nishiura Date:25/11/18
/// ------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckableObjManager : MonoBehaviour
{
    // 調査可能オブジェクトリスト
    [SerializeField] List<GameObject> checkableObjList;
    [SerializeField] GameObject checkNowText;

    [Header("調査プレイヤー用")]
    FirstPersonMovement playerMove;    //座標固定用
    FirstPersonLook playerCamera;      //視点固定用

    // キー内蔵オブジェクト番号
    int keyObjectNum;

    [SerializeField] public float chackableTimer;    //調査所要時間
    private bool isCheckNow;     //現在調査中か

    GameObject nowFindObj;       //現在調査中のオブジェクト
    FindKeyStatus nowFindStatus; //現在調査中のオブジェクトのステータス


    void Start()
    {
        //メインキャラクターのカメラを取る
        playerCamera = GameObject.Find("Main").transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();
        //プレイヤー移動処理スクリプト取得
        playerMove = GameObject.Find("Main").GetComponent<FirstPersonMovement>();

        // 0からリストの長さ分までの乱数を設定
        keyObjectNum = Random.Range(0, checkableObjList.Count);
        Debug.Log("鍵が隠されているのは" + checkableObjList[keyObjectNum].name);

        checkNowText.SetActive(false);
    }

    private void Update()
    {
        //nullチェック
        if (nowFindObj == null) return;

        //オブジェクト調査中
        if (isCheckNow)
        {
            if (nowFindStatus.isChecked) return;  //チェック済みはreturn

            checkNowText.SetActive(true);   //調査テキスト表示
            playerCamera.enabled = false;   //カメラアングル固定化
            playerMove.enabled = false;     //プレイヤー座標固定化

            nowFindStatus.checkedTime += Time.deltaTime;   //調査時間加算

        }
        else if (!isCheckNow)
        {
            checkNowText.SetActive(false); //調査テキスト非表示
            playerCamera.enabled = true;   //アングル固定解除
            playerMove.enabled = true;     //座標固定解除
        }

        //調査オブジェクトの調査時間が所要時間を超えたら
        if (chackableTimer <= nowFindStatus.checkedTime)
        {
            if(nowFindObj.name == "Door")
            {
                Initiate.Fade("Stage_3", Color.black, 1.0f);
            }

            // 調べたオブジェクトにキーが入っていた場合
            if (nowFindObj == checkableObjList[keyObjectNum])
            {
                Debug.Log("鍵を見つけた");
                //鍵獲得の処理を記述

            }
            else
            {
                Debug.Log("鍵はなかった");
            }

            isCheckNow = false;   //調査完了したら強制的に解除
            nowFindStatus.isChecked = true;  //調査済みに変更
        }

    }

    /// <summary>
    /// オブジェクト調査処理
    /// </summary>
    /// <param name="obj"></param>
    public void CheckInObject(GameObject obj , FindKeyStatus status)
    {
        nowFindObj = obj;        //調査中オブジェクト
        nowFindStatus = status;  //調査中オブジェクトステータス

        if (status.isChecked) Debug.Log("調査済み");

        isCheckNow = true;
    }

    /// <summary>
    /// 調査解除関数
    /// </summary>
    public void CheckOutObject()
    {
        isCheckNow = false;

        if (nowFindStatus == null) return;

    }
}
