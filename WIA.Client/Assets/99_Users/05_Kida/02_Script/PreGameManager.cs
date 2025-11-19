using DG.Tweening;
using NUnit;
using Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using System.Xml;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Scripting;
using static Shared.Interfaces.StreamingHubs.EnumManager;
using static UnityEngine.Rendering.DebugUI.Table;

public class PreGameManager : MonoBehaviour
{
    #region 基本
    [Header("基本設定")]
    PlayerData playerData;
    int tasks = 0;      //タスクの数
    public Vector3 spawnPos;
    [SerializeField] GameObject mainPlayerPrefab; //操作プレイヤー
    [SerializeField] GameObject subPlayerPrefab; //非操作プレイヤー
    [SerializeField] GameObject objPrefab; //オブジェクト
    [SerializeField] List<GameObject> syncObjList;//同期用オブジェクト初期設定
    public static List<GameObject> syncGameObjectList;//同期用オブジェクト
    public static List<GameObject> SyncGameObjectList
    {
        get { return syncGameObjectList; }
    }
    GameObject player; //操作プレイヤー
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
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (RoomModel.Instance)
        {//オンラインだったら

            //既存オブジェクト初期設定
            syncGameObjectList = syncObjList;

            //オブジェクト更新を行う
            InvokeRepeating("UpdateObj", 0.1f, 0.1f);

            if (RoomModel.Instance.IsMaster == false) 
            {
                //すべてのオブジェクトからRigidbodyを外す
                foreach (var obj in syncGameObjectList)
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

            //プレイヤーの設定
            foreach (var user in RoomModel.Instance.joinedUserList)
            {
                if (user.Key == RoomModel.Instance.ConnectionId)
                {
                    player = mainPlayerPrefab;
                    player.name = "Main";
                    Instantiate(player);
                    player.transform.position = Vector3.zero;
                    InvokeRepeating("UpDatePlayer", 0.1f, 0.1f);
                }
                else
                {
                    subplayer = subPlayerPrefab;
                    subplayer.name = "Sub";
                    //subplayer.transform.GetComponent<Player>().enabled = false;
                    //subplayer.transform.Find("First Person Camera").GetComponent<Camera>().enabled = false;
                    //subplayer.transform.Find("First Person Camera").GetComponent<CinemachineCamera>().enabled = false;
                    subplayer.transform.position = new Vector3(1,2,0);
                    Instantiate(subplayer);
                }
            }
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
    }

    private void Update()
    {
        /*以下はデバッグ用コマンド*/
#if DEBUG
        //Oボタンでオブジェクト生成
        if(Input.GetKeyDown("o"))
        {
            SpawnObj();
        }
        //Sボタンでオブジェクト1の権限取得
        if(Input.GetKeyDown("p"))
        {
            ObjectOwnershipSwap("0", RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder);
        }
#endif
    }

    /// <summary>
    /// オブジェクト生成
    /// </summary>
    public async void SpawnObj()
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
                if (obj.Value.GetComponent<Rigidbody>() == null) continue;
                await RoomModel.Instance.UpdateObjectAsync(obj.Value.transform.position, obj.Value.transform.rotation, obj.Key);
            }
        }

        for (int i=0;i<syncGameObjectList.Count;i++)
        {
            if (syncGameObjectList[i].GetComponent<Rigidbody>() == null) continue;
            await RoomModel.Instance.UpdateObjectAsync(syncGameObjectList[i].transform.position,
                syncGameObjectList[i].transform.rotation, i.ToString());
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
        await RoomModel.Instance.UpdatePlayerAsync(player.transform.position,player.transform.rotation);
    }

    /// <summary>
    /// プレイヤー更新通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    void OnUpdatePlayerSyn(Vector3 pos,Quaternion rot)
    {
        //subplayer.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear).OnUpdate(() =>
        //{
        //    Rigidbody rb = subplayer.GetComponent<Rigidbody>();
        //});
        Rigidbody rb = subplayer.GetComponent<Rigidbody>();

        rb.DOMove(pos, 0.1f).SetEase(Ease.Linear).OnUpdate(() =>
        {
        });
        //Vector3 prevPos = subplayer.transform.position;
        //DOTween.To(
        //    () => 0f,
        //    t =>
        //    {
        //        Vector3 newPos = Vector3.Lerp(subplayer.transform.position, pos, t);
        //        subplayer.GetComponent<Rigidbody>().MovePosition(newPos);
        //        subplayer.GetComponent<Rigidbody>().linearVelocity = (newPos - prevPos) / Time.fixedDeltaTime;
        //        prevPos = newPos;
        //    }, 1f, 0.1f)
        //    .SetEase(Ease.OutQuad);

        //Vector3 startPos = subplayer.GetComponent<Rigidbody>().position;
        //Vector3 targetPos = pos;
        //float duration = 0.1f;
        //float forceMultiplier = 500f;
        //DOTween.To(() => 0f, t =>
        //{
        //    Vector3 remaining = targetPos - subplayer.GetComponent<Rigidbody>().position;
        //    subplayer.GetComponent<Rigidbody>().AddForce(remaining * forceMultiplier * Time.fixedDeltaTime, ForceMode.Force);
        //}, 1f, duration)
        //.SetEase(Ease.OutQuad)
        //.OnComplete(() =>
        //{
        //    subplayer.GetComponent<Rigidbody>().MovePosition(targetPos);
        //    subplayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //});

        subplayer.transform.DORotate(rot.eulerAngles, 0.1f);
    }

    /// <summary>
    /// オブジェクト生成通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    void OnSpawnedObjectSyn(Vector3 pos,string id)
    {
        spawnObjId = id;
        GameObject gameObject = Instantiate(objPrefab);
        gameObject.transform.position = pos;
        objList.Add(id, gameObject);
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
        for (int i = 0;i<syncGameObjectList.Count;i++)
        {
            if (i.ToString() == id)
            {
                syncGameObjectList[i].transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
                syncGameObjectList[i].transform.DORotate(rot.eulerAngles, 0.1f);
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
        for (int i = 0; i < syncGameObjectList.Count; i++)
        {
            if (i.ToString() == uniqueId)
            {
                if (joinOrder == RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder)
                {
                    syncGameObjectList[i].AddComponent<Rigidbody>();
                    Debug.Log("オブジェクトの権限を得ました");
                    return;
                }
                else
                {
                    Destroy(syncGameObjectList[i].GetComponent<Rigidbody>());
                    Debug.Log("オブジェクトの権限を失いました");
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
    /// 退室通知
    /// </summary>
    /// <param name="joinedUser"></param>
    void OnLeavedUser(JoinedUser joinedUser)
    {
        Debug.Log(joinedUser.UserData.Name+"が退室しました。");
    }
}
