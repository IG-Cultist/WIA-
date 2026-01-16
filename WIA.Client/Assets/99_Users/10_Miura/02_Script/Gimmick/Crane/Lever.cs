using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class Lever : MonoBehaviour
{
    [SerializeField] GameObject leverObj;
    [SerializeField] GameObject craneObj;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        RotateLever();
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
            if(RoomModel.Instance)
            {
                if (OnlineGameManager.Player.name == "Stricker")
                    leverTransform.DORotate(new Vector3(-20, armTransform.rotation.y, 0), 3); //レバーが左に傾く
            }
            else
            {
                if (OnlineGameManager.Player.name == "Stricker")
                    leverTransform.DORotate(new Vector3(-20, armTransform.rotation.y, 0), 3); //レバーが左に傾く
            }
        }
        if (rKey.wasPressedThisFrame)
        {//Rキーを押したら
            if (RoomModel.Instance)
            {
                leverTransform.DORotate(new Vector3(20, armTransform.rotation.y, 0), 3); //レバーが右に傾く
            }
            else
            {
                leverTransform.DORotate(new Vector3(20, armTransform.rotation.y, 0), 3); //レバーが右に傾く
            }
        }
    }
}