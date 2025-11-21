//==============================================
// プレイヤー操作（オブジェクトを掴む、離す）
// 三宅歩人
//==============================================
using UnityEngine;
using UnityEngine.UI;

public class ObjectGrabber : MonoBehaviour
{
    [Header("掴む系")]
    [SerializeField] Transform holdPoint;         // 掴んだオブジェクトを保持する位置（カメラの子に設定）
    [SerializeField] float grabDistance = 3f;     // 掴める最大距離（Rayの届く範囲）
    [SerializeField] float moveForce = 250f;      // 掴んだオブジェクトをHoldPointに引き寄せる力
    [SerializeField] float maxDistance = 4f;      // 掴んだオブジェクトがこの距離より離れたら自動で離す

    [Header("UI系")]
    [SerializeField] Image crosshairImage;         // 通常時のクロスヘア
    [SerializeField] Image leftClickImage;         // 掴めるときに表示するLeftClick

    // 現在掴んでいるオブジェクトのRigidbody参照
    private Rigidbody grabbedRb = null;

    void Start()
    {
        // LeftClickは初期状態では非表示
        if (leftClickImage != null)
            leftClickImage.enabled = false;
    }

    void Update()
    {
        // 掴める距離にオブジェクトがあるかチェック
        ShowGrabUI();

        // 左クリックで掴む
        if (Input.GetMouseButtonDown(0))
        {
            if (grabbedRb == null)
                Grab();
        }

        // 右クリックで離す
        if (Input.GetMouseButtonDown(1))
        {
            if (grabbedRb != null)
                Release();
        }
    }

    //==============================================
    // 物理更新（FixedUpdate内で物理演算を扱う）
    //==============================================
    void FixedUpdate()
    {
        if (grabbedRb != null)
        {
            Vector3 toHoldPoint = holdPoint.position - grabbedRb.position;
            float distance = toHoldPoint.magnitude;

            if (distance > maxDistance)
            {
                Release();
                return;
            }

            grabbedRb.AddForce(toHoldPoint * moveForce * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // プレイヤーの向きに合わせてオブジェクトも回転させる
            Quaternion targetRotation = holdPoint.rotation;
            grabbedRb.MoveRotation(Quaternion.Slerp(grabbedRb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }

    //====================================================
    // 掴める距離にある場合にスプライトを表示
    //====================================================
    void ShowGrabUI()
    {
        if (grabbedRb != null)
        {
            // 掴んでいる間はUIをクロスヘア表示に戻す
            if (crosshairImage != null) crosshairImage.enabled = true;
            if (leftClickImage != null) leftClickImage.enabled = false;
            return;
        }

        Camera cam = GetComponent<Camera>();
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Itemタグのオブジェクトが掴める距離にあるか判定
        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                // 掴めるとき → Crosshair非表示、LeftClick表示
                if (crosshairImage != null) crosshairImage.enabled = false;
                if (leftClickImage != null) leftClickImage.enabled = true;
                return;
            }
        }

        // 掴めるものがない場合 → Crosshair表示、LeftClick非表示
        if (crosshairImage != null) crosshairImage.enabled = true;
        if (leftClickImage != null) leftClickImage.enabled = false;
    }

    //==============================================
    // 掴む処理
    //==============================================
    void Grab()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                grabbedRb = hit.collider.attachedRigidbody;
                if (grabbedRb != null)
                {
                    grabbedRb.useGravity = true;
                    grabbedRb.linearDamping = 10f;
                    grabbedRb.constraints = RigidbodyConstraints.FreezeRotation;

                    // 掴んだらCrosshair表示、LeftClick非表示
                    if (crosshairImage != null) crosshairImage.enabled = true;
                    if (leftClickImage != null) leftClickImage.enabled = false;
                }
            }
            else if (hit.collider.CompareTag("CheckableObject")) //調査可能オブジェクトに触れた場合
            {
                // 調査を開始する
                GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>().CheckObject(hit.transform.gameObject);
            }
            else if (hit.collider.CompareTag("CoffeeMachine")) //コーヒーマシンに触れた場合
            {
                // コーヒーを生成する
                GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>().DripCoffee();
            }
            else if (hit.collider.CompareTag("Pot")) //植木鉢に触れた場合
            {
                GameObject.Find("FlowerPot_obj(Clone)").GetComponent<FlowerPot>().GrabPot();
            }
        }
    }

    //==============================================
    // 離す処理
    //==============================================
    void Release()
    {
        if (grabbedRb == null) return;

        grabbedRb.linearDamping = 0f;
        grabbedRb.constraints = RigidbodyConstraints.None;
        grabbedRb = null;
    }
}
