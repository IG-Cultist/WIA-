using UnityEngine;
using UnityEngine.XR;

public class VR_Detection : MonoBehaviour
{
    public static VR_Detection Instance;

    public bool isVRActive = false;
    public string deviceName = "None";

    private void Awake()
    {
        // シングルトン化（必要ない場合は削除OK）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DetectVR();
    }

    void Update()
    {
        // 実行中にVR機器の接続状況が変わることがある場合に再確認
        DetectVR();
    }

    void DetectVR()
    {
        isVRActive = XRSettings.isDeviceActive;
        deviceName = XRSettings.loadedDeviceName;

        if (isVRActive)
        {
            VRMode();
        }
        else
        {
            NonVRMode();
        }
    }

    void VRMode()
    {
        // ここにVR専用処理を書く
        Debug.Log("【VRモード】使用中のデバイス：" + deviceName);
    }

    void NonVRMode()
    {
        // ここにPC・スマホ用処理を書く
        Debug.Log("【非VRモード】現在VRは使用されていません");
    }
}
