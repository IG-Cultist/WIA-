/// ------------------------------
/// デリバリーマネージャー
/// Author:Nishiura Date:25/11/10
/// ------------------------------
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    // デスクのプレハブのリスト
    [SerializeField] List<GameObject> deskList;

    // コーヒーのプレハブ
    [SerializeField] GameObject coffeePrefabs;

    // 配達完了カウント
    int deliveredCount = 0;

    GameObject coffeeObj;

    // コーヒー生存判定
    public bool isCreated = false;

    int deskNum;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isCreated)
        {
            // ランダムな数値を生成
            deskNum = Random.Range(0, deskList.Count);
            // 生成した数値のデスクを指定し、コーヒー要求アイコンを表示
            deskList[deskNum].transform.GetChild(1).gameObject.SetActive(true);

            // 生成済みとする
            isCreated = true;
            // コーヒーを生成する
            coffeeObj = Instantiate(coffeePrefabs);
            coffeeObj.transform.position = new Vector3 (-17f, 0.44f, 2.5f);
        }
    }

    /// <summary>
    /// コーヒー受け渡し処理
    /// </summary>
    public void ServeCoffee()
    {
        if (!isCreated) return; // コーヒーがない場合、処理しない
        isCreated = false;  // 未生成とする

        // デスクにコーヒーを表示し、コーヒー要求アイコンを消去
        deskList[deskNum].transform.GetChild(0).gameObject.SetActive(true);
        deskList[deskNum].transform.GetChild(1).gameObject.SetActive(false);

        // 手元のコーヒーオブジェクトを破棄
        Destroy(coffeeObj);

        // 配達完了数が5未満の場合完了数を加算
        if (deliveredCount < 4) deliveredCount++;
        else GoNextStage(); // 5の場合、次のシーンへ移動
    }

    /// <summary>
    /// コーヒー紛失処理
    /// </summary>
    public void LostCoffee()
    {
        // 未生成とする
        isCreated = false;  
        // コーヒー要求アイコンを消去
        deskList[deskNum].transform.GetChild(1).gameObject.SetActive(false);
    }

    void GoNextStage()
    {
        Initiate.DoneFading();
        Initiate.Fade("03_GameScene", Color.black, 1.0f);   // フェード時間1秒
    }
}
