/// ------------------------------
/// ポットドロップマネージャー
/// Author:Nishiura Date:25/11/17
/// ------------------------------
using UnityEngine;

public class PotDrop : MonoBehaviour
{
    Vector3 mouse;
    GameObject Pot;

    // カメラマネージャースクリプト
    CameraManager cameraManager;

    private void Start()
    {
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
    }

    void Update()
    {
        if (Pot == null) return;

        cameraManager.TurnOffPlayerCam();
        cameraManager.TurnOnLookDownCam();
        mouse = Input.mousePosition;
        Pot.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, 18f, mouse.z));
        this.transform.position = Pot.transform.position;
    }

    /// <summary>
    /// 取得ゲームオブジェクト代入処理
    /// </summary>
    /// <param name="Pot"></param>
    public void GetPotObject(GameObject Pot)
    {
        this.Pot = Pot;
    }
}
