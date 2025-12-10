////////////////////////////////////////////////////////////////
///
/// RoomHubへの接続を管理するスクリプト
/// 
/// Aughter:木田晃輔
///
////////////////////////////////////////////////////////////////

#region using一覧
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using WIA.Shared.Interfaces.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WIA.Shared.Interfaces.StreamingHubs;
using static Shared.Interfaces.StreamingHubs.EnumManager;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;
//using static Unity.Cinemachine.CinemachineSplineRoll;
using Vector2 = UnityEngine.Vector2;
using NUnit.Framework;
using static Player;
#endregion

public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannel channel;  //サーバーURL
    private IRoomHub roomHub;     //roomHubの関数を呼び出す時に使う

    //マスタークライアントかどうか
    public bool IsMaster { get; set; }

    //接続ID
    public Guid ConnectionId { get; private set; }

    // 現在の参加者情報
    public Dictionary<Guid, JoinedUser> joinedUserList { get; private set; } = new Dictionary<Guid, JoinedUser>();


    //現在のルーム情報
    //public RoomData[] roomDataList { get; set; }

    #region 通知定義一覧

    #region システム

    //ルーム検索通知
    public Action<List<string>, List<string>, List<string>> OnSearchedRoom { get; set; }

    //ルーム生成通知
    public Action OnCreatedRoom { get; set; }

    //ユーザー接続通知
    public Action<JoinedUser> OnJoinedUser { get; set; }

    //入室失敗通知
    public Action<int> OnFailedJoinSyn { get; set; }

    //ユーザー退室通知
    public Action<JoinedUser> OnLeavedUser { get; set; }

    //キャラクター変更通知
    public Action<Guid, int> OnChangedCharacter { get; set; }

    //準備完了通知
    public Action<Guid> OnReadySyn { get; set; }

    //ゲーム開始通知
    public Action OnStartedGame { get; set; }

    //プレイヤー待機通知
    public Action OnWaitSyn {  get; set; }

    //同時開始通知
    public Action OnSameStarted {  get; set; }

    public Action<bool> OnCounted {  get; set; }

    //難易度上昇通知
    public Action<int> OnAscendDifficultySyn { get; set; }

    //次ステージ進行通知
    public Action<STAGE_TYPE> OnAdanceNextStageSyn { get; set; }

    //ステージ進行通知
    public Action OnAdvancedStageSyn { get; set; }

    //ゲーム終了通知
    public Action<ResultData> OnGameEndSyn { get; set; }

    #endregion

    #region プレイヤー・マスタクライアント

    //マスタークライアントの変更通知
    public Action OnChangedMasterClient { get; set; }

    //マスタークライアントの更新通知
    public Action<MasterClientData> OnUpdateMasterClientSyn { get; set; }

    //プレイヤー位置回転通知
    public Action<Vector3,Quaternion,int,float> OnUpdatePlayerSyn { get; set; }


    //プレイヤーダウン通知
    public Action<Guid> OnPlayerDeadSyn { get; set; }

    //プレイヤーリスポーン通知
    public Action<Guid> OnPlayerRespownSyn {  get; set; }

    #endregion

    #region 敵

    //敵の出現通知
    public Action<List<SpawnEnemyData>> OnSpawndEnemy { get; set; }

    //敵体力増減通知
    public Action<EnemyDamegeData> OnEnemyHealthSyn { get; set; }

    //敵削除通知
    public Action<string> OnDeleteEnemySyn { get; set; }


    #endregion

    #region アイテム

    //レリックの生成通知
    public Action<Dictionary<string, DropRelicData>> OnDropedRelic { get; set; }

    //アイテム獲得通知
    public Action<Guid, string, int, int, int> OnGetItemSyn { get; set; }

    #endregion

    #region ギミック

    //ギミックの起動通知
    public Action<string, bool> OnBootedGimmick { get; set; }

    // オブジェクト生成通知
    public Action< Vector3, string> OnSpawnedObjectSyn { get; set; }

    // オブジェクト生成通知
    public Action< Vector3,Quaternion, string> OnUpdatedObject { get; set; }

    //オブジェクトの削除通知
    public Action<string> OnDeliteObjectSyn {  get; set; }

    //オブジェクト所有権変更通知
    public Action<string,int> OnOwnershipSwapObjectSyn {  get; set; }

    #endregion

    #region 端末

    // 端末起動通知
    public Action<int> OnBootedTerminal { get; set; }

    // 端末成功通知
    public Action<int> OnTerminalsSuccessed { get; set; }

    // 端末失敗通知
    public Action<int> OnTerminalFailured { get; set; }

    // 端末ジャンブル適用通知
    public Action<List<DropRelicData>> OnTerminalJumbled { get; set; }

    #endregion

    #region 発射物

    // 発射物の生成通知
    public Action<List<ShootBulletData>> OnShootedBullet { get; set; }

    #endregion

    #endregion

    #region RoomModelインスタンス生成
    private static RoomModel instance;
    public static RoomModel Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // インスタンスが複数存在しないように、既に存在していたら自身を消去する
            Destroy(gameObject);
        }
    }

    //public static RoomModel Instance
    //{
    //    get
    //    {
    //        // GETプロパティを呼ばれたときにインスタンスを作成する(初回のみ)
    //        if (instance == null)
    //        {
    //            GameObject gameObj = new GameObject("RoomModel"+DateTime.Now.ToString());
    //            instance = gameObj.AddComponent<RoomModel>();
    //            DontDestroyOnLoad(gameObj);
    //        }
    //        return instance;
    //    }
    //}
    #endregion

    #region MagicOnion接続・切断処理
    /// <summary>
    /// MagicOnion接続処理
    /// Aughter:木田晃輔
    /// </summary>
    /// <returns></returns>
    public async UniTask ConnectAsync()
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
    }

    /// <summary>
    /// MagicOnion切断処理
    /// Aughter:木田晃輔
    /// </summary>
    /// <returns></returns>
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }
    #endregion

    /// <summary>
    /// 破棄処理
    /// Aughter:木田晃輔
    /// </summary>
    async void OnDestroy()
    {
        DisconnectAsync();
        instance = null;
    }

    #region 通知の処理

    /// <summary>
    /// ゲーム終了通知
    /// </summary>
    /// <param name="result"></param>
    public async void OnGameEnd(ResultData result)
    {
        OnGameEndSyn(result);
        await roomHub.LeavedAsync(true);
    }

    #region 入室・退室・準備完了通知


    /// <summary>
    /// ルーム生成通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <returns></returns>
    public void OnRoom()
    {
        OnCreatedRoom();
    }

    /// <summary>
    /// 入室通知(IRoomHubReceiverインターフェイスの実装)
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="joinedUser"></param>
    public void Onjoin(JoinedUser joinedUser)
    {
        //OnJoinedUser?.Invoke(joinedUser);

        if (!joinedUserList.ContainsKey(joinedUser.ConnectionId))
            joinedUserList.Add(joinedUser.ConnectionId, joinedUser);

        //入室通知
        OnJoinedUser(joinedUser);

    }

    public void OnFailedJoin(int errorId)
    {
        OnFailedJoinSyn(errorId);
    }

    /// <summary>
    /// 退室通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="user"></param>
    public void OnLeave(Dictionary<Guid, JoinedUser> joinedUser, Guid targetUser)
    {
        int i = 1;
        JoinedUser leaveUser = joinedUser[targetUser];
        joinedUserList = joinedUser;
        joinedUserList.Remove(targetUser);
        foreach (var user in joinedUserList)
        {
            user.Value.JoinOrder = i;
            i++;
        }

        OnLeavedUser(leaveUser);
    }

    /// <summary>
    /// キャラクター変更通知
    /// Aughter:木田晃輔
    /// </summary>
    public void OnChangeCharacter(Guid guid, int characterId)
    {
        joinedUserList[guid].CharacterID = characterId;
        OnChangedCharacter(guid, characterId);
    }

    /// <summary>
    /// 準備完了通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="conID"></param>
    public void OnReady(JoinedUser joinedUser)
    {
        joinedUserList[joinedUser.ConnectionId] = joinedUser;
        OnReadySyn(joinedUser.ConnectionId);
    }


    public void OnWait()
    {
        OnWaitSyn();
    }

    public void OnSameStart()
    {
        OnSameStarted();
    }
    #endregion

    #region プレイヤー通知関連
    /// <summary>
    /// プレイヤーの移動通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="user"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="animID"></param>
    public void OnUpdatePlayer(Vector3 pos, Quaternion rot, int animState, float moveSpeed)
    {
        OnUpdatePlayerSyn(pos,rot,animState,moveSpeed);
    }

    /// <summary>
    /// マスタークライアントの変更通知
    /// Aughter:木田晃輔
    /// </summary>
    public void OnChangeMasterClient()
    {
        OnChangedMasterClient();
        Debug.Log("あなたがマスタークライアントになりました");
        IsMaster = true;
    }

    ///// <summary>
    ///// プレイヤー死亡通知
    ///// Aughter:木田晃輔
    ///// </summary>
    ///// <param name="guid"></param>
    //public void OnPlayerDead(Guid guid)
    //{
    //    OnPlayerDeadSyn(guid);
    //}

    //public void OnPlayerRespown(Guid guid)
    //{
    //    OnPlayerRespownSyn(guid);
    //}


    #endregion

    #region レリック通知関連

    /// <summary>
    /// レリック生成通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="relicID"></param>
    /// <param name="pos"></param>
    public void OnDropRelic(Dictionary<string, DropRelicData> relicDatas)
    {
        OnDropedRelic(relicDatas);
    }

    #endregion

    #region 端末関連

    /// <summary>
    /// 端末起動通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="termID"></param>
    public void OnBootTerminal(int termID)
    {
        OnBootedTerminal(termID);
    }

    /// <summary>
    /// 端末結果通知
    /// Author:木田晃輔
    /// </summary>
    /// <param name="termID"></param>
    /// <param name="result"></param>
    public void OnTerminalsSuccess(int termID)
    {
        OnTerminalsSuccessed(termID);
    }

    /// <summary>
    /// 端末失敗通知
    /// </summary>
    /// <param name="termID"></param>
    public void OnTerminalFailure(int termID)
    {
        OnTerminalFailured(termID);
    }

    /// <summary>
    /// 端末ジャンブル通知
    /// </summary>
    /// <param name="termID"></param>
    public void OnTerminalJumble(List<DropRelicData> relics)
    {
        OnTerminalJumbled(relics);
    }

    #endregion

    #region ゲーム内UI・仕様の同期関連
    /// <summary>
    /// ゲーム開始通知
    /// Aughter:木田晃輔
    /// </summary>
    public void OnStartGame()
    {
        OnStartedGame();
    }

    /// <summary>
    /// ギミックの起動通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="gimmickData"></param>
    public void OnBootGimmick(string uniqueID, bool triggerOnce)
    {
        OnBootedGimmick(uniqueID, triggerOnce);
    }

    /// <summary>
    /// アイテム獲得通知
    /// Author:木田晃輔
    /// </summary>
    public void OnGetItem(Guid conId, string itemID, int nowLevel, int nowExp, int nextLevelExp)
    {
        OnGetItemSyn(conId, itemID, nowLevel, nowExp, nextLevelExp);
    }

    /// <summary>
    /// 難易度上昇の通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="difID"></param>
    public void OnAscendDifficulty(int difID)
    {
        OnAscendDifficultySyn(difID);
    }

    /// <summary>
    /// 次ステージ進行の通知
    /// Aughter:木田晃輔
    /// </summary>
    /// <param name="stageID"></param>
    public void OnAdanceNextStage(STAGE_TYPE stageType)
    {
        OnAdanceNextStageSyn(stageType);
    }

    /// <summary>
    /// ステージ進行通知
    /// Author;木田晃輔
    /// </summary>
    public void OnAdvancedStage()
    {
        OnAdvancedStageSyn();
    }

    /// <summary>
    /// オブジェクト生成通知
    /// Author;木田晃輔
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <param name="uniqueId"></param>
    public void OnSpawnObject(Vector3 spawnPos, string uniqueId)
    {
        OnSpawnedObjectSyn(spawnPos, uniqueId);
    }

    /// <summary>
    /// オブジェクト更新通知
    /// Author;木田晃輔
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="uniqueId"></param>
    public void OnUpdateObject(Vector3 pos,Quaternion rot, string uniqueId)
    {
        OnUpdatedObject(pos,rot, uniqueId);
    }


    public void OnDeliteObject(string objName)
    {
        OnDeliteObjectSyn(objName);
    }

    /// <summary>
    /// オブジェクト所有権変更通知
    /// Author;木田晃輔
    /// </summary>
    /// <param name="uniqueId"></param>
    /// <param name="joinOrder"></param>
    public void OnOwnershipSwapObject(string uniqueId, int joinOrder)
    {
        OnOwnershipSwapObjectSyn(uniqueId, joinOrder);
    }


    /// <summary>
    /// オブジェクト所有権変更通知
    /// Author;木田晃輔
    /// </summary>
    public void OnCount(bool isTask)
    {
        OnCounted(isTask);
    }

    #endregion

    #endregion

    #region リクエスト関連

    #region 入室からゲーム開始まで

    /// <summary>
    /// 入室同期
    /// Aughter:木田晃輔
    /// </summary>
    /// <returns></returns>
    public async UniTask JoinedAsync(int userId)
    {
        this.ConnectionId = await roomHub.GetConnectionIdAsync();
        joinedUserList = await roomHub.JoinedAsync(userId);
        if (joinedUserList == null) return;
        foreach (var user in joinedUserList)
        {
            if (user.Key == this.ConnectionId)
            {
                this.IsMaster = user.Value.IsMaster;
                Debug.Log("モデル：" + RoomModel.Instance.ConnectionId);
            }
        }
    }

    /// <summary>
    /// 退室の同期
    /// Aughter:木田晃輔
    /// </summary>
    /// <returns></returns>
    public async UniTask LeavedAsync()
    {
        await roomHub.LeavedAsync(false);
        this.IsMaster = false;
        //自分をリストから消す
        joinedUserList.Clear();
    }

    /// <summary>
    /// 準備完了同期
    /// </summary>
    /// <returns></returns>
    public async UniTask ReadyAsync(int characterId)
    {
        await roomHub.ReadyAsync(characterId);
    }

    /// <summary>
    /// プレイヤー待機同期
    /// Aughter:木田晃輔
    /// </summary>
    /// <returns></returns>
    public async UniTask WaitAsync()
    {
        await roomHub.WaitAsync();
    }
    #endregion

    #region ゲーム内

    #region プレイヤー関連
    /// <summary>
    /// プレイヤーの更新同期
    /// </summary>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public async UniTask UpdatePlayerAsync(Vector3 pos,Quaternion rot,int animState, float moveSpeed)
    {
        await roomHub.UpdatePlayerAsync(pos,rot,animState, moveSpeed);
    }

    /// <summary>
    /// アイテム獲得
    /// Author:Nishiura
    /// </summary>
    /// <param name="itemType">アイテムの種類</param>
    /// <param name="itemID">識別ID(文字列)</param>
    /// <returns></returns>
    public async UniTask GetItemAsync(EnumManager.ITEM_TYPE itemType, string itemID)
    {
        //await roomHub.GetItemAsync(itemType, itemID);
    }


    //public async UniTask PlayerDeadAsync()
    //{
    //    await roomHub.PlayerDeadAsync();
    //}

    //public async UniTask PlayerRespownAsync()
    //{
    //    await roomHub.PlayerRespownAsync();
    //}
    #endregion

    #region 敵関連



    /// <summary>
    /// 未選択のステータス強化選択肢取得する
    /// </summary>
    /// <returns></returns>
    //public async UniTask<Dictionary<Guid, List<StatusUpgrateOptionData>>> GetUpgradeGroupsAsync()
    //{
    //    return await roomHub.GetUpgradeGroupsAsync();
    //}

    #endregion

    #region ゲーム内UI、仕様関連
    /// <summary>
    /// ギミックの起動同期
    ///  Aughter:木田晃輔
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <returns></returns>
    public async UniTask BootGimmickAsync(string uniqueID, bool triggerOnce)
    {
        await roomHub.BootGimmickAsync(uniqueID, triggerOnce);
    }


    /// <summary>
    /// ステージクリア
    /// Author:Nishiura
    /// </summary>
    /// <param name="isAdvance">ステージ進行判定</param>
    /// <returns></returns>
    public async UniTask StageClear(bool isAdvance)
    {
        await roomHub.StageClear(isAdvance);
    }

    /// <summary>
    /// ステージ進行完了の同期
    /// </summary>
    /// <returns></returns>
    public async UniTask AdvancedStageAsync()
    {
        await roomHub.AdvancedStageAsync();
    }

    /// <summary>
    /// オブジェクト生成リクエスト
    /// </summary>
    /// <returns></returns>
    public async UniTask SpawnObjectAsync(Vector3 spawnPos)
    {
        await roomHub.SpawnObjectAsync(spawnPos);
    }

    /// <summary>
    /// オブジェクト更新リクエスト
    /// </summary>
    /// <returns></returns>
    public async UniTask UpdateObjectAsync(Vector3 pos,Quaternion rot,string uniqueId)
    {
        await roomHub.UpdateObjectAsync(pos,rot,uniqueId);
    }

    /// <summary>
    /// オブジェクトの削除
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public async UniTask DeliteObjectAsync(string objName)
    {
        await roomHub.DeliteObjectAsync(objName);
    }

    /// <summary>
    /// オブジェクト所有権変更
    /// </summary>
    /// <returns></returns>
    public async UniTask ObjectOwnershipSwapAsync(string uniqueId, int joinOrder)
    {
        await roomHub.OwnershipSwapObjectAsync(uniqueId,joinOrder);
    }

    /// <summary>
    /// オブジェクト所有権変更
    /// </summary>
    /// <returns></returns>
    public async UniTask CountAsync(bool isTask)
    {
        await roomHub.CountAsync(isTask);
    }

    public async Task GameEndAsync()
    {
        await roomHub.GameEndAsync();
    }

    #endregion


    #endregion

    #endregion
}
