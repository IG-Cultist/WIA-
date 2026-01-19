using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// VR用：プレイヤー移動 ⇔ オブジェクト操作 を切り替える制御クラス
/// XR Interaction Toolkit (3.2.2) 対応
/// 三宅歩人    2026/1/16
/// </summary>
public class VRObjectControlSwitcher : MonoBehaviour
{
    [Header("操作対象オブジェクト")]
    [SerializeField]
    private Transform targetObject;

    [Header("Locomotion 制御")]
    [SerializeField]
    private GameObject moveLocomotionObject;        // XR Origin / Locomotion / Move を指定する

    [Header("オブジェクト操作用 Input")]
    [SerializeField]
    private InputActionReference moveAction;        // スティック入力（Vector2）

    [Header("オブジェクト移動速度")]
    [SerializeField]
    private float moveSpeed = 1.5f;

    private bool isObjectControlMode = false;

    XRControllerButtonEvents xrControllerButtonEvents;

    private void Start()
    {
        isObjectControlMode = true;
        //VRのスクリプト取得
        xrControllerButtonEvents = GameObject.Find("Main").GetComponent<XRControllerButtonEvents>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Update()
    {
        MoveObject();
    }

    /// <summary>
    /// 外部から呼び出す：オブジェクト操作モード開始
    /// </summary>
    public void EnterObjectControl()
    {
        isObjectControlMode = true;

        // プレイヤー移動を完全に停止
        if (moveLocomotionObject != null)
        {
            moveLocomotionObject.SetActive(false);
        }
    }

    /// <summary>
    /// 外部から呼び出す：オブジェクト操作モード終了
    /// </summary>
    public void ExitObjectControl()
    {
        isObjectControlMode = false;

        // プレイヤー移動を復帰
        if (moveLocomotionObject != null)
        {
            moveLocomotionObject.SetActive(true);
        }
    }

    //スティックでオブジェクト動かす
    public void MoveObject()
    {
        if (xrControllerButtonEvents.isGrip) EnterObjectControl();
        else ExitObjectControl();

        // オブジェクト操作モードでなければ処理しない
        if (!isObjectControlMode) return;

        //FlowerPotスクリプトを取得して関数呼び出し
        FlowerPot flowerPot = GameObject.Find("FlowerPot_obj").GetComponent<FlowerPot>();
        flowerPot.GrabPot();

        // スティック入力を取得
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // カメラ（HMD）の向きを基準に移動方向を決定
        Transform cameraTransform = Camera.main.transform;

        // カメラの右方向と前方向を取得
        Vector3 right = cameraTransform.right;
        Vector3 forward = cameraTransform.forward;

        // 上下成分を無視（床に沿った移動にするため）
        right.y = 0f;
        forward.y = 0f;

        // 念のため正規化
        right.Normalize();
        forward.Normalize();

        // 入力値を方向ベクトルに変換
        Vector3 moveDirection =
            right * input.x +
            forward * input.y;

        // オブジェクトを移動
        // Time.deltaTime を掛けてフレームレート非依存にする
        targetObject.position +=
            moveDirection * moveSpeed * Time.deltaTime;
    }
}
