/// ------------------------------
/// コップマネージャー
/// Author:Nishiura Date:25/12/01
/// ------------------------------
using KanKikuchi.AudioManager;
using UnityEngine;

public class Cup : MonoBehaviour
{
    [SerializeField] GameObject waterPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground" || collision.transform.tag == "Trap")
        {
            Instantiate(waterPrefab, new Vector3(this.gameObject.transform.position.x, -0.47f, this.gameObject.transform.position.z), waterPrefab.transform.rotation);
            SEManager.Instance.Play(
                audioPath: SEPath.GLASS_CRASH_2, //再生したいオーディオのパス
                 volumeRate: 1,                 //音量の倍率
                 delay: 0,                      //再生されるまでの遅延時間
                 pitch: 1,                      //ピッチ
                 isLoop: false,                 //ループ再生するか
                 callback: null                 //再生終了後の処理
                );
            Destroy(this.gameObject);
        }
    }
}
