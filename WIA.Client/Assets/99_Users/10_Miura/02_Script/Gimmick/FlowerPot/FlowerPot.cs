using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] public GameObject potObj; //植木鉢プレハブ
    [SerializeField] GameObject potFragmentObj; //植木鉢の破片プレハブ
    // 警告円のプレハブ
    [SerializeField] GameObject dangerZone;
    Vector3 mouse;
    Vector3 target;

    public bool isGrab = false;

    // カメラマネージャースクリプト
    CameraManager cameraManager;

    // 警告円オブジェクト
    GameObject dangerZoneObj;

    // 植木鉢がスポーンした場所のリスト
    List<int> nowSpawnList = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        potObj.name = "FlowerPot_obj"; //生成される植木鉢オブジェクトの名前を固定
    }

    // Update is called once per frame
    void Update()
    {
        // 現在のマウス情報
        var currentMouse = Mouse.current;

        // マウス接続チェック
        if (currentMouse == null)
        {
            // マウスが接続されていないと
            // Mouse.currentがnullになる
            return;
        }

        // マウスカーソル位置取得
        var cursorPosition = currentMouse.position.ReadValue();

        // 左ボタンの入力状態取得
        var leftButton = currentMouse.leftButton;
        var rightButton = currentMouse.rightButton;

        // 警告円オブジェクトを花瓶の下部に常に移動させる
        if (dangerZoneObj != null) dangerZoneObj.transform.position
            = new Vector3(this.gameObject.transform.position.x,0.52f, this.gameObject.transform.position.z);

        if (isGrab) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else 
        {
            Cursor.visible = false;
        } 

        if (rightButton.wasPressedThisFrame)
        {//右クリックしたら
            isGrab = false;

            Cursor.visible = false;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;

            //接地してからn秒後に一人称に戻る
            CancelInvoke();
            Invoke("ChangeFirstCamera", 1.5f);

        }

        if (isGrab)
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
           
            target = Camera.main.ScreenToWorldPoint(new Vector3(cursorPosition.x, cursorPosition.y, 5f));

            this.transform.position = target;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Base" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "CheckableObject")
        {//Baseタグのオブジェクトに触れたら
            FlowerPotManager flowerPotManager = GameObject.Find("FlowerPotManager").GetComponent<FlowerPotManager>();
            potObj = flowerPotManager.potObj;
            potFragmentObj = flowerPotManager.potFragmentObj;
            nowSpawnList = flowerPotManager.nowSpawnList;

            GameObject fragment; //破片オブジェクト
            fragment = Instantiate(potFragmentObj, this.gameObject.transform.position, this.gameObject.transform.rotation);

            flowerPotManager.RemoveList(flowerPotManager.generatNumber);

            //植木鉢を消す
            Destroy(this.gameObject);

            // 警告円を破壊する
            Destroy(dangerZoneObj);
            flowerPotManager.potList.Remove(potObj);
            flowerPotManager.isThreePot = false;

            for (int i = 0; i < fragment.transform.childCount; i++)
            {//potFragmentObjの子の数だけループ
                fragment.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(new Vector2(50, 50)); //子を取得　

                DestroyFragment(fragment);
                FadeFragment(fragment.transform.GetChild(i)); //破片をフェードアウトさせる
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Abyss"))
        {// Abyssタグのものに触れたら
            FlowerPotManager flowerPotManager = GameObject.Find("FlowerPotManager").GetComponent<FlowerPotManager>();

            Destroy(potObj); //植木鉢を消す

            //flowerPotManagerで使用していた番号を削除
            flowerPotManager.RemoveList(flowerPotManager.generatNumber);
        }
    }

    public void GrabPot()
    {
        isGrab = true;

        // 警告円オブジェクトを生成
        dangerZoneObj = Instantiate(dangerZone, new Vector3(0f,0.52f,0f), dangerZone.transform.rotation);
        
        // カメラを切り替える
        cameraManager.TurnOffPlayerCam();
        cameraManager.TurnOnLookDownCam();
    }

    /// <summary>
    /// 破片をフェードアウトさせる処理
    /// </summary>
    /// <param name="fragment"></param>
    public void FadeFragment(Transform fragment)
    {
        // 破片をフェードアウトさせる
        fragment.GetComponent<Renderer>().material.DOFade(0, 6); // マテリアルを取得してフェードアウト
    }

    /// <summary>
    /// 破片を消す処理
    /// </summary>
    /// <param name="fragment"></param>
    public async void DestroyFragment(GameObject fragment)
    {
        await Task.Delay(6000); //6秒待つ　
        Destroy(fragment.gameObject);　//破片を消す
    }


    private void ChangeFirstCamera()
    {
        Debug.Log("Invoke通ってきたy");
        cameraManager.TurnOnPlayerCam();
        cameraManager.TurnOffLookDownCam();
    }
}