using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
public class MoveArm : MonoBehaviour
{
    [SerializeField] GameObject crane;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        Transform armTransform = crane.transform;
        var keyBoardCurrent = Keyboard.current;
        var lKey = keyBoardCurrent.lKey; //Lキー
        var rKey = keyBoardCurrent.rKey; //Rキー
        if (rKey.wasPressedThisFrame)
        {
            if(RoomModel.Instance)
            {
                if (OnlineGameManager.Player.name == "Stricker")
                    armTransform.DORotate(new Vector3(0, 45, 0), 40);
            }
            else
            armTransform.DORotate(new Vector3(0, 45, 0), 40);
        }
        if (lKey.wasPressedThisFrame)
        {
            if (RoomModel.Instance)
            {
                if (OnlineGameManager.Player.name == "Stricker")
                    armTransform.DORotate(new Vector3(0, -45, 0), 40);
            }
            else
                armTransform.DORotate(new Vector3(0, -45, 0), 40);
        }
    }
}










