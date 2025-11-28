using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// 鍵調査オブジェクト処理スクリプト
/// </summary>
public class FindKeyStatus : MonoBehaviour
{
    [Header("調査判定用")]
    public bool isChecked;        //調査済みか
    public float checkedTime;     //調査完了時間
    public bool canCheckArea;     //調査可能エリアにいるか

    [Header("調査UI表示切り替え用")]
    public Slider checkSlider;     //調査進捗UI
    float activationDistance = 1f; //UI表示可能距離
    public Vector3 player;         //プレイヤー座標

    [Header("VR用")]
    [SerializeField] private InputActionReference hold_L;           //入力を受け取る対象のAction(左)
    [SerializeField] private InputActionReference hold_R;           //（右）

    CheckableObjManager checkableObjManager;

    private void Awake()
    {
        if (hold_L == null) return;

        // performedコールバックのみを受け取る
        // 長押し判定になったらこのコールバックが呼ばれる
        hold_L.action.performed += OnHold;

        // 入力を受け取るためには必ず有効化する必要がある
        hold_L.action.Enable();

        if (hold_R == null) return;

        // performedコールバックのみを受け取る
        // 長押し判定になったらこのコールバックが呼ばれる
        hold_R.action.performed += OnHold;

        // 入力を受け取るためには必ず有効化する必要がある
        hold_R.action.Enable();
    }

    void Start()
    {
        checkableObjManager = GameObject.Find("CheckableObjManager").gameObject.GetComponent<CheckableObjManager>();    //マネージャー取得

        checkSlider = transform.GetChild(0).transform.GetChild(0).gameObject.transform.GetComponent<Slider>();
        checkSlider.maxValue = checkableObjManager.chackableTimer;    //調査完了時間を設定

        checkSlider.gameObject.SetActive(false);

        canCheckArea = false;
        isChecked = false;
        checkedTime = 0f;
    }

    void Update()
    {
        player = GameObject.Find("Main").gameObject.transform.position;   //調査プレイヤーの現在地取得

        float distance = Vector3.Distance(transform.position, player);    //距離計算

        //距離が指定した範囲内ならオブジェクトを表示、そうでなければ非表示
        if (distance <= activationDistance)
        {
            canCheckArea = true;   //調査可能に
            checkSlider.gameObject.SetActive(true);
        }
        else
        {
            canCheckArea = false;   //調査不可に
            checkSlider.gameObject.SetActive(false);
        }
        

        if (isChecked) return;   //調査済みの場合return

        checkSlider.value = checkedTime;

        //調査所要時間を超えた場合
        if(checkSlider.maxValue <= checkedTime)
        {
            isChecked = true;   //調査済みに変更
        }
    }

    // 長押しされたときに呼ばれるメソッド
    private void OnHold(InputAction.CallbackContext context)
    {
        Debug.Log("長押しされてるよ～");
    }
}
