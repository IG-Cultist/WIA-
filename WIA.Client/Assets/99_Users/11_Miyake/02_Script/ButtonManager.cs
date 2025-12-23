//==============================================
//ボタン（オブジェクト）処理
//三宅歩人：2025/12/19
//==============================================
using DG.Tweening;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ボタンの初期位置
        buttonPostion_R = leverButton_R.transform.position;
        buttonPostion_L = leverButton_L.transform.position;

        //VRのスクリプト取得
        xrControllerButtonEvents = GameObject.Find("Main").GetComponent<XRControllerButtonEvents>();
        vrObjectFindManager = GameObject.Find("VRObjectFindManager").GetComponent<VRObjectFindManager>();
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
                    armTransform.DORotate(new Vector3(0, 45, 0), 40);
            }
            else
                armTransform.DORotate(new Vector3(0, 45, 0), 40);

            //leverButton_R.transform.position = new Vector3(leverButton_R.transform.position.x, -0.05f, leverButton_R.transform.position.z);
        }
        if (lKey.wasPressedThisFrame)
        {
            if (RoomModel.Instance)
            {
                if (OnlineGameManager.Player.name == "Stricker")
                    armTransform.DORotate(new Vector3(0, -45, 0), 40);
            }
            else
                armTransform.DORotate(new Vector3(0, -45, 0), 40);

            //leverButton_L.transform.position = new Vector3(leverButton_L.transform.position.x, -0.05f, leverButton_L.transform.position.z);
        }
    }

    //右のボタン
    public void OnButton_R()
    {
        Transform armTransform = crane.transform;

        if (xrControllerButtonEvents.isTrriger)
        {
            armTransform.DORotate(new Vector3(0, 45, 0), 40);       //クレーンを右に
        }
    }

    //左のボタン
    public void OnButton_L()
    {
        Transform armTransform = crane.transform;

        if (xrControllerButtonEvents.isTrriger)
        {
            armTransform.DORotate(new Vector3(0, -45, 0), 40);       //クレーンを右に
        }
    }
}
