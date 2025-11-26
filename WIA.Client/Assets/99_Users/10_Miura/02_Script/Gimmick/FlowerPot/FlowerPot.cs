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
    [SerializeField] public GameObject potObj;
    [SerializeField] GameObject potFragmentObj;
    [SerializeField] float moveSpeed;
    // 警告円のプレハブ
    [SerializeField] GameObject dangerZone;
    Vector3 mouse;
    Vector3 target;

    int potCheckCnt;
    public bool isGrab = false;

    // カメラマネージャースクリプト
    CameraManager cameraManager;

    // 警告円オブジェクト
    GameObject dangerZoneObj;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        potObj.name = "FlowerPot_obj";
    }

    // Update is called once per frame
    void Update()
    {
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

        if (Input.GetMouseButtonDown(1))
        {
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
            mouse = Input.mousePosition;
            target = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 5f));

            this.transform.position = target;
        }
    }

    private async void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Base"||collision.gameObject.tag== "Player" || collision.gameObject.tag == "Abyss" || collision.gameObject.tag == "CheckableObject")
        {//Baseタグのオブジェクトに触れたら
            FlowerPotManager flowerPotManager = GameObject.Find("FlowerPotManager").GetComponent<FlowerPotManager>();
            potObj = flowerPotManager.potObj;
            potFragmentObj = flowerPotManager.potFragmentObj;

            GameObject fragment; //破片オブジェクト
            fragment = Instantiate(potFragmentObj, this.gameObject.transform.position, this.gameObject.transform.rotation);

            //植木鉢を消す
            Destroy(this.gameObject);
            // 警告円を破壊する
            Destroy(dangerZoneObj);
            flowerPotManager.potList.Remove(potObj);
            flowerPotManager.potCnt -=1;
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
        {
            Destroy(potObj);
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
        fragment.GetComponent<Renderer>().material.DOFade(0, 6);
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