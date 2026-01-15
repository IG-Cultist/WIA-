///////////////////////////////////////////////////////////////////////////////
///        オンライン通信を使ったゲームを動作させるスクリプト
///        
///　Aughtor：木田晃輔
///　更新日：1月14日
///　概要：Unityのステージのシーンに配置。！！オンライン環境のみ使用可能！！
///　
///////////////////////////////////////////////////////////////////////////////
using DG.Tweening;
using NUnit;
using NUnit.Framework;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Xml;
using Unity.Cinemachine;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Scripting;
using static Shared.Interfaces.StreamingHubs.EnumManager;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.SceneManagement;

public class OnlineGameManager : MonoBehaviour
{
    #region 基本
    [Header("基本設定")]
    PlayerData playerData;
    int tasks = 0;      //タスクの数
    [SerializeField] Transform spawnPointP1; //プレイヤー1の初期配置場所
    [SerializeField] Transform spawnPointP2; //プレイヤー2の初期配置場所
    private static Transform mainSpawnPoint; //操作プレイヤーの初期配置
    public static Transform MainSpawnPoint
    {
        get { return mainSpawnPoint; }
    }
    [SerializeField] GameObject mainPlayerPrefab; //操作プレイヤー
    [SerializeField] GameObject subPlayerPrefab; //非操作プレイヤー
    [SerializeField] GameObject objPrefab; //オブジェクト
    [SerializeField] List<GameObject> syncObjList;//同期用オブジェクト初期設定
    private static GameObject player;//操作プレイヤー
    public static GameObject Player
    {
        get { return player; }
    }
    GameObject subplayer; //非操作プレイヤー
    private static Dictionary<string,GameObject> objList = new Dictionary<string, GameObject>(); //生成オブジェクトリスト
    public static Dictionary<string,GameObject> ObjList 
    {
        get { return objList; } 
    }
    private static string spawnObjId;//生成オブジェクトID
    public static string SpawnObjId
    {
        get { return spawnObjId; }
    }

    private int TaskCnt; //タスクカウント
    private int potCount;
    private bool isGetKey = false; //鍵を持っている
    #endregion

    private void Awake()
    {
        objList = new Dictionary<string, GameObject>();

        if (RoomModel.Instance.IsMaster == false)
        {
            //すべてのオブジェクトからRigidbodyを外す
            foreach (var obj in syncObjList)
            {
                Destroy(obj.GetComponent<Rigidbody>());
            }
        }

        //通知の設定
        RoomModel.Instance.OnUpdatePlayerSyn += OnUpdatePlayerSyn;
        RoomModel.Instance.OnSpawnedObjectSyn += OnSpawnedObjectSyn;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
        RoomModel.Instance.OnUpdatedObject += OnUpdatedObject;
        RoomModel.Instance.OnOwnershipSwapObjectSyn += OnOwnershipSwapObjectSyn;
        RoomModel.Instance.OnCounted += this.OnCounted;
        RoomModel.Instance.OnDeliteObjectSyn += this.OnDeliteObjectSyn;
        //RoomModel.Instance.OnPlayerDeadSyn += this.OnPlayerDeadSyn;
        //RoomModel.Instance.OnPlayerRespownSyn += this.OnPlayerRespownSyn;


        //プレイヤーの設定
        foreach (var user in RoomModel.Instance.joinedUserList)
        {
            if (user.Key == RoomModel.Instance.ConnectionId)
            {
                player = Instantiate(mainPlayerPrefab);
                if (RoomModel.Instance.joinedUserList[user.Key].JoinOrder == 1)
                {//働く方リスポーン
                    player.name = "Worker";
                    player.transform.position = spawnPointP1.position;
                    switch (SceneManager.GetActiveScene().name)
                    {
                        case "Stage_1":
                            Destroy(GameObject.Find("Crane").GetComponent<Rigidbody>());
                            Destroy(GameObject.Find("Hook").GetComponent<Rigidbody>());
                            syncObjList.Add(GameObject.Find("Crane"));
                            syncObjList.Add(GameObject.Find("Hook"));
                            player.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                            break;
                        case "Stage_2":
                            player.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                            break;
                        case "Stage_3":
                            player.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                            break;

                    }
                }
                else if (RoomModel.Instance.joinedUserList[user.Key].JoinOrder == 2)
                {//労災側リスポーン
                    player.name = "Stricker";
                    player.transform.position = spawnPointP2.position;
                    player.transform.parent = spawnPointP2.transform;
                    switch (SceneManager.GetActiveScene().name)
                    {
                        case "Stage_1":
                            syncObjList.Add(GameObject.Find("Crane"));
                            syncObjList.Add(GameObject.Find("Hook"));
                            player.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                            break;
                        case "Stage_2":
                            player.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                            break;
                        case "Stage_3":
                            player.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                            break;
                    }
                }
                mainSpawnPoint = player.transform;
                InvokeRepeating("UpDatePlayer", 0.1f, 0.1f);
                if (SceneManager.GetActiveScene().name == "Stage_1")
                    player.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    player.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                    player.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                subplayer = Instantiate(subPlayerPrefab);
                if (RoomModel.Instance.joinedUserList[user.Key].JoinOrder == 1)
                {//働く方リスポーン
                    subplayer.name = "Worker";
                    subplayer.transform.position = spawnPointP2.position;
                    subplayer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                }
                else if (RoomModel.Instance.joinedUserList[user.Key].JoinOrder == 2)
                {//労災側リスポーン
                    subplayer.name = "Stricker";
                    subplayer.transform.position = spawnPointP1.position;
                    subplayer.transform.parent = spawnPointP1.transform;
                    subplayer.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                }
            }
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (RoomModel.Instance)
        {//オンラインだったら

            //オブジェクト更新を行う
            InvokeRepeating("UpdateObj", 0.1f, 0.1f);

        }
    }

    private void OnDisable()
    {
        //通知の削除
        RoomModel.Instance.OnUpdatePlayerSyn -= OnUpdatePlayerSyn;
        RoomModel.Instance.OnSpawnedObjectSyn -= OnSpawnedObjectSyn;
        RoomModel.Instance.OnLeavedUser -= OnLeavedUser;
        RoomModel.Instance.OnUpdatedObject -= OnUpdatedObject;
        RoomModel.Instance.OnOwnershipSwapObjectSyn -= OnOwnershipSwapObjectSyn;
        RoomModel.Instance.OnCounted -= this.OnCounted;
        RoomModel.Instance.OnDeliteObjectSyn -= this.OnDeliteObjectSyn;
        //RoomModel.Instance.OnPlayerDeadSyn -= this.OnPlayerDeadSyn;
        //RoomModel.Instance.OnPlayerRespownSyn -= this.OnPlayerRespownSyn;

        //値初期化
        TaskCnt = 0;
        isGetKey = false;
    }

    /// <summary>
    /// 同期オブジェクトの取得
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetSynObj()
    {
        return syncObjList;
    }

    /// <summary>
    /// 同期オブジェクトの更新
    /// </summary>
    /// <param name="gameObject"></param>
    public async void DeliteSynObj(GameObject gameObject , string tag)
    {
        await RoomModel.Instance.DeliteObjectAsync(gameObject.name,tag);
    }

    /// <summary>
    /// オブジェクト生成
    /// </summary>
    public async void SpawnObj(Vector3 spawnPos)
    {
        await RoomModel.Instance.SpawnObjectAsync(spawnPos);
    }

    /// <summary>
    /// オブジェクト更新
    /// </summary>
    public async void UpdateObj()
    {
        if (objList != null)
        {
            foreach (var obj in objList)
            {
                if(obj.Value == null)
                {
                    Debug.Log("null");
                    continue;
                }
                if (obj.Value.GetComponent<Rigidbody>() == null) continue;
                await RoomModel.Instance.UpdateObjectAsync(obj.Value.transform.position, obj.Value.transform.rotation, obj.Key);
            }
        }
        if(syncObjList != null)
        {
            for (int i = 0; i < syncObjList.Count; i++)
            {
                if (syncObjList[i].GetComponent<Rigidbody>() == null) continue;
                await RoomModel.Instance.UpdateObjectAsync(syncObjList[i].transform.localPosition,
                    syncObjList[i].transform.rotation, i.ToString());
            }
        }
    }

    /// <summary>
    /// オブジェクトの所有権変更
    /// </summary>
    public async void ObjectOwnershipSwap(string uniqueId,int joinOrder)
    {
        await RoomModel.Instance.ObjectOwnershipSwapAsync(uniqueId, joinOrder);
    }

    /// <summary>
    /// プレイヤーの更新
    /// </summary>
    public async void UpDatePlayer()
    {
        await RoomModel.Instance.UpdatePlayerAsync(player.transform.position,
            player.transform.rotation,
            GameObject.Find(player.name).GetComponent<Player>().plaAnimation.animator.GetInteger("AnimID"),
            GameObject.Find(player.name).GetComponent<Player>().plaAnimation.animator.GetFloat("MoveSpeed"));
    }

    /// <summary>
    /// プレイヤー更新通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    void OnUpdatePlayerSyn(Vector3 pos,Quaternion rot, int animState,float moveSpeed)
    {
        Rigidbody rb = subplayer.GetComponent<Rigidbody>();

        rb.DOMove(pos, 0.1f).SetEase(Ease.Linear);
        rb.transform.GetChild(0).DOMove(pos, 0.1f).SetEase(Ease.Linear);

        subplayer.transform.DORotate(rot.eulerAngles, 0.1f);
        subplayer.transform.GetChild(0).DORotate(rot.eulerAngles, 0.1f);
        subplayer.GetComponent<Player>().plaAnimation.anim_State = (PlayerAnimation.ANIM_STATE)animState;
        Debug.Log("現在のアニメーションは" + animState.ToString());
        subplayer.GetComponent<Player>().subMoveSpeed = moveSpeed;
    }

    /// <summary>
    /// オブジェクト生成通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    void OnSpawnedObjectSyn(Vector3 pos, string id)
    {
        spawnObjId = id;
        GameObject gameObject = Instantiate(objPrefab);
        gameObject.name = gameObject.name + potCount;
        gameObject.transform.position = pos;
        if(gameObject == null)
        {
            Debug.Log("Nullオブジェクト");
        }
        objList.Add(id, gameObject);
        switch (SceneManager.GetActiveScene().name)
        {
            case "Stage_2":
                FlowerPotManager flowerPotManager = GameObject.Find("FlowerPotManager").GetComponent<FlowerPotManager>();
                flowerPotManager.potList.Add(gameObject);
                potCount++;
                if(Player.name == "Worker")
                {
                    Destroy(gameObject.GetComponent<Rigidbody>());
                }
                break;
        }
        CancelInvoke("UpdateObj");
        //オブジェクト更新を行う
        InvokeRepeating("UpdateObj", 0.1f, 0.1f);
    }

    /// <summary>
    /// オブジェクト更新通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    void OnUpdatedObject(Vector3 pos,Quaternion rot,string id)
    {
        foreach (var obj in objList)
        {
            if(obj.Key == id)
            {
                obj.Value.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
                obj.Value.transform.DORotate(rot.eulerAngles, 0.1f);
            }
        }
        for (int i = 0;i<syncObjList.Count;i++)
        {
            if (i.ToString() == id)
            {
                syncObjList[i].transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
                syncObjList[i].transform.DORotate(rot.eulerAngles, 0.1f);
            }
        }
    }

    void OnDeliteObjectSyn(string objName, string tag)
    {
        if(objList != null)
        {
            foreach (var obj in objList)
            {
                if (obj.Value.name != objName) continue;
                objList.Remove(obj.Key);
                if(SceneManager.GetActiveScene().name == "Stage_2")
                {
                    FlowerPotManager flowerPotManager = GameObject.Find("FlowerPotManager").GetComponent<FlowerPotManager>();
                    flowerPotManager.PotLost(GameObject.Find(objName));
                    if(tag == "Player"&&Player.name == "Worker")
                    {
                        player.GetComponent<Player>().OnlineDeath();
                    }
                }
                else
                {
                    Destroy(GameObject.Find(objName));
                }
                CancelInvoke("UpdateObj");
                //オブジェクト更新を行う
                InvokeRepeating("UpdateObj", 0.1f, 0.1f);
                break;
            }
        }
        if(syncObjList != null)
        {
            foreach (var syncObj in syncObjList)
            {
                if (syncObj.name != objName) continue;
                syncObjList.Remove(syncObj);
                Destroy(GameObject.Find(objName));
                break;
            }
        }
    }

    /// <summary>
    /// オブジェクト所有権変更通知
    /// </summary>
    /// <param name="uniqueId"></param>
    /// <param name="joinOrder"></param>
    void OnOwnershipSwapObjectSyn(string uniqueId,int joinOrder)
    {
        //既存オブジェクトの場合
        for (int i = 0; i < syncObjList.Count; i++)
        {
            if (i.ToString() == uniqueId)
            {
                if (joinOrder == RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder)
                {
                    syncObjList[i].AddComponent<Rigidbody>();
                    Debug.Log("オブジェクトの権限を得ました");
                    if (syncObjList[i].name == "MovablePartition")
                    {
                        syncObjList[i].GetComponent<Collider>().enabled = false;
                    }
                    return;
                }
                else if(joinOrder != RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder)
                {
                    Destroy(syncObjList[i].GetComponent<Rigidbody>());
                    Debug.Log("オブジェクトの権限を失いました");
                    if (syncObjList[i].name == "MovablePartition")
                    {
                        syncObjList[i].GetComponent<Collider>().enabled = true;
                    }
                    return;
                }
            }
        }

        //生成オブジェクトの場合
        if (joinOrder == RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder)
        {
            ObjList[uniqueId].AddComponent<Rigidbody>();
            Debug.Log("オブジェクトの権限を得ました");
            return;
        }
        else
        {
            Destroy(ObjList[uniqueId].GetComponent<Rigidbody>());
            Debug.Log("オブジェクトの権限を失いました");
            return;
        }
    }

    void OnCounted(bool isTask)
    {
        switch (isTask)
        {
            case true:
                // タスク回数を加算
                TaskCnt++;
                switch (SceneManager.GetActiveScene().name)
                {
                    case "Stage_1":
                        // タスク完了回数テキストを取得し。現在のシーンに応じて回数を反映
                        GameObject.Find("TaskCount").GetComponent<Text>().text = ": " + TaskCnt + "/5";
                        if (TaskCnt >= 5)
                        {//要素数が目標数と同じになったら
                         //フェードアウトしてシーン遷移
                            if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 1)
                            {
                                Initiate.Fade("01_Exp_Worker_2", Color.black, 1.0f);
                            }
                            else if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 2)
                            {
                                Initiate.Fade("01_Exp_Stricker_2", Color.black, 1.0f);
                            }
                        }
                        break;
                    case "Stage_2":                   
                        if (TaskCnt >= 1 && isGetKey)
                        {
                            //フェードアウトしてシーン遷移
                            if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 1)
                            {
                                Initiate.Fade("01_Exp_Worker_3", Color.black, 1.0f);
                            }
                            else if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 2)
                            {
                                Initiate.Fade("01_Exp_Stricker_3", Color.black, 1.0f);
                            }
                        }
                        else
                        {
                            isGetKey = true;
                            // タスク完了回数テキストを取得し。現在のシーンに応じて回数を反映
                            GameObject.Find("TaskCount").GetComponent<Text>().text = ": " + TaskCnt + "/1";
                        }
                        break;
                    case "Stage_3":
                        // タスク完了回数テキストを取得し。現在のシーンに応じて回数を反映
                        GameObject.Find("TaskCount").GetComponent<Text>().text = ": " + TaskCnt + "/5";
                        if (TaskCnt >= 5)
                        {
                            Initiate.Fade("ResultScene", Color.black, 1.0f);
                        }
                        break;
                }
                break;
            case false:
                // 死亡回数を加算
                GameObject.Find(player.name).GetComponent<Player>().deathCnt++;
                // 死亡回数テキストを取得し、死亡回数を反映
                GameObject.Find("DeathCount").GetComponent<Text>().text = 
                    ": " +
                    GameObject.Find(player.name).GetComponent<Player>().deathCnt +
                    "/3";
                break;
        }
    }


    /// <summary>
    /// 退室通知
    /// </summary>
    /// <param name="joinedUser"></param>
    void OnLeavedUser(JoinedUser joinedUser)
    {
        Debug.Log(joinedUser.UserData.Name+"が退室しました。");
    }

    //void OnPlayerDeadSyn(Guid guid)
    //{
    //    if (RoomModel.Instance.ConnectionId == guid)
    //        subplayer.GetComponent<Player>().plaAnimation.enabled = false;
    //}

    //void OnPlayerRespownSyn(Guid guid)
    //{
    //    if (RoomModel.Instance.ConnectionId == guid)
    //        subplayer.GetComponent<Player>().plaAnimation.enabled = true;
    //}
}
