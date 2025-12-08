//=================================
// タスクの進捗をチェックするスクリプト
// Aouther:y-miura
// Date:2025/11/06
//=================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KanKikuchi.AudioManager;


public class TaskCheck : MonoBehaviour
{
    public int boxCnt; //要素の変数
    int checkCnt = 5; //目標の要素数
    [SerializeField] List<int> cubeList = new List<int>(); //int型のList

    // プレイヤー
    private Player player;

    //死亡判定
    private bool isDead;

    private void Start()
    {
        if(RoomModel.Instance)
        player = OnlineGameManager.Player.gameObject.GetComponent<Player>();

        isDead = false;
    }

    private void Update()
    {
        //Debug.Log(player.deathCnt);
        if (player.deathCnt >= 3)
        {
            if (!isDead) Initiate.Fade("Exp_Worker_2", Color.black, 1.0f);
            isDead = true;
        }
    }

    /// <summary>
    /// エリアに触れた時の処理
    /// </summary>
    /// <param name="other">触れたオブジェクト</param>
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            SEManager.Instance.Play(
                audioPath: SEPath.SUCCESS, //再生したいオーディオのパス
                volumeRate: 1,                 //音量の倍率
                delay: 0,                      //再生されるまでの遅延時間
                pitch: 1,                      //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                 //再生終了後の処理
            );
            if(!RoomModel.Instance)
            {
                // 運搬した箱の数を加算
                boxCnt++;
                GameObject.Find("TaskCount").GetComponent<Text>().text = ": " + boxCnt + "/5";
                //リストに要素を追加する
                cubeList.Add(boxCnt);
            }

            if (RoomModel.Instance) await RoomModel.Instance.CountAsync(true);//木田晃輔が変更


            //箱を消す
            if (RoomModel.Instance) GameObject.Find("OnlineGameManager").
                GetComponent<OnlineGameManager>().DeliteSynObj(other.gameObject); //木田晃輔が変更

                Destroy(other.gameObject);


            if(!RoomModel.Instance)
            if (cubeList.Count >= checkCnt)
            {//要素数が目標数と同じになったら
             //フェードアウトしてシーン遷移
                Initiate.Fade("Exp_Worker_2", Color.black, 1.0f);
            }
        }
    }
}
