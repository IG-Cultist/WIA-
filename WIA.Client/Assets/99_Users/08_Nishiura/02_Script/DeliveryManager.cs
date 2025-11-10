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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            deskList[0].transform.GetChild(0).gameObject.SetActive(true);

            // 配達完了数が5未満の場合完了数を加算
            if(deliveredCount < 4) deliveredCount++;
            else GoNextStage(); // 5の場合、次のシーンへ移動
        }


        if (Input.GetKeyDown(KeyCode.F) && !isCreated)
        {
            isCreated = true;
            coffeeObj = Instantiate(coffeePrefabs);
            coffeeObj.transform.position = new Vector3 (-17f, 0.44f, 2.5f);
        }
    }

    void GoNextStage()
    {
        Initiate.DoneFading();
        Initiate.Fade("03_GameScene", Color.black, 1.0f);   // フェード時間1秒
    }
}
