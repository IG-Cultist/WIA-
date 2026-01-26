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

    [SerializeField] private XRInputButtonReader aButton_PressInput = new XRInputButtonReader("A_Button_Press");           //Aボタンを押したとき
    [SerializeField] private XRInputButtonReader aButton_ReleaseInput = new XRInputButtonReader("A_Button_Release");      //離したとき

    [Header("Button Events")]
    public UnityEvent OnRTrrigerPressed;
    public UnityEvent OnLTrrigerPressed;

    public UnityEvent OnRTrrigerReleased;
    public UnityEvent OnLTrrigerReleased;

    public UnityEvent OnRGripPressed;
    public UnityEvent OnLGripPressed;
    
    public UnityEvent OnRGripReleased;
    public UnityEvent OnLGripReleased;

    public UnityEvent OnAButtonPressed;
    public UnityEvent OnAButtonReleased;

    //探しているか
    public bool isTrriger;      //トリガーを押したか
    public bool isGrip;         //グリップを押したか
    public bool isButton;       //ボタンを押したか
    public bool isAButton;
    ObjectGrabber grabber;

    void Start()
    {
        isTrriger = false;
        isGrip = false;
        isButton = false;
        isAButton = false;

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

        aButton_PressInput.EnableDirectActionIfModeUsed();
        aButton_ReleaseInput.EnableDirectActionIfModeUsed();
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

        if (aButton_PressInput.ReadWasPerformedThisFrame())
        {
            OnAButtonPressed?.Invoke();
        }
        if (aButton_PressInput.ReadWasCompletedThisFrame())
        {
            OnAButtonReleased?.Invoke();
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

        aButton_PressInput.DisableDirectActionIfModeUsed();
        aButton_ReleaseInput.DisableDirectActionIfModeUsed();
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
    
    //Aボタンが押されたとき
    public void PressButton_A()
    {
        Debug.Log("Aボタンが押されたよん");
        isButton = true;
    }

    public void ReleaseButton_A()
    {
        Debug.Log("Aボタンが離されたよ");

        if (isButton)
        {
            isAButton = true;
        }

        isButton = false;
    }
}