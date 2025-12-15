/// ------------------------------
/// デスクスクリプト
/// Author:Nishiura Date:25/11/19
/// ------------------------------
using UnityEngine;

public class Desk : MonoBehaviour
{
    // デリバリーマネージャ
    DeliveryManager deliveryManager;

    ResultScoreManager resultScoreManager;

    void Start()
    {
        // デリバリーマネージャをシーン内から取得
        deliveryManager = GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>();

        resultScoreManager = GameObject.Find("ResultScoreManager").GetComponent<ResultScoreManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Item" && other.transform.name =="Coffee")
        {
            if(resultScoreManager) resultScoreManager.successNum++;

            // コーヒー受け渡し処理を呼ぶ
            deliveryManager.ServeCoffee();
        }
    }
}
