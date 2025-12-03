using DG.Tweening;
using KanKikuchi.AudioManager;
/// ------------------------------
/// コーヒースクリプト
/// Author:Nishiura Date:25/11/10
/// ------------------------------
using UnityEngine;

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

            SEManager.Instance.Play(
                audioPath: SEPath.GLASS_CRASH_2, //再生したいオーディオのパス
                 volumeRate: 1,                 //音量の倍率
                 delay: 0,                      //再生されるまでの遅延時間
                 pitch: 1,                      //ピッチ
                 isLoop: false,                 //ループ再生するか
                 callback: null                 //再生終了後の処理
                );
                         // コーヒー紛失処理を呼ぶ
            deliveryManager.LostCoffee();
        }
    }
}
