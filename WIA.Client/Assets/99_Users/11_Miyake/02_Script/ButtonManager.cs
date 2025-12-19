//==============================================
//ボタン（オブジェクト）処理
//三宅歩人：2025/12/19
//==============================================
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    XRControllerButtonEvents xrControllerButtonEvents;
    Vector3 buttonPostion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonPostion = gameObject.transform.position;
        //VRのスクリプト取得
        xrControllerButtonEvents = GameObject.Find("Main").GetComponent<XRControllerButtonEvents>();
    }

    // Update is called once per frame
    void Update()
    {
        OnButton();
    }

    public void OnButton()
    {
        if (xrControllerButtonEvents.isTrriger)
        {
            Debug.Log("ボタンが押されたよ");
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, -2f, gameObject.transform.position.z);
        }
        else
        {
            Debug.Log("ボタンが離されたよ");
            gameObject.transform.position = buttonPostion;
        }
    }
}
