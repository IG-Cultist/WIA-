using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;

public class VR_Detection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DetectCurrentDevices();
    }

    void DetectCurrentDevices()
    {
        Debug.Log("接続中のデバイス一覧:");

        foreach (var device in InputSystem.devices)
        {
            Debug.Log($"デバイス名: {device.name}, クラス: {device.GetType().Name}");

            if (device is Keyboard)
            {
                Debug.Log("これはキーボードです。");
            }
            else if (device is OpenVRHMD)
            {
                Debug.Log("これはVRです。");
            }
        }
    }
}
