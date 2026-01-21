//=============================================
// VRでオブジェクトを探す判定（左右対応）
//三宅歩人:2025/12/5
//=============================================
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRObjectFindManager : MonoBehaviour
{
    [Header("Near-Far Interactors (LeftとRight)")]
    [SerializeField] NearFarInteractor nearFarInteractor_L; // 左手の Near-Far Interactor
    [SerializeField] NearFarInteractor nearFarInteractor_R; // 右手の Near-Far Interactor

    // 前回ヒットしていたオブジェクトを保存して、"当たった瞬間だけ" を検出する
    private GameObject lastHitObjLeft;
    private GameObject lastHitObjRight;

    // レイキャストのデフォルト最大距離（必要に応じてInspectorから調整できるようにしても良い）
    [SerializeField] float defaultMaxDistance = 10f;

    // 必要ならヒットさせたくないレイヤーがある場合、LayerMaskを使って除外できます
    [SerializeField] LayerMask raycastMask = ~0; // デフォルトは全部（全レイヤー）

    //ボタン入力取得
    XRControllerButtonEvents xrControllerButtonEvents;

    private void Start()
    {
        xrControllerButtonEvents = GameObject.Find("Main").GetComponent<XRControllerButtonEvents>();
    }

    void Update()
    {
        // 左右をチェック（nullチェック）
        CheckHit(nearFarInteractor_L, ref lastHitObjLeft);
        CheckHit(nearFarInteractor_R, ref lastHitObjRight);
    }

    /// <summary>
    /// 指定した NearFarInteracto（または類似のinteractor）から起点・方向を取って
    /// Physics.Raycastを投げてタグが当たった瞬間にタグに対応した関数を呼ぶ
    /// </summary>
    /// <param name="interactor">NearFarInteractor（Inspector でセット）</param>
    /// <param name="lastHitObj">前回ヒットしていたオブジェクト（ref）</param>
    void CheckHit(NearFarInteractor interactor, ref GameObject lastHitObj)
    {
        if (interactor == null)
            return;

        //NearFarInteractorは'curveOrigin'プロパティを持っているため起点と方向は取得可能
        var originTransform = interactor.curveOrigin;
        if (originTransform == null)
        {
            // originが無い場合はinteractorのtransformを使う
            originTransform = interactor.transform;
        }

        Vector3 origin = originTransform.position;
        Vector3 direction = originTransform.forward;

        // ここではinteractorが持つ最大距離（もしpublicプロパティがあればそれを使う）の代わりにdefaultMaxDistanceを使用
        float maxDistance = defaultMaxDistance;

        // Raycast 発射
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, raycastMask, QueryTriggerInteraction.Ignore))
        {
            GameObject obj = hit.collider.gameObject;
            HandleHitObject(obj, ref lastHitObj, interactor);
        }
        else
        {
            // ヒットしていない場合は前回のヒットをリセット（Exit 相当）
            lastHitObj = null;
        }
    }

    /// <summary>
    /// ヒットしたオブジェクトが CheckableObject タグなら、"当たった瞬間" の処理を実行する。
    /// </summary>
    void HandleHitObject(GameObject obj, ref GameObject lastHitObj, NearFarInteractor interactor)
    {
        if (obj.CompareTag("CheckableObject"))
        {
            // 新しく当たった瞬間だけ処理を実行
            if (obj != lastHitObj)
            {
                lastHitObj = obj;
                OnCheckableHit(obj, interactor);
            }
        }
        else if (obj.CompareTag("CoffeeMachine"))
        {
            // 新しく当たった瞬間だけ処理を実行
            if (obj != lastHitObj)
            {
                lastHitObj = obj;
                OnCoffeeHit(obj, interactor);
            }
        }
        else if (obj.CompareTag("LeverButton_R"))
        {
            // 新しく当たった瞬間だけ処理を実行
            if (obj != lastHitObj)
            {
                lastHitObj = obj;
                OnLeverButtonHit_R(obj,interactor);
            }
        }
        else if (obj.CompareTag("LeverButton_L"))
        {
            // 新しく当たった瞬間だけ処理を実行
            if (obj != lastHitObj)
            {
                lastHitObj = obj;
                OnLeverButtonHit_L(obj, interactor);
            }
        }
        else
        {
            // タグが違う → 以前のヒットはリセット
            lastHitObj = null;

            //ステージ2のみ
            if (SceneManager.GetActiveScene().name == "Stage_2_Miyake")
            {
                OutCheckableHit();
            }
        }
    }

    /// <summary>
    /// CheckableObject にヒットした瞬間に呼ばれる処理
    /// </summary>
    void OnCheckableHit(GameObject obj, NearFarInteractor interactor)
    {
        Debug.Log($"CheckableObject にヒット: {obj.name} ｜ Interactor: {interactor.gameObject.name}");

        // FindKeyStatus と CheckInObject を使うため、まずステータスを取る
        FindKeyStatus status = obj.GetComponent<FindKeyStatus>();
        if (status == null)
        {
            Debug.LogWarning("FindKeyStatus がオブジェクトに見つかりません");
            return;
        }

        // CheckableObjManagerに処理を渡す
        CheckableObjManager manager = GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>();
        manager.CheckInObject(obj, status);
    }

    /// <summary>
    /// レバーのボタンを押す
    /// </summary>
    public void OnLeverButtonHit_R(GameObject obj, NearFarInteractor interactor)
    {
        Debug.Log("右のボタンだよ");
        GameObject.Find("LeverButton").GetComponent<ButtonManager>().OnButton_R();
    }

    public void OnLeverButtonHit_L(GameObject obj, NearFarInteractor interactor)
    {
        Debug.Log("左のボタンだよ");
        GameObject.Find("LeverButton").GetComponent<ButtonManager>().OnButton_L();
    }

    //音を止める
    public void OutCheckableHit()
    {
        CheckableObjManager manager = GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager> ();
        manager.CheckOutObject();
    }

    //コーヒーを生成
    void OnCoffeeHit(GameObject obj, NearFarInteractor interactor)
    {
        Debug.Log($"CoffeeMachine にヒット: {obj.name} ｜ Interactor: {interactor.gameObject.name}");

        // コーヒーを生成する
        GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>().RequestCoffee();
    }
}
