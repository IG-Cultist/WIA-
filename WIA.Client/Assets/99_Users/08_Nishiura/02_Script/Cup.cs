/// ------------------------------
/// コップマネージャー
/// Author:Nishiura Date:25/12/01
/// ------------------------------
using KanKikuchi.AudioManager;
using UnityEngine;

public class Cup : MonoBehaviour
{
    [SerializeField] GameObject waterPrefab;

    /// <summary>
    /// 通信用
    /// </summary>
     
    OnlineGameManager gameManager;

    private void Start()
    {
        if(RoomModel.Instance) gameManager = GameObject.Find("OnlineGameManager").GetComponent<OnlineGameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground" || collision.transform.tag == "Trap")
        {
            KillMyself();
        }
    }

    /// <summary>
    /// 自己破壊処理
    /// </summary>
    public void KillMyself()
    {
        GameObject waterObj = waterPrefab;

        if (RoomModel.Instance)
        {
            waterObj.transform.position = new Vector3(this.gameObject.transform.position.x, -0.47f, this.gameObject.transform.position.z);
            waterObj.transform.rotation = waterPrefab.transform.rotation;
        }
        else
        {
            Instantiate(waterPrefab, new Vector3(this.gameObject.transform.position.x, -0.47f, this.gameObject.transform.position.z), waterPrefab.transform.rotation);
        }
        SEManager.Instance.Play(
            audioPath: SEPath.GLASS_CRASH_2, //再生したいオーディオのパス
             volumeRate: 1,                 //音量の倍率
             delay: 0,                      //再生されるまでの遅延時間
             pitch: 1,                      //ピッチ
             isLoop: false,                 //ループ再生するか
             callback: null                 //再生終了後の処理
            );

        if (RoomModel.Instance)
        {
            gameManager.SpawnObj(waterObj.transform.position);
            gameManager.DeliteSynObj(this.gameObject, this.gameObject.tag);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
