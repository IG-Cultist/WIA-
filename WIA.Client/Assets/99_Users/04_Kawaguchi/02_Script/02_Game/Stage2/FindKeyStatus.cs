using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using KanKikuchi.AudioManager;
using Unity.VisualScripting;

/// <summary>
/// 鍵調査オブジェクト処理スクリプト
/// </summary>
public class FindKeyStatus : MonoBehaviour
{
    [Header("調査判定用")]
    public bool isChecked;        //調査済みか
    public bool isSearchedSE;
    public float checkedTime;     //調査完了時間
    public bool canCheckArea;     //調査可能エリアにいるか

    [Header("調査UI表示切り替え用")]
    public Slider checkSlider;     //調査進捗UI
    float activationDistance = 1.5f; //UI表示可能距離
    public Vector3 player;         //プレイヤー座標
    [SerializeField] Outline outline;
    private bool canCheck = false;

    private bool isLook1 = false;
    private bool isLook2 = false;
    CheckableObjManager checkableObjManager;


    void Start()
    {
        checkableObjManager = GameObject.Find("CheckableObjManager").gameObject.GetComponent<CheckableObjManager>();    //マネージャー取得

        checkSlider = transform.GetChild(0).transform.GetChild(0).gameObject.transform.GetComponent<Slider>();
        checkSlider.maxValue = checkableObjManager.chackableTimer;    //調査完了時間を設定

        checkSlider.gameObject.SetActive(false);

        canCheckArea = false;
        isChecked = false;
        isSearchedSE = false;
        checkedTime = 0f;
    }

    void Update()
    {
        if(isLook1 != isLook2)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;       //ここ前回

        }
        isLook2 = isLook1;

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
            if (!isChecked)
            {
                SEManager.Instance.Play(
                    audioPath: SEPath.SEARCHED, //再生したいオーディオのパス
                    volumeRate: 1,                //音量の倍率
                    delay: 0,                //再生されるまでの遅延時間
                    pitch: 1,                //ピッチ
                    isLoop: true,             //ループ再生するか
                    callback: null              //再生終了後の処理
                );
                isChecked = true;   //調査済みに変更
            }
        }
    }

    public void LookObject()
    {
        if (isLook1) isLook1 = false;
        else if (!isLook1) isLook1 = true;
    }
}
