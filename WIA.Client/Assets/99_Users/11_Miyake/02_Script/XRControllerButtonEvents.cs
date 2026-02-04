//===============================================
//VRのボタン管理
//2025/12/04
//===============================================
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class XRControllerButtonEvents : MonoBehaviour
{
    [Header("VRのボタン")]
    [SerializeField] private XRInputButtonReader rTrriger_PressInput = new XRInputButtonReader("R_Press_Trriger");          //右トリガーを押した時
    [SerializeField] private XRInputButtonReader rTrriger_ReleaseInput = new XRInputButtonReader("R_Release_Trriger");      //離したとき
    [SerializeField] private XRInputButtonReader lTrriger_PressInput = new XRInputButtonReader("L_Press_Trriger");           //左トリガーを押したとき
    [SerializeField] private XRInputButtonReader lTrriger_ReleaseInput = new XRInputButtonReader("L_Release_Trriger");      //離したとき

    [SerializeField] private XRInputButtonReader rGrip_PressInput = new XRInputButtonReader("R_Press_Grip");          //右グリップを押した時
    [SerializeField] private XRInputButtonReader rGrip_ReleaseInput = new XRInputButtonReader("R_Release_Grip");      //離したとき
    [SerializeField] private XRInputButtonReader lGrip_PressInput = new XRInputButtonReader("L_Press_Grip");           //左グリップを押したとき
    [SerializeField] private XRInputButtonReader lGrip_ReleaseInput = new XRInputButtonReader("L_Release_Grip");      //離したとき

    [Header("Button Events")]
    public UnityEvent OnRTrrigerPressed;
    public UnityEvent OnLTrrigerPressed;

    public UnityEvent OnRTrrigerReleased;
    public UnityEvent OnLTrrigerReleased;

    public UnityEvent OnRGripPressed;
    public UnityEvent OnLGripPressed;
    
    public UnityEvent OnRGripReleased;
    public UnityEvent OnLGripReleased;

    //探しているか
    public bool isTrriger;      //トリガーを押したか
    public bool isGrip;         //グリップを押したか

    //フラグ
    public GameObject nowObj;
    public bool isWater = false;
    public bool isItemBox = false;
    public bool isCheck = false;

    ObjectGrabber grabber;

    void Start()
    {
        nowObj = null;
        isWater = false;
        isItemBox = false;
        isCheck = false;
        isTrriger = false;
        isGrip = false;

        //スクリプト取得
        grabber = GetComponent<ObjectGrabber>();

        rTrriger_PressInput.EnableDirectActionIfModeUsed();
        rTrriger_ReleaseInput.EnableDirectActionIfModeUsed();
        lTrriger_PressInput.EnableDirectActionIfModeUsed();
        lTrriger_ReleaseInput.EnableDirectActionIfModeUsed();

        rGrip_PressInput.EnableDirectActionIfModeUsed();
        rGrip_ReleaseInput.EnableDirectActionIfModeUsed();
        lGrip_PressInput.EnableDirectActionIfModeUsed();
        lGrip_ReleaseInput.EnableDirectActionIfModeUsed();
    }

    void Update()
    {
        // ボタンが押されたらチェック
        if (rTrriger_PressInput.ReadWasPerformedThisFrame())
        {
            OnRTrrigerPressed?.Invoke();
        }
        if (rTrriger_PressInput.ReadWasCompletedThisFrame())
        {
            OnRTrrigerReleased?.Invoke();
        }

        if (lTrriger_PressInput.ReadWasPerformedThisFrame())
        {
            OnLTrrigerPressed?.Invoke();
        }
        if (lTrriger_PressInput.ReadWasCompletedThisFrame())
        {
            OnLTrrigerReleased?.Invoke();
        }

        if (rGrip_PressInput.ReadWasPerformedThisFrame())
        {
            OnRGripPressed?.Invoke();
        }
        if (rGrip_PressInput.ReadWasCompletedThisFrame())
        {
            OnRGripReleased?.Invoke();
        }

        if (lGrip_PressInput.ReadWasPerformedThisFrame())
        {
            OnLGripPressed?.Invoke();
        }
        if (lGrip_PressInput.ReadWasCompletedThisFrame())
        {
            OnLGripReleased?.Invoke();
        }
    }

    void OnDestroy()
    {
        rTrriger_PressInput.DisableDirectActionIfModeUsed() ;
        lTrriger_PressInput.DisableDirectActionIfModeUsed() ;
        rTrriger_ReleaseInput.DisableDirectActionIfModeUsed() ;
        lTrriger_ReleaseInput.DisableDirectActionIfModeUsed() ;

        rGrip_PressInput.DisableDirectActionIfModeUsed() ;
        lGrip_PressInput.DisableDirectActionIfModeUsed() ;
        rGrip_ReleaseInput.DisableDirectActionIfModeUsed() ;
        lGrip_ReleaseInput.DisableDirectActionIfModeUsed() ;
    }

    //右トリガーが押されたとき
    public async void PressTrriger_R()
    {
        Debug.Log("右のトリガーが押されたよん");
        isTrriger = true;

        if(SceneManager.GetActiveScene().name == "Stage_2" && isCheck)
        {
            // 調査を開始する
            GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>().CheckInObject(nowObj.transform.gameObject, nowObj.transform.GetComponent<FindKeyStatus>());        
        }

        if (SceneManager.GetActiveScene().name == "Stage_3" && isWater)
        {
            await RoomModel.Instance.ActGimicAsync(nowObj.transform.parent.parent.name);
        }
        else if(SceneManager.GetActiveScene().name == "Stage_3" && isItemBox)
        {
            ItemBox itemBox = GameObject.Find("ItemBox").GetComponent<ItemBox>();
            itemBox.GetItem();
        }
    }

    //左トリガーを押したとき
    public async void PressTrriger_L()
    {
        Debug.Log("左のトリガーが押されたよん");
        isTrriger = true;

        if (SceneManager.GetActiveScene().name == "Stage_2" && isCheck)
        {
            // 調査を開始する
            GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>().CheckInObject(nowObj.transform.gameObject, nowObj.transform.GetComponent<FindKeyStatus>());
        }

        if (SceneManager.GetActiveScene().name == "Stage_3" && isWater)
        {
            await RoomModel.Instance.ActGimicAsync(nowObj.transform.parent.parent.name);
        }
        else if (SceneManager.GetActiveScene().name == "Stage_3" && isItemBox)
        {
            ItemBox itemBox = GameObject.Find("ItemBox").GetComponent<ItemBox>();
            itemBox.GetItem();
        }

    }

    //右トリガーを離したとき
    public void ReleaseTrriger_R()
    {
        Debug.Log("右のトリガーが離されたよん");
        isTrriger = false;

        //ステージ2のみ
        if(SceneManager.GetActiveScene().name == "Stage_2")
        {
            //音を止める
            CheckableObjManager manager = GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>();
            manager.CheckOutObject();
        }
    }

    //左トリガーを離したとき
    public void ReleaseTrriger_L()
    {
        Debug.Log("左のトリガーが離されたよん");
        isTrriger = false;

        //ステージ2のみ
        if (SceneManager.GetActiveScene().name == "Stage_2")
        {
            //音を止める
            CheckableObjManager manager = GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>();
            manager.CheckOutObject();
        }
    }

    //右グリップを押したとき
    public void PressGrip_R()
    {
        Debug.Log("右のグリップが押されたよん");
        isGrip = true;
    }

    //左グリップを押したとき
    public void PressGrip_L()
    {
        Debug.Log("左のグリップが押されたよん");
        isGrip = true;
    }

    //右グリップを離したとき
    public void ReleaseGrip_R()
    {
        Debug.Log("右のグリップが離されたよん");
        isGrip = false;
    }

    //左グリップを離したとき
    public void ReleaseGrip_L()
    {
        Debug.Log("左のグリップが離されたよん");
        isGrip = false;
    }
}