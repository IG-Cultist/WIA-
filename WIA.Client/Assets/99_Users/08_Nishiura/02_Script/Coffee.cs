/// ------------------------------
/// コーヒースクリプト
/// Author:Nishiura Date:25/11/10
/// ------------------------------
using UnityEngine;
using DG.Tweening;

public class Coffee : MonoBehaviour
{
    [SerializeField] GameObject sadCoffeePrefab;

    // デリバリーマネージャ
    DeliveryManager deliveryManager;

    private void Start()
    {
        // デリバリーマネージャをシーン内から取得
        deliveryManager = GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground" || collision.transform.tag == "Trap")
        {
            GameObject spilledCoffee = Instantiate(sadCoffeePrefab, new Vector3(this.gameObject.transform.position.x, -0.485f, this.gameObject.transform.position.z), sadCoffeePrefab.transform.rotation);
            spilledCoffee.name = "SadCoffee";
            // コーヒー紛失処理を呼ぶ
            deliveryManager.LostCoffee();
        }
    }
}
