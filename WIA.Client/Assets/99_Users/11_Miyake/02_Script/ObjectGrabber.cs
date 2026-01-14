//==============================================
// プレイヤー操作（オブジェクトを掴む、離す）
// 三宅歩人
//==============================================
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("調査処理系")]
    FirstPersonMovement playerMove;
    FirstPersonLook playerCamera;

    [SerializeField] Player player;


    // 現在掴んでいるオブジェクトのRigidbody参照
    private Rigidbody grabbedRb = null;

    //通信用
    OnlineGameManager gameManager;

    void Start()
    {
        //メインキャラクターのカメラを取る
        playerCamera = GameObject.Find(player.name).transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();

        playerMove = transform.parent.gameObject.GetComponent<FirstPersonMovement>();

        // LeftClickは初期状態では非表示
        if (leftClickImage != null)
            leftClickImage.enabled = false;

        if (RoomModel.Instance)
            gameManager = GameObject.Find("OnlineGameManager").GetComponent<OnlineGameManager>();
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

        if (Input.GetMouseButtonUp(0))
        {
            if (SceneManager.GetActiveScene().name == "Stage_2")
            {
                Debug.Log("解除");
                // 調査を開始する
                GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>().CheckOutObject();
            }
            if (SceneManager.GetActiveScene().name == "Stage_K02")
            {
                Debug.Log("解除");
                // 調査を開始する
                GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>().CheckOutObject();
            }
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
                // 掴んだものがナイフかつプレイヤーが労働者であった場合、処理しない
                if (hit.collider.name == "Knife" && player.name == "Worker") return;
                grabbedRb = hit.collider.attachedRigidbody;
                if (grabbedRb != null)
                {
                    grabbedRb.useGravity = true;
                    grabbedRb.linearDamping = 10f;
                    grabbedRb.constraints = RigidbodyConstraints.FreezeRotation;

                    player.isHave = true;

                    // 掴んだらCrosshair表示、LeftClick非表示
                    if (crosshairImage != null) crosshairImage.enabled = true;
                    if (leftClickImage != null) leftClickImage.enabled = false;
                }
            }
            else if (hit.collider.CompareTag("CheckableObject")) //調査可能オブジェクトに触れた場合
            {
                // 調査を開始する
                GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>().CheckInObject(hit.transform.gameObject, hit.transform.GetComponent<FindKeyStatus>());

            }
            else if (hit.collider.CompareTag("CoffeeMachine")) //コーヒーマシンに触れた場合
            {
                // コーヒーを生成する
                GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>().RequestCoffee();
            }
            else if (hit.collider.CompareTag("Pot")) //植木鉢に触れた場合
            {
                if(RoomModel.Instance)
                {
                    GameObject.Find(hit.collider.name).GetComponent<FlowerPot>().GrabPot();
                }
                else
                GameObject.Find("FlowerPot_obj").GetComponent<FlowerPot>().GrabPot();
            }
            else if (hit.collider.CompareTag("WaterCooler"))    // ウォータークーラーに触れた場合
            {
                // 水を吹き出す
                hit.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (hit.collider.CompareTag("ItemBox"))    // アイテムボックスの場合
            {
                GameObject.Find("ItemBox").GetComponent<ItemBox>().GetItem();
            }
        }
    }

    //==============================================
    // 離す処理
    //==============================================
    public void Release()
    {
        if (grabbedRb == null) return;

        player.isHave = false;

        grabbedRb.linearDamping = 0f;
        grabbedRb.constraints = RigidbodyConstraints.None;
        grabbedRb = null;
    }

}
