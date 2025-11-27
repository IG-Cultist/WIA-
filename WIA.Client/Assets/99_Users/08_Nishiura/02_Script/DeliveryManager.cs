/// ------------------------------
/// デリバリーマネージャー
/// Author:Nishiura Date:25/11/10
/// ------------------------------
using DG.Tweening;
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

    // 配達済みデスクリスト
    List<int> servedDeskList = new List<int>();

    // コーヒーのゲームオブジェクト
    GameObject coffeeObj;

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

    private void Start()
    {
        player = GameObject.Find("Main").gameObject.GetComponent<Player>();
        isDead = false;
        coolDownSlider.SetActive(false);
    }

    private void Update()
    {
        //if (isCooldown)
        //{
        //    coolDownSlider.value -= 0.0015f;   //調査時間加算
        //}

        if (player.deathCnt >= 3)
        {
            if(!isDead) Initiate.Fade("30_ResultScene", Color.black, 1.0f);
            isDead = true;
        }
    }

    /// <summary>
    /// コーヒー生成処理
    /// </summary>
    public void DripCoffee()
    {
        // まだコーヒーを生成していない場合または、クールダウン中でない場合、コーヒーを生成する
        if (isCreated || isCooldown) return;

        // デスク番号がユニークなものになるまでループ
        while (true)
        {
            // ランダムな数値を生成
            deskNum = Random.Range(0, deskList.Count);
            // 生成された数値がすでに届けられたデスク番号でない場合、ループを抜ける
            if (!servedDeskList.Contains(deskNum)) break;
        }
        // 生成済みとする
        isCreated = true;
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
        // コーヒーを生成する
        coffeeObj = Instantiate(coffeePrefabs);
        coffeeObj.transform.position = new Vector3(-17f, 0.44f, 2.5f);
    }

    /// <summary>
    /// コーヒー受け渡し処理
    /// </summary>
    public void ServeCoffee()
    {
        if (!isCreated) return; // コーヒーがない場合、処理しない
        isCreated = false;  // 未生成とする

        // コーヒーマシンを使用可能にする
        coffeeMachine.GetComponent<BoxCollider>().enabled = true;

        // 受け渡し済みリストにデスク番号を入れる
        servedDeskList.Add(deskNum);

        // デスクにコーヒーを表示し、コーヒー要求アイコンを消去
        deskList[deskNum].transform.GetChild(0).gameObject.SetActive(true);
        deskList[deskNum].transform.GetChild(1).gameObject.SetActive(false);
        deskList[deskNum].transform.GetComponent<BoxCollider>().enabled = false;

        // 手元のコーヒーオブジェクトを破棄
        Destroy(coffeeObj);

        // 配達完了数を加算
        deliveredCount++;
        GameObject.Find("TaskCount").GetComponent<Text>().text = ": " + deliveredCount + "/5";

        if (deliveredCount >=5) GoNextStage(); // 5の場合、次のシーンへ移動
    }

    /// <summary>
    /// コーヒー紛失処理
    /// </summary>
    public void LostCoffee()
    {
        // 手元のコーヒーオブジェクトを破棄
        Destroy(coffeeObj);

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
        Initiate.Fade("30_ResultScene", Color.black, 1.0f);   // フェード時間1秒
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
