//==============================================
// プレイヤー操作（オブジェクトを掴む、離す）
// 三宅歩人
//==============================================
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectGrabber : MonoBehaviour
{
    [Header("掴む系")]
    [SerializeField] Transform holdPoint;         // 掴んだオブジェクトを保持する位置（カメラの子に設定）
    [SerializeField] float grabDistance = 3f;     // 掴める最大距離（Rayの届く範囲）
    [SerializeField] float moveForce = 250f;      // 掴んだオブジェクトをHoldPointに引き寄せる力
    [SerializeField] float maxDistance = 4f;      // 掴んだオブジェクトがこの距離より離れたら自動で離す

    [Header("調査処理系")]
    FirstPersonMovement playerMove;
    FirstPersonLook playerCamera;

    [SerializeField] Player player;
    [SerializeField] ButtonManager buttonManager;


    XRControllerButtonEvents xrControllerButtonEvents;
    VRObjectFindManager vrObjectFindManager;

    // 現在掴んでいるオブジェクトのRigidbody参照
    private Rigidbody grabbedRb = null;

    //通信用
    OnlineGameManager gameManager;

    TaskUIManager taskUIManager;


    void Start()
    {
        //メインキャラクターのカメラを取る
        playerCamera = GameObject.Find(player.name).transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();

        playerMove = transform.parent.gameObject.GetComponent<FirstPersonMovement>();

        //TaskUIManagerスクリプトを取得
        taskUIManager = GameObject.Find("TaskUIManager").GetComponent<TaskUIManager>();

        //ButtonManagerスクリプトを取得
        if( SceneManager.GetActiveScene().name=="Stage_1")
        {
            buttonManager = GameObject.Find("LeverButton").gameObject.GetComponent<ButtonManager>();
        }

        if (UnityEngine.XR.XRSettings.isDeviceActive)
        {
            //VRのスクリプト取得
            xrControllerButtonEvents = GameObject.Find("Main").GetComponent<XRControllerButtonEvents>();
            vrObjectFindManager = GameObject.Find("VRObjectFindManager").GetComponent<VRObjectFindManager>();
        }

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
            {
                Grab();

            }
            else
            {
                Release();
            }
        }

        //// 右クリックで離す
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (grabbedRb != null)
        //        Release();
        //}

        if (Input.GetMouseButtonUp(1))
        {
            if (SceneManager.GetActiveScene().name == "Stage_2")
            {
                Debug.Log("解除");
                // 調査を開始する
                GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>().CheckOutObject();
            }
            if (SceneManager.GetActiveScene().name == "Stage_2")
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
        //if (grabbedRb != null)
        //{
        //    // 掴んでいる間はUIをクロスヘア表示に戻す
        //    taskUIManager.ShowCrossHair();

        //    return;
        //}

        Camera cam = GetComponent<Camera>();
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Itemタグのオブジェクトが掴める距離にあるか判定
        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Item") || hit.collider.CompareTag("LeverButton_R")|| hit.collider.CompareTag("LeverButton_L") || hit.collider.CompareTag("CheckableObject") || hit.collider.CompareTag("CoffeeMachine") || hit.collider.CompareTag("ItemBox") || hit.collider.CompareTag("WaterCooler")
)
            {
                // 掴めるとき → Crosshair非表示、LeftClick表示
                taskUIManager.ShowLeftCrickIcon();
            }
            else if(hit.collider.CompareTag("Pot"))
            {
                taskUIManager.ShowRightCrickIcon();
            }
            else
            {
                // 掴めるものがない場合 → Crosshair表示、LeftClick非表示
                taskUIManager.ShowCrossHair();
            }
        }
        else
        {
            // 掴めるものがない場合 → Crosshair表示、LeftClick非表示
            taskUIManager.ShowCrossHair();
        }

    }

    //==============================================
    // 掴む処理
    //==============================================
    async void Grab()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                // 掴んだものがナイフかつプレイヤーが労働者であった場合、処理しない
                if (hit.collider.name == "knife" && player.name == "Worker") return;
                grabbedRb = hit.collider.attachedRigidbody;
                if (grabbedRb != null)
                {
                    grabbedRb.useGravity = true;
                    grabbedRb.linearDamping = 10f;
                    grabbedRb.constraints = RigidbodyConstraints.FreezeRotation;

                    player.isHave = true;

                    // 掴んだらCrosshair表示、LeftClick非表示
                    taskUIManager.ShowCrossHair();
                }
            }
            else if (hit.collider.CompareTag("CheckableObject")) //調査可能オブジェクトに触れた場合
            {
                // 調査を開始する
                GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>().CheckInObject(hit.transform.gameObject, hit.transform.GetComponent<FindKeyStatus>());

            }
            else if (hit.collider.CompareTag("CoffeeMachine")) //コーヒーマシンに触れた場合
            {
                if (player.name != "Worker") return;

                // コーヒーを生成する
                GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>().RequestCoffee();
            }
            else if (hit.collider.CompareTag("Pot")) //植木鉢に触れた場合
            {
                if (RoomModel.Instance)
                {
                    GameObject.Find(hit.collider.name).GetComponent<FlowerPot>().GrabPot();
                }
                else
                {
                    GameObject.Find("FlowerPot_obj").GetComponent<FlowerPot>().GrabPot();
                }
            }
            else if (hit.collider.CompareTag("WaterCooler"))    // ウォータークーラーに触れた場合
            {
                if (RoomModel.Instance)
                {
                    // ギミック動作同期
                    await RoomModel.Instance.ActGimicAsync(hit.transform.parent.parent.name);
                }
                else
                {
                    // 水を吹き出す
                    hit.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else if (hit.collider.CompareTag("ItemBox"))    // アイテムボックスの場合
            {
                GameObject.Find("ItemBox").GetComponent<ItemBox>().GetItem();
            }
            else if (hit.collider.CompareTag("LeverButton_R")) // レバーの右ボタンの場合
            {
                buttonManager.OnButton_R_PC();
            }
            else if (hit.collider.CompareTag("LeverButton_L")) // レバーの左ボタンの場合
            {
                buttonManager.OnButton_L_PC();
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
