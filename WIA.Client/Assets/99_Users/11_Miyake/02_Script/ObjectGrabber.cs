//==============================================
//プレイヤー移動（オブジェクトを掴む、離す）
//三宅歩人
//==============================================
using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{
    [Header("掴み設定")]
    [SerializeField] float grabDistance = 3f;     // 掴める最大距離（Rayの届く範囲）
    [SerializeField] Transform holdPoint;         // 掴んだオブジェクトを保持する位置（カメラの子に設定）

    [Header("物理設定")]
    [SerializeField] float moveForce = 250f;      // 掴んだオブジェクトをHoldPointに引き寄せる力
    [SerializeField] float maxDistance = 4f;      // 掴んだオブジェクトがこの距離より離れたら自動で離す

    // 現在掴んでいるオブジェクトのRigidbody参照
    private Rigidbody grabbedRb = null;

    void Update()
    {
        // 左クリックで掴む
        if (Input.GetMouseButtonDown(0))
        {
            // まだ何も掴んでいないときのみ掴む処理
            if (grabbedRb == null)
                Grab();
        }

        // 右クリックで離す
        if (Input.GetMouseButtonDown(1))
        {
            // 何かを掴んでいる場合のみ離す処理
            if (grabbedRb != null)
                Release();
        }
    }

    //==============================================
    // 物理更新（FixedUpdate内で物理演算を扱う）
    //==============================================
    void FixedUpdate()
    {
        // 掴んでいる間はHoldPointに向かって引き寄せる
        if (grabbedRb != null)
        {
            // HoldPointまでのベクトルを計算
            Vector3 toHoldPoint = holdPoint.position - grabbedRb.position;
            float distance = toHoldPoint.magnitude;

            // 掴んだ物体がプレイヤーから離れすぎたら自動で離す
            if (distance > maxDistance)
            {
                Release();
                return;
            }

            // 物理的にHoldPoint方向へ力を加える
            // ForceMode.VelocityChange：速度を即座に変化させるタイプの力
            grabbedRb.AddForce(toHoldPoint * moveForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    //==============================================
    // 掴む処理
    //==============================================
    void Grab()
    {
        // このスクリプトがアタッチされているカメラを取得
        Camera cam = GetComponent<Camera>();
        if (cam == null) return;

        // カメラ正面方向にRayを飛ばす
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // Rayが何かに当たったかチェック
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            // "Item"タグのオブジェクトだけ掴めるようにする
            if (hit.collider.CompareTag("Item"))
            {
                // ぶつかったコライダーにRigidbodyがあるか取得
                grabbedRb = hit.collider.attachedRigidbody;
                if (grabbedRb != null)
                {
                    // 掴んでいる間も物理挙動は有効
                    grabbedRb.useGravity = true;

                    // 移動を安定させるために空気抵抗を高める
                    grabbedRb.linearDamping = 10f;

                    // 回転を固定して、持っている間にオブジェクトが暴れないようにする
                    grabbedRb.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
        }
    }

    //==============================================
    // 離す処理
    //==============================================
    void Release()
    {
        if (grabbedRb == null) return;

        // 空気抵抗を元に戻す
        grabbedRb.linearDamping = 0f;

        // 回転固定を解除して自然な物理挙動に戻す
        grabbedRb.constraints = RigidbodyConstraints.None;

        // 掴んでいた参照を解除
        grabbedRb = null;
    }
}
