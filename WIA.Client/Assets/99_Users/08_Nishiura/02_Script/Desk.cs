/// ------------------------------
/// デスクスクリプト
/// Author:Nishiura Date:25/11/19
/// ------------------------------
using UnityEngine;

public class Desk : MonoBehaviour
{
    // デリバリーマネージャ
    DeliveryManager deliveryManager;

    void Start()
    {
        // デリバリーマネージャをシーン内から取得
        deliveryManager = GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Item" && other.transform.name =="Coffee")
        {
            // コーヒー受け渡し処理を呼ぶ
            deliveryManager.ServeCoffee();
        }
    }
}
