using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class XRControllerButtonEvents : MonoBehaviour
{
    [Header("XR Button Input Readers")]
    [SerializeField] private XRInputButtonReader rTrriger_PressInput = new XRInputButtonReader("R_Press_Trriger");
    [SerializeField] private XRInputButtonReader rTrriger_ReleaseInput = new XRInputButtonReader("R_Release_Trriger");
    [SerializeField] private XRInputButtonReader lTrriger_PressInput = new XRInputButtonReader("L_PressTrriger");
    [SerializeField] private XRInputButtonReader lTrriger_ReleaseInput = new XRInputButtonReader("L_Release_Trriger");

    [Header("Button Events")]
    public UnityEvent OnRTrrigerPressed;
    public UnityEvent OnLTrrigerPressed;

    public UnityEvent OnRTrrigerReleased;
    public UnityEvent OnLTrrigerReleased;

    void Start()
    {
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

    public void PressTrriger_R()
    {
        Debug.Log("右のトリガーが押されたよん");
    }

    public void PressTrriger_L()
    {
        Debug.Log("左のトリガーが押されたよん");
    }

    public void ReleaseTrriger_R()
    {
        Debug.Log("右のトリガーが離されたよん");
    }

    public void ReleaseTrriger_L()
    {
        Debug.Log("左のトリガーが離されたよん");
    }
}