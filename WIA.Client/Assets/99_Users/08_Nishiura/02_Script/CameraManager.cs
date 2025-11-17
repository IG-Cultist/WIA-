/// ------------------------------
/// カメラマネージャー
/// Author:Nishiura Date:25/11/17
/// ------------------------------
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // 一人称視点カメラ
    [SerializeField] GameObject playerCamera;
    // 三人称視点カメラ
    [SerializeField] GameObject thirdPersonCamera;
    // 見下ろし視点カメラ
    [SerializeField] GameObject lookDownCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // カメラの優先順位調整
        thirdPersonCamera.GetComponent<CinemachineCamera>().Priority = 0;
        lookDownCamera.GetComponent<CinemachineCamera>().Priority = 1;
        playerCamera.transform.GetChild(1).GetComponent<CinemachineCamera>().Priority = 10;

        TurnOffLookDownCam();
        TurnOffPlayerCam();
        TurnOnPlayerCam();
    }

    #region 一人称カメラ処理
    /// <summary>
    /// 一人称視点カメラ起動処理
    /// </summary>
    public void TurnOnPlayerCam()
    {
        playerCamera.SetActive(true);
    }

    /// <summary>
    /// 一人称カメラ停止処理
    /// </summary>
    public void TurnOffPlayerCam()
    {
        playerCamera.SetActive(false);
    }
#endregion

    #region 見下ろしカメラ処理
    /// <summary>
    /// 見下ろし視点カメラ起動処理
    /// </summary>
    public void TurnOnLookDownCam()
    {
        lookDownCamera.SetActive(true);
    }

    /// <summary>
    /// 見下ろし視点カメラ停止処理
    /// </summary>
    public void TurnOffLookDownCam()
    {
        lookDownCamera.SetActive(false);
    }
    #endregion
    
    #region 三人称カメラ処理
    /// <summary>
    /// 三人称視点カメラ起動処理
    /// </summary>
    public void TurnOnThirdPersonCam()
    {
        thirdPersonCamera.SetActive(true);
    }

    /// <summary>
    /// 三人称視点カメラ停止処理
    /// </summary>
    public void TurnOffThirdPersonCam()
    {
        thirdPersonCamera.SetActive(false);
    }
    #endregion

    #region ～カメラ処理
    #endregion
}
