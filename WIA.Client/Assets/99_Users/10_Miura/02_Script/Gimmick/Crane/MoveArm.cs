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

        // 現在のキーボード情報
        var currentKeyBoard = Keyboard.current;

        // キーボード接続チェック
        if (currentKeyBoard == null)
        {
            // キーボードが接続されていないと
            // Keyboard.currentがnullになる
            return;
        }

        // キーの入力状態取得
        var rKey = currentKeyBoard.rKey;
        var lkey = currentKeyBoard.lKey;

        // Rキー/Lキーが押されたかどうか
        if (rKey.wasPressedThisFrame)
        {
            //クレーンを動かす
            armTransform.DORotate(new Vector3(0, 45, 0), 40);
        }
        if (lkey.wasPressedThisFrame)
        {
            //クレーンを動かす
            armTransform.DORotate(new Vector3(0, -45, 0), 40);
        }
    }
}