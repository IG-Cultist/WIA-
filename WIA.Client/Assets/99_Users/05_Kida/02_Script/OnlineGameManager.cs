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
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Unity.Loading;

public class OnlineGameManager : MonoBehaviour
{
    #region 基本
    [Header("基本設定")]

    PlayerData playerData;

    [SerializeField] Transform spawnPointP1; //プレイヤー1の初期配置場所
    [SerializeField] Transform spawnPointP2; //プレイヤー2の初期配置場所
    [SerializeField] GameObject mainPlayerPrefab; //操作プレイヤー
    [SerializeField] GameObject mainPlayerVRPrefab; //VR操作プレイヤー
    [SerializeField] GameObject subPlayerPrefab; //非操作プレイヤー
    [SerializeField] GameObject objPrefab; //オブジェクト
    [SerializeField] GameObject vrObjPrefab; //VRオブジェクト
    [SerializeField] GameObject coffeePrefab; //コーヒー
    [SerializeField] GameObject coffeeLostPrefab; //こぼれたコーヒー
    [SerializeField] GameObject InjectorPrefab; //注射器
    [SerializeField] GameObject cupPrefab; //水入りコップ
    [SerializeField] public List<GameObject> syncObjList;//同期用オブジェクト初期設定
    [SerializeField] List<GameObject> syncCraneList;//クレーン用同期リスト

    private static GameObject player;//操作プレイヤー
    GameObject pot;

    private ResultScoreManager resultScoreManager;

    public static GameObject Player
    {
        get { return player; }
    }
    private static Transform mainSpawnPoint; //操作プレイヤーの初期配置
    public static Transform MainSpawnPoint
    {
        get { return mainSpawnPoint; }
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


    public bool isDelivery = false;

    ItemBox itemBox; //アイテムボックス
    private int tasks = 0;      //タスクの数
    private int TaskCnt; //タスクカウント
    private int potCount;
    private int cupCount;
    private int injecterCount;
    private int coffeeCount;
    private bool isGetKey = false; //鍵を持っている
    private string craneid = "shfkuiuiasf";
    private string hookid = "loogaklghhsdhgoas";
    #endregion

    private void Awake()
    {
        objList = new Dictionary<string, GameObject>();

        if (RoomModel.Instance.IsMaster == true)
        {
            //Stage3限定アイテムからRigidbodyを外す
            foreach (var obj in syncObjList)
            {
                if (obj.tag == "Item" && SceneManager.GetActiveScene().name == "Stage3") Destroy(obj.GetComponent<Rigidbody>());
            }
        }
        if (RoomModel.Instance.IsMaster == false)
        {
            //アイテム以外からRigidbodyを外す
            foreach (var obj in syncObjList)
            {
                if (obj.tag == "Item" && SceneManager.GetActiveScene().name == "Stage3") continue;
                if (obj.name.Contains("coffeTable")) continue;
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
        RoomModel.Instance.OnSpawnItemSyn += this.OnSpawnItemSyn;
        RoomModel.Instance.OnActGimicSyn += this.OnActGimicSyn;
        //RoomModel.Instance.OnPlayerDeadSyn += this.OnPlayerDeadSyn;
        //RoomModel.Instance.OnPlayerRespownSyn += this.OnPlayerRespownSyn;


        //プレイヤーの設定
        foreach (var user in RoomModel.Instance.joinedUserList)
        {
            if (user.Key == RoomModel.Instance.ConnectionId)
            {
                //プレイヤーを生成
                if (UnityEngine.XR.XRSettings.isDeviceActive) player = Instantiate(mainPlayerVRPrefab);
                else player = Instantiate(mainPlayerPrefab);

                if (RoomModel.Instance.joinedUserList[user.Key].JoinOrder == 1)
                {//働く方リスポーン
                    player.name = "Worker";
                    player.transform.position = spawnPointP1.position;
                    switch (SceneManager.GetActiveScene().name)
                    {
                        case "Stage_1":
                            foreach(var crane in syncCraneList)
                            {
                                Destroy(crane.GetComponent<Rigidbody>());
                            }
                            //player.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                            break;
                        case "Stage_2":
                            if (UnityEngine.XR.XRSettings.isDeviceActive) player.transform.localScale = new Vector3(1f, 1f, 1f);
                            break;
                        case "Stage_3":
                            GameObject[] gameObject = GameObject.FindGameObjectsWithTag("MovablePartition");
                            foreach(var obj in gameObject)
                            {
                                obj.GetComponent<BoxCollider>().enabled = false;
                            }
                            itemBox = GameObject.Find("ItemBox").GetComponent<ItemBox>();
                            if (UnityEngine.XR.XRSettings.isDeviceActive) player.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
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
                            foreach (var obj in syncObjList)
                            {
                                Destroy (obj.GetComponent<XRGrabInteractable>());
                                Destroy (obj.GetComponent<Rigidbody>());
                            }
                            //player.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                            break;
                        case "Stage_2":
                            if (UnityEngine.XR.XRSettings.isDeviceActive) player.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                            break;
                        case "Stage_3":
                            GameObject[] gameObject = GameObject.FindGameObjectsWithTag("MovablePartition");
                            itemBox = GameObject.Find("ItemBox").GetComponent<ItemBox>();
                            if (UnityEngine.XR.XRSettings.isDeviceActive) player.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                            foreach (var obj in syncObjList)
                            {
                                if(obj.GetComponent<XRGrabInteractable>()) Destroy(obj.GetComponent<XRGrabInteractable>());
                                Destroy(obj.GetComponent<Rigidbody>());
                            }
                            break;
                    }
                }
                mainSpawnPoint = player.transform;
                InvokeRepeating("UpDatePlayer", 0.1f, 0.1f);
                //if (SceneManager.GetActiveScene().name == "Stage_1")
                //    player.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                if (player.transform.GetComponent<Rigidbody>() != null)
                {
                    player.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                    player.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
            else
            {
                subplayer = Instantiate(subPlayerPrefab);
                if (RoomModel.Instance.joinedUserList[user.Key].JoinOrder == 1)
                {//働く方リスポーン
                    subplayer.name = "Worker";
                    subplayer.transform.position = spawnPointP1.position;
                    if (UnityEngine.XR.XRSettings.isDeviceActive) subplayer.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                }
                else if (RoomModel.Instance.joinedUserList[user.Key].JoinOrder == 2)
                {//労災側リスポーン
                    subplayer.name = "Stricker";
                    subplayer.transform.position = spawnPointP2.position;
                    subplayer.transform.parent = spawnPointP2.transform;
                    Debug.Log("労災側の相手プレイヤー生成成功");
                    if (UnityEngine.XR.XRSettings.isDeviceActive) subplayer.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                }
            }

            resultScoreManager = GameObject.Find("ResultScoreManager").GetComponent<ResultScoreManager>();

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
        RoomModel.Instance.OnActGimicSyn -= this.OnActGimicSyn;
        RoomModel.Instance.OnSpawnItemSyn -= this.OnSpawnItemSyn;
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
    /// ギミック動作
    /// </summary>
    public async void ActGimic(string parentName)
    {
        await RoomModel.Instance.ActGimicAsync(parentName);
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
        if (syncCraneList != null)
        {
            foreach(var crane in syncCraneList)
            {
                if (crane.GetComponent<Rigidbody>() == null) continue;
                await RoomModel.Instance.UpdateObjectAsync(crane.transform.localPosition,
                    crane.transform.rotation, crane.name);            }

        }
        if(syncObjList != null)
        {
            for (int i = 0; i < syncObjList.Count; i++)
            {
                if (syncObjList[i].GetComponent<Rigidbody>() == null) continue;
                //if (Player.name == "Stricker") continue;
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
        switch (SceneManager.GetActiveScene().name)
        {
            case "Stage_2":
                spawnObjId = id;
                if (UnityEngine.XR.XRSettings.isDeviceActive) pot = Instantiate(vrObjPrefab);
                else pot = Instantiate(objPrefab);
                pot.name = pot.name + potCount;
                pot.transform.position = pos;
                if (pot == null)
                {
                    Debug.Log("Nullオブジェクト");
                }
                objList.Add(id, pot);
                FlowerPotManager flowerPotManager = GameObject.Find("FlowerPotManager").GetComponent<FlowerPotManager>();
                flowerPotManager.potList.Add(pot);
                potCount++;
                if (Player.name == "Worker")
                {
                    Destroy(pot.GetComponent<XRGrabInteractable>());
                    Destroy(pot.GetComponent<Rigidbody>());
                }
                break;
            case "Stage_3":
                spawnObjId = id;
                GameObject obj = Instantiate(objPrefab);
                obj.transform.position = pos;
                if (obj == null)
                {
                    Debug.Log("Nullオブジェクト");
                }
                //objList.Add(id, obj);
                break;
        }
        CancelInvoke("UpdateObj");
        //オブジェクト更新を行う
        InvokeRepeating("UpdateObj", 0.1f, 0.1f);
    }

    /// <summary>
    /// アイテム生成通知
    /// </summary>
    void OnSpawnItemSyn(int itemId,string uniqueId, Vector3 spawnPos)
    {
        if(itemId == 0)
        {//注射器
            injecterCount++;
            GameObject gameObject = Instantiate(InjectorPrefab);
            gameObject.transform.position = spawnPos;
            gameObject.name = "Injector" + injecterCount;
            objList.Add(uniqueId, gameObject);
            if (Player.name == "Worker")
            {
                Destroy(gameObject.GetComponent<Rigidbody>());
            }
            else
            {
                itemBox.createdObj = gameObject;
            }
        }
        else if(itemId == 1)
        {//水入りコップ
            cupCount++;
            GameObject gameObject = Instantiate(cupPrefab);
            gameObject.transform.position = spawnPos;
            gameObject.name = "Cup" + cupCount;
            objList.Add(uniqueId, gameObject);
            if (Player.name == "Worker")
            {
                Destroy(gameObject.GetComponent<Rigidbody>());
            }
            else
            {
                itemBox.createdObj = gameObject;
            }
        }
        else if(itemId == 2)
        {//コーヒー
            coffeeCount++;
            GameObject gameObject = Instantiate(coffeePrefab);
            gameObject.transform.position = spawnPos;
            gameObject.name = "Coffee" + coffeeCount;
            DeliveryManager deliveryManager = GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>();
            deliveryManager.coffeeObj = gameObject;
            objList.Add(uniqueId, gameObject);
            if (Player.name == "Stricker")
            {
                Destroy(gameObject.GetComponent<XRGrabInteractable>());
                Destroy(gameObject.GetComponent<Rigidbody>());
            }
        }
        else if(itemId == 3)
        {//コーヒーの破損
            coffeeCount++;
            GameObject gameObject = Instantiate(coffeeLostPrefab);
            gameObject.transform.position = spawnPos;
            gameObject.name = "SadCoffee";
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
        foreach (var obj in syncCraneList)
        {
            if(obj.name == id)
            {
                obj.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
                obj.transform.DORotate(rot.eulerAngles, 0.1f);
            }
        }

    }

    /// <summary>
    /// オブジェクト削除通知
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="tag"></param>
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
                else if(SceneManager.GetActiveScene().name == "Stage_3")
                {
                    if(player.name == "Stricker" && itemBox.createdObj != null)
                    {
                        if (itemBox.createdObj.name == objName)
                        {
                            itemBox.createdObj = null;
                        }
                    }

                    Destroy(GameObject.Find(objName));

                    if (objName.Contains("Coffee") && isDelivery == false)
                    {
                        GameObject.Find("TaskUIManager").GetComponent<TaskUIManager>().FluctNowTime(-5);
                    }
                }
                else
                {
                    Destroy(GameObject.Find(objName));
                }
                if (SceneManager.GetActiveScene().name != "Stage_1")
                {
                    CancelInvoke("UpdateObj");
                    //オブジェクト更新を行う
                    InvokeRepeating("UpdateObj", 0.1f, 0.1f);
                }
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
                    if (syncObjList[i].tag == "Item") syncObjList[i].AddComponent<XRGrabInteractable>();
                    Rigidbody rigidbody = syncObjList[i].AddComponent<Rigidbody>();
                    Debug.Log(i.ToString()+"番のオブジェクトの権限を得ました");
                    if (syncObjList[i].tag == "MovablePartition")
                    {
                        syncObjList[i].gameObject.GetComponent<BoxCollider>().enabled = false;
                        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                    }
                    return;
                }
                else if(joinOrder != RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder)
                {
                    if (syncObjList[i].tag == "Item") Destroy(syncObjList[i].GetComponent<XRGrabInteractable> ());
                    Destroy(syncObjList[i].GetComponent<Rigidbody>());
                    Debug.Log("オブジェクトの権限を失いました");
                    if (syncObjList[i].tag == "MovablePartition")
                    {
                        syncObjList[i].GetComponent<BoxCollider>().enabled = true;
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

    /// <summary>
    /// カウント通知
    /// </summary>
    /// <param name="isTask"></param>
    async void OnCounted(bool isTask)
    {
        switch (isTask)
        {
            case true:
                // タスク回数を加算
                TaskCnt++;
                //resultScoreManager.successNum++;

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
                                if (UnityEngine.XR.XRSettings.isDeviceActive)
                                {
                                    Initiate.Fade("01_Exp_Worker_2_VR", Color.black, 2.0f);
                                    //Initiate.Fade("VR_30_ResultScene", Color.black, 1.0f);
                                }
                                else Initiate.Fade("01_Exp_Worker_2", Color.black, 1.0f);
                            }
                            else if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 2)
                            {
                                if (UnityEngine.XR.XRSettings.isDeviceActive)
                                {
                                    Initiate.Fade("01_Exp_Worker_2_VR", Color.black, 2.0f);
                                    //Initiate.Fade("VR_30_ResultScene", Color.black, 1.0f);
                                }
                                else Initiate.Fade("01_Exp_Stricker_2", Color.black, 1.0f);
                            }
                        }
                        break;
                    case "Stage_2":                   
                        if (TaskCnt >= 1 && isGetKey)
                        {
                            //フェードアウトしてシーン遷移
                            if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 1)
                            {
                                if (UnityEngine.XR.XRSettings.isDeviceActive) Initiate.Fade("01_Exp_Worker_3_VR", Color.black, 2.0f);
                                else Initiate.Fade("01_Exp_Worker_3", Color.black, 1.0f);
                            }
                            else if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 2)
                            {
                                if (UnityEngine.XR.XRSettings.isDeviceActive) Initiate.Fade("01_Exp_Stricker_3_VR", Color.black, 2.0f);
                                else Initiate.Fade("01_Exp_Stricker_3", Color.black, 1.0f);
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
                        GameObject.Find("TaskUIManager").GetComponent<TaskUIManager>().FluctNowTime(10);
                        isDelivery = false;
                        if (TaskCnt >= 5)
                        {
                            await RoomModel.Instance. LeavedAsync();
                            if (UnityEngine.XR.XRSettings.isDeviceActive) Initiate.Fade("VR_30_ResultScene", Color.black, 2.0f);
                            else Initiate.Fade("30_ResultScene", Color.black, 1.0f);
                        }
                        break;
                }
                break;
            case false:
                // 死亡回数を加算
                GameObject.Find(player.name).GetComponent<Player>().deathCnt++;

                resultScoreManager.failureNum++;
                if (SceneManager.GetActiveScene().name == "Stage_1")
                {
                    // 死亡回数テキストを取得し、死亡回数を反映
                    GameObject.Find("DeathCount").GetComponent<Text>().text =
                        ": " +
                        GameObject.Find(player.name).GetComponent<Player>().deathCnt;
                }
                else
                {
                    // 死亡回数テキストを取得し、死亡回数を反映
                    GameObject.Find("DeathCount").GetComponent<Text>().text =
                        ": " +
                        GameObject.Find(player.name).GetComponent<Player>().deathCnt +
                        "/5";
                }
                break;
        }
    }

    /// <summary>
    /// ギミック動作通知
    /// </summary>
    /// <param name="parentName"></param>
    void OnActGimicSyn(string parentName)
    {
        //オブジェクトに応じて対応
        if (parentName.Contains("Group")) GameObject.Find(parentName).transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
        else if (parentName.Contains("knife")) 
        {
            player.GetComponent<Player>().OnlineDeath(); 
            GameObject.Find("ObjectGrabber").GetComponent<ObjectGrabber>().Release();
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
