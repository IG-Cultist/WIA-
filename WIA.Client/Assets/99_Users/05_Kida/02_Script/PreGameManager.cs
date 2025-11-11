using DG.Tweening;
using Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Scripting;
using static Shared.Interfaces.StreamingHubs.EnumManager;
using static UnityEngine.Rendering.DebugUI.Table;

public class PreGameManager : MonoBehaviour
{
    #region 初期設定
    [Header("初期設定")]
    PlayerData playerData;
    int tasks = 0;      // 労災、タスクの達成状況
    public Vector3 spawnPos;
    [SerializeField] GameObject mainPlayerPrefab; //メインプレイヤーのプレハブ
    [SerializeField] GameObject subPlayerPrefab; //他プレイヤーのプレハブ
    [SerializeField] GameObject objPrefab; //オブジェクトのプレハブ
    GameObject player; //自分のプレイヤー
    GameObject subplayer; //他人のプレイヤー
    Dictionary<string,GameObject> objList = new Dictionary<string, GameObject>(); //すべてのオブジェクト
    #endregion

    #region その他
    [Header("その他")]
    [SerializeField] float xRadius;            // 生成範囲のx半径
    [SerializeField] float yRadius;            // 生成範囲のy半径
    [SerializeField] float distMinSpawnPos;    // 生成しない範囲
    [SerializeField] AudioResource normalBGM;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (RoomModel.Instance)
        RoomModel.Instance.OnUpdatePlayerSyn += OnUpdatePlayerSyn;
        RoomModel.Instance.OnSpawnedObjectSyn += OnSpawnedObjectSyn;
        RoomModel.Instance.OnUpdatedObject += OnUpdatedObject;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
        foreach (var user in RoomModel.Instance.joinedUserList)
            {
                if (user.Key == RoomModel.Instance.ConnectionId)
                {
                    player=Instantiate(mainPlayerPrefab);
                    InvokeRepeating("UpDatePlayer",0.2f,0.2f);
                }
                else
                {
                    subplayer=Instantiate(subPlayerPrefab);
                }
            }
        InvokeRepeating("UpdateObj", 0.2f, 0.2f);
    }

    private void OnDisable()
    {
        RoomModel.Instance.OnUpdatePlayerSyn -= OnUpdatePlayerSyn;
        RoomModel.Instance.OnSpawnedObjectSyn -= OnSpawnedObjectSyn;
        RoomModel.Instance.OnUpdatedObject -= OnUpdatedObject;
        RoomModel.Instance.OnLeavedUser -= OnLeavedUser;
    }

    private void Update()
    {
        if(Input.GetKeyDown("o"))
        {
            SpawnObj();
        }
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
        if (objList == null) return;
        foreach (var obj in objList) 
        {
            await RoomModel.Instance.UpdateObjectAsync(obj.Value.transform.position, obj.Key);
        }
    }

    /// <summary>
    /// プレイヤーの位置同期
    /// </summary>
    public async void UpDatePlayer()
    {
        await RoomModel.Instance.UpdatePlayerAsync(player.transform.position,player.transform.rotation);
    }

    /// <summary>
    /// プレイヤーの更新通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    void OnUpdatePlayerSyn(Vector3 pos,Quaternion rot)
    {
        subplayer.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
        subplayer.transform.DORotate(rot.eulerAngles, 0.1f);
    }

    /// <summary>
    /// オブジェクト生成通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    void OnSpawnedObjectSyn(Vector3 pos,string id)
    {
        GameObject gameObject = Instantiate(objPrefab);
        gameObject.transform.position = pos;
        objList.Add(id, gameObject);
    }

    /// <summary>
    /// オブジェクト更新通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    void OnUpdatedObject(Vector3 pos,string id)
    {
        foreach (var obj in objList)
        {
            if(obj.Key == id)
            {
                obj.Value.transform.position = pos;
            }
        }
    }

    /// <summary>
    /// 退室通知
    /// </summary>
    /// <param name="joinedUser"></param>
    void OnLeavedUser(JoinedUser joinedUser)
    {
        Debug.Log(joinedUser.UserData.Name+"の退室を確認");
    }
}
