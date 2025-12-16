//==============================================
//スマホかVRでシーンを選別する
//三宅歩人:2025/12/16
//==============================================
using UnityEngine;
using UnityEngine.SceneManagement;

public class SortingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //VRを使っていたらVR専用のタイトルシーンに遷移する
        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            SceneManager.LoadScene("VR_00_AttentionScene");
        }
        else
        {
            SceneManager.LoadScene("00_AttentionScene");
        }
    }
}
