/// ------------------------------
/// 調査可能オブジェクトマネージャー
/// Author:Nishiura Date:25/11/18
/// ------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using KanKikuchi.AudioManager;
using static Unity.Burst.Intrinsics.X86;
using UnityEngine.ProBuilder.MeshOperations;

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

    public bool isGetKey;      //キーを取得できたか

    public GameObject nowFindObj;       //現在調査中のオブジェクト
    FindKeyStatus nowFindStatus; //現在調査中のオブジェクトのステータス

    // プレイヤー
    private Player player;
    //死亡判定
    private bool isDead;

    private bool playSE;
    private bool searchingSE;
    private bool searchedSE;
    private bool openDoor;
    //VR
    private XRControllerButtonEvents xrControllerButtonEvents;


    void Start()
    {
        if(RoomModel.Instance)
        {
            //メインキャラクターのカメラを取る
            playerCamera = GameObject.Find(OnlineGameManager.Player.name).transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();
            //プレイヤー移動処理スクリプト取得
            playerMove = GameObject.Find(OnlineGameManager.Player.name).GetComponent<FirstPersonMovement>();
            //VRのスクリプト取得
            xrControllerButtonEvents = GameObject.Find(OnlineGameManager.Player.name).GetComponent<XRControllerButtonEvents>();
        }
        else
        {
            //メインキャラクターのカメラを取る
            playerCamera = GameObject.Find("Main").transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();
            //プレイヤー移動処理スクリプト取得
            playerMove = GameObject.Find("Main").GetComponent<FirstPersonMovement>();
            //VRのスクリプト取得
            xrControllerButtonEvents = GameObject.Find("Main").GetComponent<XRControllerButtonEvents>();
        }

        // 0からリストの長さ分までの乱数を設定
        keyObjectNum = Random.Range(0, checkableObjList.Count);
        Debug.Log("鍵が隠されているのは" + checkableObjList[keyObjectNum].name);

        checkNowText.SetActive(false);

        if(RoomModel.Instance) player = GameObject.Find(OnlineGameManager.Player.name).gameObject.GetComponent<Player>();
        else player = GameObject.Find("Main").gameObject.GetComponent<Player>();
        isDead = false;


        playSE = false;
        searchingSE = false;
        searchedSE = false;
        openDoor = false;
    }

    private async void Update()
    {
        if (player.deathCnt >= 3)
        {
            if (playSE) return;
            SEManager.Instance.Play(
                audioPath: SEPath.TASK_FAILURE, //再生したいオーディオのパス
                volumeRate: 1,                //音量の倍率
                delay: 1,                //再生されるまでの遅延時間
                pitch: 1,                //ピッチ
                isLoop: false,             //ループ再生するか
                callback: null              //再生終了後の処理
            );
            playSE = true;
            if (!isDead)
            {
                if(RoomModel.Instance && OnlineGameManager.Player.name == "Worker")
                    Initiate.Fade("Exp_Worker_K03", Color.black, 1.0f);
                else if (RoomModel.Instance && OnlineGameManager.Player.name == "Stricker")
                    Initiate.Fade("Exp_Stricker_K03", Color.black, 1.0f);
                else
                    Initiate.Fade("Exp_Worker_3", Color.black, 1.0f);
            }
            isDead = true;
        }

        //VR時のみの処理
        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            //VRでオブジェクトを調査していたら
            if (xrControllerButtonEvents.isTrriger)
            {
                isCheckNow = true;
                Debug.Log("trueにしたよ");
            }
            else
            {
                isCheckNow = false;
                Debug.Log("falseにしたよ");
            }
        }


        //nullチェック
        if (nowFindObj == null) return;


        //オブジェクト調査中
        if (isCheckNow)
        {
            if (nowFindStatus.isChecked || !nowFindStatus.canCheckArea) return;  //チェック済みはreturn

            if (nowFindObj.name == "Door" && !isGetKey) return; //鍵非所持でドア開錠もreturn

            if(!searchingSE)
            {
                if (nowFindObj.name == "Door")
                {
                    SEManager.Instance.Play(
                        audioPath: SEPath.DIFFUSE, //再生したいオーディオのパス
                        volumeRate: 1,                //音量の倍率
                        delay: 0,                //再生されるまでの遅延時間
                        pitch: 1,                //ピッチ
                        isLoop: true,             //ループ再生するか
                        callback: null              //再生終了後の処理
                    );
                }
                else
                {
                    SEManager.Instance.Play(
                        audioPath: SEPath.SEARCH_KEY, //再生したいオーディオのパス
                        volumeRate: 1,                //音量の倍率
                        delay: 0,                //再生されるまでの遅延時間
                        pitch: 1,                //ピッチ
                        isLoop: true,             //ループ再生するか
                        callback: null              //再生終了後の処理
                    );
                }

                searchingSE = true;
            }

            checkNowText.SetActive(true);   //調査テキスト表示

            //VR時は無視
            if (!UnityEngine.XR.XRSettings.isDeviceActive)
            {
                playerCamera.enabled = false;   //カメラアングル固定化
                playerMove.enabled = false;     //プレイヤー座標固定化
            }

            nowFindStatus.checkedTime += Time.deltaTime;   //調査時間加算

            if (nowFindObj.name == "Book" || nowFindObj.name == "Cube_2") player.isLow = true;
            else player.isLow = false;

            player.isSearch = isCheckNow;
        }
        else if (!isCheckNow)
        {
            checkNowText.SetActive(false); //調査テキスト非表示

            //VR時は無視
            if (!UnityEngine.XR.XRSettings.isDeviceActive)
            {
                playerCamera.enabled = true;   //アングル固定解除
                playerMove.enabled = true;     //座標固定解除
            }
            player.isSearch = isCheckNow;

        }
        



        //調査オブジェクトの調査時間が所要時間を超えたら
        if (chackableTimer <= nowFindStatus.checkedTime)
        {
            if(nowFindObj.name == "Door")
            {
                if (!isGetKey) return;

                if(!openDoor)
                {
                    SEManager.Instance.Play(
                        audioPath: SEPath.UNLOCK_KEY, //再生したいオーディオのパス
                        volumeRate: 1,                //音量の倍率
                        delay: 0,                //再生されるまでの遅延時間
                        pitch: 1,                //ピッチ
                        isLoop: false,             //ループ再生するか
                        callback: null              //再生終了後の処理
                    );

                    SEManager.Instance.Play(
                        audioPath: SEPath.OPEN_DOOR, //再生したいオーディオのパス
                        volumeRate: 1,                //音量の倍率
                        delay: 0.5f,                //再生されるまでの遅延時間
                        pitch: 1,                //ピッチ
                        isLoop: false,             //ループ再生するか
                        callback: null              //再生終了後の処理
                    );

                    SEManager.Instance.Play(
                        audioPath: SEPath.TASK_COMPLETED, //再生したいオーディオのパス
                        volumeRate: 1,                //音量の倍率
                        delay: 1,                //再生されるまでの遅延時間
                        pitch: 1,                //ピッチ
                        isLoop: false,             //ループ再生するか
                        callback: null              //再生終了後の処理
                    );
                    SEManager.Instance.Stop(SEPath.DIFFUSE);

                    openDoor = true;
                }
                if (RoomModel.Instance)
                    await RoomModel.Instance.CountAsync(true);
                else
                    Initiate.Fade("Exp_Worker_3", Color.black, 1.0f);
            }

            // 調べたオブジェクトにキーが入っていた場合
            if (nowFindObj == checkableObjList[keyObjectNum])
            {

                if (!searchedSE)
                {
                    Debug.Log("鍵発見");
                    if (RoomModel.Instance)
                        await RoomModel.Instance.CountAsync(true);
                    else
                        GameObject.Find("TaskCount").GetComponent<Text>().text = ": " + "1" + "/1";

                    SEManager.Instance.Play(
                        audioPath: SEPath.FIND_KEY, //再生したいオーディオのパス
                        volumeRate: 1,                //音量の倍率
                        delay: 0,                //再生されるまでの遅延時間
                        pitch: 1,                //ピッチ
                        isLoop: false,             //ループ再生するか
                        callback: null              //再生終了後の処理
                    );
                    isGetKey = true;

                    searchedSE = true;
                }
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
        if (nowFindStatus == null) return;

        isCheckNow = false;

        if (searchingSE)
        {
            //SYSTEM20のSEだけを停止
            SEManager.Instance.Stop(SEPath.SEARCH_KEY);
            SEManager.Instance.Stop(SEPath.DIFFUSE);
            searchingSE = false;
        }

        

    }
}
