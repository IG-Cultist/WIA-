//==============================================
//ボタン（オブジェクト）処理
//三宅歩人：2025/12/19
//==============================================
using DG.Tweening;
using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject leverButton_R;
    [SerializeField] GameObject leverButton_L;
    [SerializeField] GameObject crane;

    VRObjectFindManager vrObjectFindManager;
    XRControllerButtonEvents xrControllerButtonEvents;
    Vector3 buttonPostion_R;
    Vector3 buttonPostion_L;

    bool isPlay = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ボタンの初期位置
        buttonPostion_R = leverButton_R.transform.position;
        buttonPostion_L = leverButton_L.transform.position;

        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            //VRのスクリプト取得
            xrControllerButtonEvents = OnlineGameManager.Player.GetComponent<XRControllerButtonEvents>();
            vrObjectFindManager = GameObject.Find("VRObjectFindManager").GetComponent<VRObjectFindManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        OnButton();
    }

    public void OnButton()
    {
        Transform armTransform = crane.transform;
        var keyBoardCurrent = Keyboard.current;
        var lKey = keyBoardCurrent.lKey; //Lキー
        var rKey = keyBoardCurrent.rKey; //Rキー
        if (rKey.wasPressedThisFrame)
        {
            if (RoomModel.Instance)
            {
                if (OnlineGameManager.Player.name == "Stricker")
                    armTransform.DORotate(new Vector3(0, 30, 0), 40);
            }
            else
                armTransform.DORotate(new Vector3(0, 30, 0), 40);

            //leverButton_R.transform.position = new Vector3(leverButton_R.transform.position.x, -0.05f, leverButton_R.transform.position.z);
        }
        if (lKey.wasPressedThisFrame)
        {
            if (RoomModel.Instance)
            {
                if (OnlineGameManager.Player.name == "Stricker")
                    armTransform.DORotate(new Vector3(0, -30, 0), 40);
            }
            else
                armTransform.DORotate(new Vector3(0, -30, 0), 40);

            //leverButton_L.transform.position = new Vector3(leverButton_L.transform.position.x, -0.05f, leverButton_L.transform.position.z);
        }
    }

    //右のボタン
    public void OnButton_R()
    {
        Transform armTransform = crane.transform;

        float nowDis = 30 - armTransform.rotation.y;
        float speed = 1.5f;

        //すでにクレーン移動SEが再生されていたら
        if (isPlay == true)
        {
            SEManager.Instance.Stop();
            isPlay = false;
        }

        armTransform.DORotate(new Vector3(0, 30, 0), nowDis/speed);       //クレーンを右に

        //クレーン移動SE
        SEManager.Instance.Play(
            audioPath: SEPath.CONTAINER_SOUND,   //再生したいオーディオのパス
            volumeRate: 1,                 //音量の倍率
            delay: 0,                      //再生されるまでの遅延時間
            pitch: 1,                      //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                 //再生終了後の処理
        );

        //SEが再生されていることにする
        isPlay = true;

    }

    public void OnButton_R_PC()
    {
        Transform armTransform = crane.transform;

        float nowDis = 30 - armTransform.rotation.y;
        float speed = 1.5f;

        //すでにクレーン移動SEが再生されていたら
        if (isPlay == true)
        {
            SEManager.Instance.Stop();
            isPlay = false;
        }

        armTransform.DORotate(new Vector3(0, 30, 0), nowDis/speed); // クレーンを右に移動

        //クレーン移動SE
        SEManager.Instance.Play(
            audioPath: SEPath.CONTAINER_SOUND,   //再生したいオーディオのパス
            volumeRate: 1,                 //音量の倍率
            delay: 0,                      //再生されるまでの遅延時間
            pitch: 1,                      //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                 //再生終了後の処理
        );

        //SEが再生されていることにする
        isPlay = true;

    }

    //左のボタン
    public void OnButton_L()
    {
        Transform armTransform = crane.transform;
        Debug.Log(armTransform.rotation.y);

        float nowDis = armTransform.rotation.y - (-30);
        float speed = 1.5f;

        //すでにクレーン移動SEが再生されていたら
        if (isPlay == true)
        {
            SEManager.Instance.Stop();
            isPlay = false;
        }

        armTransform.DORotate(new Vector3(0, -30, 0), nowDis/speed);       //クレーンを左に

        //クレーン移動SE
        SEManager.Instance.Play(
            audioPath: SEPath.CONTAINER_SOUND,   //再生したいオーディオのパス
            volumeRate: 1,                 //音量の倍率
            delay: 0,                      //再生されるまでの遅延時間
            pitch: 1,                      //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                 //再生終了後の処理
        );

        //SEが再生されていることにする
        isPlay = true;

    }

    public void OnButton_L_PC()
    {
        Transform armTransform = crane.transform;

        float nowDis = armTransform.rotation.y - (-30);
        float speed = 1.5f;

        //すでにクレーン移動SEが再生されていたら
        if (isPlay == true)
        {
            SEManager.Instance.Stop();
            isPlay = false;
        }

        armTransform.DORotate(new Vector3(0, -30, 0), nowDis/speed); // クレーンを 左に移動

        //クレーン移動SE
        SEManager.Instance.Play(
            audioPath: SEPath.CONTAINER_SOUND,   //再生したいオーディオのパス
            volumeRate: 1,                 //音量の倍率
            delay: 0,                      //再生されるまでの遅延時間
            pitch: 1,                      //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                 //再生終了後の処理
        );

        //SEが再生されていることにする
        isPlay = true;
    }
}
