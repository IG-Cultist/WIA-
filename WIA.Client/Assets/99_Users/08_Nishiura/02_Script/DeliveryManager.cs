/// ------------------------------
/// デリバリーマネージャー
/// Author:Nishiura Date:25/11/10
/// ------------------------------
using DG.Tweening;
using KanKikuchi.AudioManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    // デスクのプレハブのリスト
    [SerializeField] List<GameObject> deskList;

    // コーヒーのプレハブ
    [SerializeField] GameObject coffeePrefabs;

    // コーヒーマシンのオブジェクト
    [SerializeField] GameObject coffeeMachine;

    // クールダウン用スライダー
    [SerializeField] GameObject coolDownSlider;

    // コーヒーを淹れる音
    [SerializeField] AudioClip brewCoffee;

    // コーヒーを置く音
    [SerializeField] AudioClip PutCoffee;

    // 配達済みデスクリスト
    List<int> servedDeskList = new List<int>();

    // コーヒーのゲームオブジェクト
    public  GameObject coffeeObj;

    // 配達完了カウント
    int deliveredCount = 0;

    // コーヒー生存判定
    public bool isCreated = false;

    // クールダウン中判定
    bool isCooldown = false;

    // コーヒー要求デスク番号
    int deskNum;

    // プレイヤー
    private Player player;

    // 死亡判定
    private bool isDead;


    AudioSource source; //オーディオソース
    private ResultScoreManager resultScoreManager;
    private void Start()
    {
        if(RoomModel.Instance)
        {
            player = GameObject.Find(OnlineGameManager.Player.name).gameObject.GetComponent<Player>(); // シーン内のプレイヤーからスクリプトを取得
        }
        else
        {
            player = GameObject.FindWithTag("Player").gameObject.GetComponent<Player>(); // シーン内のプレイヤーからスクリプトを取得
        }
        resultScoreManager = GameObject.Find("ResultScoreManager").GetComponent<ResultScoreManager>();

        isDead = false; //死んでいない状態にする
        coolDownSlider.SetActive(false);    //クールダウンスライダーを非表示にする
        source = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (player.deathCnt >= 5)
        {
            if (!isDead)
            {
                SEManager.Instance.Play(
                    audioPath: SEPath.TASK_FAILURE, //再生したいオーディオのパス
                    volumeRate: 1,                //音量の倍率
                    delay: 1,                //再生されるまでの遅延時間
                    pitch: 1,                //ピッチ
                    isLoop: false,             //ループ再生するか
                    callback: null              //再生終了後の処理
                );

                Initiate.Fade("30_ResultScene", Color.black, 1.0f);
            }
            isDead = true;
        }
        if ((GameObject.Find("TaskUIManager").GetComponent<TaskUIManager>().limitCount <= 0))
        {
            GoNextStage();
        }
    }

    /// <summary>
    /// コーヒー生成要求処理
    /// </summary>
    public void RequestCoffee()
    {
        // まだコーヒーを生成していない場合または、クールダウン中でない場合、コーヒーを生成する
        if (isCreated || isCooldown) return;
        // 生成済みとする
        isCreated = true;

        source.PlayOneShot(brewCoffee);
        Invoke("DripCoffee",1f);
        GameObject.Find("FadeImage").GetComponent<Image>().DOFade(1f, 1f);
    }

    /// <summary>
    /// コーヒー生成処理
    /// </summary>
    async void DripCoffee()
    {
        GameObject.Find("FadeImage").GetComponent<Image>().DOFade(0f, 1f);
        // デスク番号がユニークなものになるまでループ
        while (true)
        {
            // ランダムな数値を生成
            deskNum = Random.Range(0, deskList.Count);
            // 生成された数値がすでに届けられたデスク番号でない場合、ループを抜ける
            if (!servedDeskList.Contains(deskNum)) break;
        }

        // コーヒーを生成する

        if (RoomModel.Instance)
        {
            coffeeObj = coffeePrefabs;
            coffeeObj.name = coffeePrefabs.name;
            coffeeObj.transform.position = new Vector3(-17f, 0.44f, 2.5f);
            await RoomModel.Instance.SpawnItemAsync(2, coffeeObj.transform.position);
        }
        else
        {
            coffeeObj = Instantiate(coffeePrefabs);
            coffeeObj.name = coffeePrefabs.name;
            coffeeObj.transform.position = new Vector3(-17f, 0.44f, 2.5f);
        }

        isCooldown = true;
        coolDownSlider.SetActive(true);
        coolDownSlider.GetComponent<Slider>().value = 1f;
        coolDownSlider.GetComponent<Slider>().DOValue(0, 10f).SetEase(Ease.Linear);
        // 10秒後、クールダウン終了
        Invoke("ResetCooldown", 10f);

        // コーヒーマシンを使用不可にする
        coffeeMachine.GetComponent<BoxCollider>().enabled = false;
        // 生成した数値のデスクを指定し、コーヒー要求アイコンを表示
        deskList[deskNum].transform.GetChild(1).gameObject.SetActive(true);
        deskList[deskNum].transform.GetComponent<BoxCollider>().enabled = true;

    }

    /// <summary>
    /// コーヒー受け渡し処理
    /// </summary>
    public async void ServeCoffee()
    {
        if (!isCreated) return; // コーヒーがない場合、処理しない
        isCreated = false;  // 未生成とする
        player.isHave = false;
        source.PlayOneShot(PutCoffee);

        // コーヒーマシンを使用可能にする
        coffeeMachine.GetComponent<BoxCollider>().enabled = true;

        // 受け渡し済みリストにデスク番号を入れる
        servedDeskList.Add(deskNum);

        // デスクにコーヒーを表示し、コーヒー要求アイコンを消去
        deskList[deskNum].transform.GetChild(0).gameObject.SetActive(true);
        deskList[deskNum].transform.GetChild(1).gameObject.SetActive(false);
        deskList[deskNum].transform.GetComponent<BoxCollider>().enabled = false;

        // 手元のコーヒーオブジェクトを破棄
        if (RoomModel.Instance)
        {
            GameObject.Find("OnlineGameManager").GetComponent<OnlineGameManager>().isDelivery = true;
            await RoomModel.Instance.DeliteObjectAsync(coffeeObj.name, coffeeObj.tag);
        }
        else
        {
            Destroy(coffeeObj);
        }

        // 配達完了数を加算
        deliveredCount++;
        if (resultScoreManager) resultScoreManager.carelesslyNum++;

        if (RoomModel.Instance)
        {
            await RoomModel.Instance.CountAsync(true);
        }
        else
        {
            GameObject.Find("TaskCount").GetComponent<Text>().text = ": " + deliveredCount + "/5";

            GameObject.Find("TaskUIManager").GetComponent<TaskUIManager>().FluctNowTime(10);

            if (deliveredCount >= 5)
            {
                GameObject.Find("TaskUIManager").GetComponent<TaskUIManager>().isFinish = true;
                if (resultScoreManager) resultScoreManager.GetClearTime();
                GoNextStage(); // 5の場合、次のシーンへ移動
            }
        }
    }

    /// <summary>
    /// コーヒー紛失処理
    /// </summary>
    public async void LostCoffee()
    {
        // 手元のコーヒーオブジェクトを破棄
        if(RoomModel.Instance)
        {
            await RoomModel.Instance.DeliteObjectAsync(coffeeObj.name,coffeeObj.tag);
        }
        else
        {
            Destroy(coffeeObj);
            GameObject.Find("TaskUIManager").GetComponent<TaskUIManager>().FluctNowTime(-5);
        }

        if (resultScoreManager) resultScoreManager.carelesslyNum++;

        // コーヒーマシンを使用可能にする
        coffeeMachine.GetComponent<BoxCollider>().enabled = true;
        // 未生成とする
        isCreated = false;  
        // コーヒー要求アイコンを消去
        deskList[deskNum].transform.GetChild(1).gameObject.SetActive(false);
        deskList[deskNum].transform.GetComponent<BoxCollider>().enabled = false;
    }

    void GoNextStage()
    {
        Initiate.DoneFading();
        if (UnityEngine.XR.XRSettings.isDeviceActive) Initiate.Fade("VR_30_ResultScene", Color.black, 1.0f);   // フェード時間1秒
        else Initiate.Fade("30_ResultScene", Color.black, 1.0f);
    }

    /// <summary>
    /// クールダウン終了処理
    /// </summary>
    void ResetCooldown()
    {
        isCooldown = false;
        coolDownSlider.SetActive(false);
    }
}
