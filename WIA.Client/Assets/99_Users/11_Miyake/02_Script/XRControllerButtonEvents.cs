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
    [SerializeField] private XRInputButtonReader lTrriger_PressInput = new XRInputButtonReader("L_PressTrriger");           //左トリガーを押したとき
    [SerializeField] private XRInputButtonReader lTrriger_ReleaseInput = new XRInputButtonReader("L_Release_Trriger");      //離したとき

    [Header("Button Events")]
    public UnityEvent OnRTrrigerPressed;
    public UnityEvent OnLTrrigerPressed;

    public UnityEvent OnRTrrigerReleased;
    public UnityEvent OnLTrrigerReleased;

    //探しているか
    public bool isTrriger;
    ObjectGrabber grabber;

    void Start()
    {
        isTrriger = false;

        //スクリプト取得
        grabber = GetComponent<ObjectGrabber>();

        rTrriger_PressInput.EnableDirectActionIfModeUsed();
        rTrriger_ReleaseInput.EnableDirectActionIfModeUsed();
        lTrriger_PressInput.EnableDirectActionIfModeUsed();
        lTrriger_ReleaseInput.EnableDirectActionIfModeUsed();
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
    }

    void OnDestroy()
    {
        rTrriger_PressInput.DisableDirectActionIfModeUsed() ;
        lTrriger_PressInput.DisableDirectActionIfModeUsed() ;
        rTrriger_ReleaseInput.DisableDirectActionIfModeUsed() ;
        lTrriger_ReleaseInput.DisableDirectActionIfModeUsed() ;
    }

    //右トリガーが押されたとき
    public void PressTrriger_R()
    {
        Debug.Log("右のトリガーが押されたよん");
        isTrriger = true;
    }

    //左トリガーを押したとき
    public void PressTrriger_L()
    {
        Debug.Log("左のトリガーが押されたよん");
        isTrriger = true;
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
}