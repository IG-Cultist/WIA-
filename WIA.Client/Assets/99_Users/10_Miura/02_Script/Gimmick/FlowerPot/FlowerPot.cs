using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] public GameObject potObj;
    [SerializeField] GameObject potFragmentObj;
    [SerializeField] float moveSpeed;

    Vector3 mouse;
    Vector3 target;

    public bool isGrab = false;

    // カメラマネージャースクリプト
    CameraManager cameraManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrab) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
          
        }
        else 
        {
            Cursor.visible = false;
           
        } 

        if (Input.GetKeyDown(KeyCode.E))
        {
            isGrab = true;
            
            cameraManager.TurnOffPlayerCam();
            cameraManager.TurnOnLookDownCam();
           
        }

        if (Input.GetKeyDown(KeyCode.Q))
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
        if(collision.gameObject.tag=="Base")
        {//Baseタグのオブジェクトに触れたら
            FlowerPotManager flowerPotManager = GameObject.Find("FlowerPotManager").GetComponent<FlowerPotManager>();
            potObj = flowerPotManager.potObj;
            potFragmentObj = flowerPotManager.potFragmentObj;

            GameObject fragment; //破片オブジェクト
            fragment = Instantiate(potFragmentObj, this.gameObject.transform.position, this.gameObject.transform.rotation);

            //植木鉢を消す
            Destroy(this.gameObject);
            flowerPotManager.potList.Remove(potObj);
            flowerPotManager.potCnt -=1;
            flowerPotManager.isPot = false;

            for (int i = 0; i < fragment.transform.childCount; i++)
            {//potFragmentObjの子の数だけループ
                fragment.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(new Vector2(50, 50)); //子を取得　

                DestroyFragment(fragment);
                FadeFragment(fragment.transform.GetChild(i)); //破片をフェードアウトさせる
            }
        }
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