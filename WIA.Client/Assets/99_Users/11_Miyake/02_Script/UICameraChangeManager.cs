//==============================================
//UIカメラの切替
//三宅歩人:2025/12/11
//==============================================
using UnityEngine;

public class UICameraChangeManager : MonoBehaviour
{
    //カメラ
    private GameObject pcCamera;
    private GameObject vrCamera;

    [SerializeField] GameObject pcCanvas;
    [SerializeField] GameObject vrCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //VRの場合カメラを切り替える
        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            pcCamera = GameObject.Find("PC_Camera").gameObject;
            pcCamera.SetActive(false);
            Debug.Log("PCのカメラをOFFにしたよん");
            if(pcCanvas) pcCanvas.SetActive(false);
        }
        else
        {
            vrCamera = GameObject.Find("VR_Camera").gameObject;
            vrCamera.SetActive(false);
            Debug.Log("VRのカメラをOFFにしたよん");
            if (vrCanvas) vrCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
