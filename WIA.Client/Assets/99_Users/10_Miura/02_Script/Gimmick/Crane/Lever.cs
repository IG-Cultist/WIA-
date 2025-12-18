using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class Lever : MonoBehaviour
{
    [SerializeField] GameObject leverObj;
    [SerializeField] GameObject craneObj;
    [SerializeField] GameObject cylinder;
    Vector3 leverPostion;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leverPostion = cylinder.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        RotateLever();

        //レバーの座標とか回転を固定
        leverObj.transform.localEulerAngles = new Vector3(this.gameObject.transform.localEulerAngles.x, 0.06f, 0.175f);
        this.gameObject.transform.position = new Vector3(leverPostion.x, cylinder.transform.position.y, leverPostion.z);
    }
    public void RotateLever()
    {
        Transform leverTransform = leverObj.transform;
        Transform armTransform = craneObj.transform;
        var keyBoardCurrent = Keyboard.current;
        var lKey = keyBoardCurrent.lKey;
        var rKey = keyBoardCurrent.rKey;
        if (lKey.wasPressedThisFrame)
        {//Lキーを押したら
            leverTransform.DORotate(new Vector3(-20, armTransform.rotation.y, 0), 3); //レバーが左に傾く
        }
        if (rKey.wasPressedThisFrame)
        {//Rキーを押したら
            leverTransform.DORotate(new Vector3(20, armTransform.rotation.y, 0), 3); //レバーが右に傾く
        }

        if (this.gameObject.transform.localEulerAngles.x >= -9 && this.gameObject.transform.localEulerAngles.x <= 9)
        {
            Debug.Log("不動あきお");
        }

        else if (this.gameObject.transform.localEulerAngles.x > 250)
        {
            Debug.Log("左に曲げたね");

            //左方向の限界値
            if (this.gameObject.transform.localEulerAngles.x < 320)
            {
                leverObj.transform.localEulerAngles = new Vector3(-40f, 0.06f, 0.175f);
            }

            armTransform.DORotate(new Vector3(0, -45, 0), 40);      //クレーンを左に
        }
        else if (this.gameObject.transform.localEulerAngles.x > 0)
        {
            Debug.Log("右に曲げたね");

            //右方向の限界値
            if (this.gameObject.transform.localEulerAngles.x > 40)
            {
                leverObj.transform.localEulerAngles = new Vector3(40f, 0.06f, 0.175f);
            }

            armTransform.DORotate(new Vector3(0, 45, 0), 40);       //クレーンを右に
        }

        Debug.Log(this.gameObject.transform.localEulerAngles.x);
    }
}







