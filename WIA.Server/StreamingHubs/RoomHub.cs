//=============================
// クライアントからサーバーへの通信を管理するスクリプト
// Author:木田晃輔
//=============================

#region using一覧
using Grpc.Core;
using MagicOnion.Server.Hubs;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WIA.Server.Model.Context;
using WIA.Server.StreamingHubs;
using Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;
using static System.Net.Mime.MediaTypeNames;
using WIA.Shared.Interfaces.Model.Entity;
using WIA.Shared.Interfaces.StreamingHubs;
#endregion

namespace WIA.Server.StreamingHubs
{
    public class RoomHub(RoomContextRepository roomContextRepository) : StreamingHubBase<IRoomHub, IRoomHubReceiver>, IRoomHub
    {
        //コンテキスト定義
        private RoomContext roomContext;
        RoomContextRepository roomContextRepos;
        Dictionary<Guid, JoinedUser> JoinedUsers { get; set; }

        // 参加可能人数
        private const int MAX_JOINABLE_PLAYERS = 3;

        // ステータス上限定数
        private const float LVUP_HP = 0.06f;
        private const float LVUP_POW = 0.05f;
        private const float LVUP_DEF = 0.01f;
        private const float MAX_ATTACKSPEED = 1.15f;
        private const float MAX_REGENERATE = 0.065f;

        // ターミナル関連定数 (MAXの値はRandで用いるため、上限+1の数)
        private const int MIN_TERMINAL_ID = 1;
        private const int MAX_TERMINAL_ID = 6;

        // レリック関連定数
        private const int MAX_DAMAGE = 99999;

        #region 接続・切断処理
        //接続した場合
        protected override ValueTask OnConnected()
        {
            roomContextRepos = roomContextRepository;
            return default;
        }

        // 切断された場合
        protected override ValueTask OnDisconnected()
        {
            // 退室処理を実行
            LeavedAsync(false); return CompletedTask;
        }
        #endregion

        #region マッチングしてからゲーム開始までの処理
        /// <summary>
        /// 入室処理
        /// Author:Kida
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Dictionary<Guid, JoinedUser>> JoinedAsync(int userId)
        {
            lock (roomContextRepository)
            { //同時に生成しないように排他制御

                GameDbContext dbContext = new GameDbContext();

                //DBからユーザー情報取得
                //var user = dbContext.Users.Where(user => user.Id == userId).First();

                //ユーザーデータを設定(Steam対応デバッグ用)
                User userSteam = new User();
                userSteam.Id = userId;


                // ルームに参加＆ルームを保持
                this.roomContext = roomContextRepository.GetContext("Sample");
                if (this.roomContext == null)
                { //無かったら生成
                    this.roomContext = roomContextRepository.CreateContext("Sample");

                    //if(gameMode != 0)
                    //{
                    //    //DBに生成
                    //    room.roomName = roomName;
                    //    room.userName = userName;
                    //    room.password = pass;
                    //    room.is_started = false;
                    //    roomService.RegistRoom(room.roomName, room.userName, room.password, gameMode);
                    //}
                    this.roomContext.IsStartGame = false;
                }
                this.roomContext.Group.Add(this.ConnectionId, Client);

                // グループストレージにユーザーデータを格納
                var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = userSteam };

                if (roomContext.JoinedUserList.Count == 0)
                {//roomContext内の参加人数が0である場合

                    //参加順番の初期化
                    joinedUser.JoinOrder = 1;

                    //1人目をマスタークライアントにする
                    joinedUser.IsMaster = true;
                }
                else
                {
                    //参加順番の設定
                    joinedUser.JoinOrder = roomContext.JoinedUserList.Count + 1;
                }

                // ルームコンテキストに参加ユーザーを保存
                this.roomContext.JoinedUserList[this.ConnectionId] = joinedUser;

                //this.roomContext.Group.Only([this.ConnectionId]).OnRoom();

                //　ルームに参加
                this.roomContext.Group.Except([this.ConnectionId]).Onjoin(roomContext.JoinedUserList[this.ConnectionId]);

                // 参加中のユーザー情報を返す
                return this.roomContext.JoinedUserList;
            }
        }

        /// <summary>
        /// 退室処理
        /// Author:Kida
        /// </summary>
        /// <returns></returns>
        public async Task LeavedAsync(bool isEnd)
        {
            lock (roomContextRepository) // 排他制御
            {
                // Nullチェック入れる
                if (this.roomContext == null) return;
                if (!this.roomContext.JoinedUserList.ContainsKey(this.ConnectionId)) return;

                GameDbContext context = new GameDbContext();

                //　退室するユーザーを取得
                var joinedUser = this.roomContext.JoinedUserList[this.ConnectionId];

                if (isEnd == false)
                {
                    ////マスタークライアントだったら次の人に譲渡する
                    if (joinedUser.IsMaster == true)
                    {
                        //MasterLostAsync(this.ConnectionId);
                        //foreach (var user in this.roomContext.JoinedUserList)
                        //{
                        //    if (user.Value.IsMaster == true)
                        //    {
                        //        this.roomContext.Group.Only([user.Key]).OnChangeMasterClient();
                        //    }
                        //}
                    }

                }

                ////最後の1人の場合ルームを削除
                //if (this.roomContext.JoinedUserList.Count == 1)
                //{
                //    roomService.RemoveRoom(this.roomContext.Name);
                //}

                //// ルーム参加者全員に、ユーザーの退室通知を送信
                this.roomContext.Group.All.OnLeave(roomContext.JoinedUserList, this.ConnectionId);

                //　ルームから退室
                this.roomContext.Group.Remove(this.ConnectionId);

                //コンテキストからユーザーを削除
                roomContext.RemoveUser(this.ConnectionId);


                //// ルームデータから自身のデータを削除
                //roomContext.characterDataList.Remove(this.ConnectionId);;

                //ゲームが始まっていないなら実行しない
                if (roomContext.IsStartGame == false) return;

                // 全滅判定変数
                bool isAllDead = true;

                //foreach (var player in this.roomContext.characterDataList)
                //{
                //    if (player.Value.IsDead == false) // もし誰かが生きていた場合
                //    {
                //        isAllDead = false;
                //        break;
                //    }
                //}

                // 全滅した場合、ゲーム終了通知を全員に出す
                if (isAllDead)
                {
                    //Result();
                }
            }
        }

        /// <summary>
        /// キャラクター変更
        ///  Author:木田晃輔
        /// </summary>
        /// <returns></returns>
        public async Task ChangeCharacterAsync(int characterId)
        {
            lock (roomContextRepository) //排他制御
            {
                this.roomContext.JoinedUserList[this.ConnectionId].CharacterID = characterId;
                this.roomContext.Group.All.OnChangeCharacter(this.ConnectionId, characterId);
            }
        }

        /// <summary>
        /// 準備完了
        /// Author:Nishiura
        /// </summary>
        /// <returns></returns>
        public async Task ReadyAsync(int characterId)
        {
            lock (roomContextRepository) // 排他制御
            {
                bool canStartGame = true; // ゲーム開始可能判定変数

                // 自身のデータを取得
                var joinedUser = roomContext.JoinedUserList[this.ConnectionId];
                joinedUser.IsReady = true; // 準備完了にする
                joinedUser.CharacterID = characterId; //キャラクターIDを保存

                // ルーム参加者全員に、自分が準備完了した通知を送信
                this.roomContext.Group.All.OnReady(joinedUser);

                foreach (var user in this.roomContext.JoinedUserList)
                { // 現在の参加者数分ループ
                    if (user.Value.IsReady != true) canStartGame = false; // もし一人でも準備完了していなかった場合、開始させない
                }
                // 難易度を初期値にする
                this.roomContext.NowDifficulty = 0;

                // ゲームが開始できる場合、開始通知をする
                if (canStartGame)
                {
                    this.roomContext.Group.All.OnStartGame();

                    this.roomContext.IsStartGame = true;

                    // 現在時刻を代入
                    this.roomContext.startTime = DateTime.Now;
                }
            }
        }
        #endregion

        #region ゲーム内での処理

        /// <summary>
        /// プレイヤーの更新
        /// Author:Nishiura
        /// </summary>
        /// <param name="playerData"></param>
        /// <returns></returns>
        //public async Task UpdatePlayerAsync(PlayerData playerData)
        //{
        //    lock (roomContextRepository) // 排他制御
        //    {
        //        // キャラクターデータリストに自身のデータがない場合
        //        if (!this.roomContext.characterDataList.ContainsKey(this.ConnectionId))
        //        {
        //            // 新たなキャラクターデータを追加
        //            this.roomContext.AddCharacterData(this.ConnectionId, playerData);
        //        }
        //        else // 既に存在している場合
        //        {
        //            // キャラクターデータを更新
        //            this.roomContext.characterDataList[this.ConnectionId] = playerData;
        //        }

        //        // ルームの自分以外に、ユーザ情報通知を送信
        //        this.roomContext.Group.Except([this.ConnectionId]).OnUpdatePlayer(playerData);
        //    }
        //}

        /// <summary>
        /// マスタークライアントの更新
        /// Author:木田晃輔
        /// </summary>
        /// <param name="masterClientData"></param>
        /// <returns></returns>
        public async Task UpdateMasterClientAsync(MasterClientData masterClientData)
        {
            lock (roomContextRepository) // 排他制御
            {
                //// ルームデータから敵のリストを取得し、該当する要素を更新する
                //var gottenEnemyDataList = this.roomContext.enemyDataList;
                //foreach (var enemyData in masterClientData.EnemyDatas)
                //{
                //    if (gottenEnemyDataList.ContainsKey(enemyData.UniqueId))
                //    {
                //        gottenEnemyDataList[enemyData.UniqueId] = enemyData;
                //    }
                //}

                //// ルームデータから端末情報を取得し、アクティブ状態の端末を更新
                //if(masterClientData.TerminalDatas != null)
                //{
                //    foreach (var termData in masterClientData.TerminalDatas)
                //    {
                //        if(termData.State == TERMINAL_STATE.Active)
                //        {
                //            var data = roomContext.terminalList.FirstOrDefault(t => t.ID == termData.ID);

                //            if (data != null)
                //            {
                //                data.Time = termData.Time;
                //            }
                //        }
                //    }
                //}

                //foreach (var item in masterClientData.GimmickDatas)
                //{
                //    // すでにルームコンテキストにギミックが含まれている場合
                //    if (this.roomContext.gimmickList.ContainsKey(item.UniqueID))
                //    {
                //        // そのギミックを更新する
                //        this.roomContext.gimmickList[item.UniqueID] = item;
                //    }
                //    else // 含まれていない場合
                //    {
                //        // そのギミックを追加する
                //        this.roomContext.gimmickList.Add(item.UniqueID, item);
                //    }
                //}

                //// キャラクターデータリストに自身のデータがない場合
                //if (!this.roomContext.characterDataList.ContainsKey(this.ConnectionId))
                //{
                //    // 新たなキャラクターデータを追加
                //    this.roomContext.AddCharacterData(this.ConnectionId, masterClientData.PlayerData);
                //}
                //else // 既に存在している場合
                //{
                //    // キャラクターデータを更新
                //    this.roomContext.characterDataList[this.ConnectionId] = masterClientData.PlayerData;
                //}

                // ルームの自分以外に、マスタークライアントの状態の更新通知を送信
                this.roomContext.Group.Except([this.ConnectionId]).OnUpdateMasterClient(masterClientData);
            }
        }


        /// <summary>
        /// ギミック起動同期処理
        /// Autho:Nishiura
        /// </summary>
        /// <param name="gimID">ギミック識別ID</param>
        /// <returns></returns>
        public async Task BootGimmickAsync(string uniqueID, bool triggerOnce)
        {
            lock (roomContextRepository)
            {
                //// 対象ギミックが存在している場合
                //if (this.roomContext.gimmickList.ContainsKey(uniqueID))
                //{
                //    if (triggerOnce)
                //    {
                //        this.roomContext.gimmickList.Remove(uniqueID);
                //    }

                //    // 参加者全員にギミック情報を通知
                //    this.roomContext.Group.All.OnBootGimmick(uniqueID, triggerOnce);
                //}
            }
        }


        /// <summary>
        /// 次ステージ進行同期処理
        /// Autho:Nishiura
        /// </summary>
        /// <param name="conID">接続ID</param>
        /// <param name="isAdvance">ステージ進行判定</param>
        /// <returns></returns>
        public async Task StageClear(bool isAdvance)
        {
            // すでに申請済みの場合処理しない
            if (this.roomContext.isAdvanceRequest == true) return;
            lock (roomContextRepository) // 排他制御
            {
                // 進行申請を申請済みにするにする
                this.roomContext.isAdvanceRequest = true;
                this.roomContext.totalClearStageCount++;

                if (isAdvance)
                {
                    //if((int)this.roomContext.NowStage == 4)
                    //{
                    //    this.roomContext.NowStage = STAGE_TYPE.Rust;
                    //}else this.roomContext.NowStage++; // 現在のステージを加算

                    //// 獲得したアイテムリストをクリア
                    //this.roomContext.gottenItemList.Clear();

                    //// 生成した端末リストをクリア
                    //this.roomContext.terminalList.Clear();

                    //// 生成した敵のリストを初期化
                    //this.roomContext.enemyDataList.Clear();

                    //// 参加者全員にステージの進行を通知
                    //this.roomContext.Group.All.OnAdanceNextStage(this.roomContext.NowStage);

                    //// 各進行判定変数の値をfalseにする
                    //this.roomContext.JoinedUserList[this.ConnectionId].IsAdvance = false;
                }
                else
                {
                    // ゲーム終了を全員に通知
                    //Result();
                }
            }
        }

        /// <summary>
        /// ステージ進行完了同期処理
        /// </summary>
        /// <param name="conID">接続ID</param>
        /// <param name="isAdvance">ステージ進行判定</param>
        /// <returns></returns>
        public async Task AdvancedStageAsync()
        {
            lock (roomContextRepository)
            {
                bool canAdvenceStage = true; // ステージ進行済み判定変数

                // 自身のデータを取得
                var joinedUser = roomContext.JoinedUserList[this.ConnectionId];
                joinedUser.IsAdvance = true; // 準備完了にする

                foreach (var user in this.roomContext.JoinedUserList)
                { // 現在の参加者数分ループ
                    if (user.Value.IsAdvance != true) canAdvenceStage = false; // もし一人でも準備完了していなかった場合、進行させない
                }

                // 進行できる場合、進行通知をする
                if (canAdvenceStage)
                {
                    //// 端末データを抽選・保存
                    //var terminals = LotteryTerminal();
                    //foreach (var terminal in terminals)
                    //{
                    //    this.roomContext.terminalList.Add(terminal);
                    //}

                    //// 同期開始通知と共に端末データを送信
                    //this.roomContext.Group.All.OnSameStart(terminals);

                    //joinedUser.IsAdvance = false; // 準備完了を解除する
                    //canAdvenceStage = false;
                    //roomContext.isAdvanceRequest = false;
                }
            }
        }

        /// <summary>
        /// オブジェクト生成処理
        /// </summary>
        /// <returns></returns>
        public async Task SpawnObjectAsync(Vector2 spawnPos)
        {
            lock (roomContextRepository)
            {
                string uniqueId = Guid.NewGuid().ToString();
                GimmickData gimmickData = new GimmickData()
                {
                    UniqueID = uniqueId,
                    Position = spawnPos,
                };
                //this.roomContext.gimmickList.Add(uniqueId, gimmickData);
                this.roomContext.Group.All.OnSpawnObject(spawnPos, uniqueId);
            }
        }


        /// <summary>
        /// プレイヤー死亡同期処理
        /// Author:Nishiura
        /// </summary>
        /// <param name="conID">プレイヤーID</param>
        /// <returns></returns>
        //public async Task<PlayerDeathResult> PlayerDeadAsync()
        //{
        //    lock (roomContextRepository) // 排他制御
        //    {
        //        PlayerDeathResult resultData = new PlayerDeathResult()
        //        {
        //            BuckupHDMICnt = 0,
        //            IsDead = true
        //        };

        //        // 蘇生アイテムを持っているかチェック
        //        var relicStatusData = this.roomContext.playerStatusDataList[this.ConnectionId].Item2;
        //        if (relicStatusData.BuckupHDMICnt > 0)
        //        {
        //            // 蘇生アイテムを消費して全回復して復活する
        //            relicStatusData.BuckupHDMICnt--;
        //            this.roomContext.characterDataList[this.ConnectionId].State.hp = this.roomContext.characterDataList[this.ConnectionId].Status.hp;

        //            resultData.BuckupHDMICnt = relicStatusData.BuckupHDMICnt;
        //            resultData.IsDead = false;
        //            return resultData;
        //        }

        //        // 生存時間を登録
        //        this.roomContext.resultDataList[this.ConnectionId].AliveTime = DateTime.Now - this.roomContext.startTime;

        //        // 全滅判定変数
        //        bool isAllDead = true;
        //        // ルームデータから接続IDを指定して自身のデータを取得
        //        var playerData = this.roomContext.characterDataList[this.ConnectionId].IsDead = true;

        //        foreach (var player in this.roomContext.characterDataList)
        //        {
        //            if (player.Value.IsDead == false) // もし誰かが生きていた場合
        //            {
        //                isAllDead = false;
        //                break;
        //            }
        //        }

        //        // 死亡者以外の参加者全員に対象者が死亡したことを通知
        //        this.roomContext.Group.Except([this.ConnectionId]).OnPlayerDead(this.ConnectionId);

        //        // 全滅した場合、ゲーム終了通知を全員に出す
        //        if (isAllDead)
        //        {
        //            Result();
        //        }

        //        return resultData;
        //    }
        //}

        /// <summary>
        /// ビームエフェクト通知処理
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        async Task BeamEffectActiveAsync(bool isActive)
        {
            // 自分以外にビームエフェクトの状態を通知
            this.roomContext.Group.Except([this.ConnectionId]).OnBeamEffectActive(this.ConnectionId, isActive);
        }

        /// <summary>
        /// アイテム獲得同期処理
        /// Author:Nishiura
        /// </summary>
        /// <param name="itemType">アイテムの種類</param>
        /// <param name="itemID">識別ID(文字列)</param>
        /// <returns></returns>
        async Task GetItemAsync(EnumManager.ITEM_TYPE itemType, string itemID)
        {
            lock (roomContextRepository) // 排他制御
            {
                // すでに取得済みである場合、処理しない
                if (this.roomContext.gottenItemList.Contains(itemID)) return;

                int getExp = 0;

                switch (itemType)   // アイテムの種類に応じて処理を分ける
                {
                    //case EnumManager.ITEM_TYPE.Relic:       // レリックの場合
                    //        var relic = this.roomContext.dropRelicDataList[itemID];

                    //        // DBからレリック情報取得
                    //        GameDbContext dbContext = new GameDbContext();
                    //        var relicData = dbContext.Relics.Where(data => data.id == (int)relic.RelicType).First();

                    //        if (!this.roomContext.relicDataList.ContainsKey(this.ConnectionId)) this.roomContext.relicDataList[this.ConnectionId] = new List<Relic>();

                    //        // 取得したレリックをリストに入れる
                    //        this.roomContext.relicDataList[this.ConnectionId].Add(relicData);

                    //        // リザルトデータを更新
                    //        this.roomContext.resultDataList[this.ConnectionId].GottenRelicList.Add((RELIC_TYPE)relicData.id);

                    //        // レリック強化を付与
                    //        GetStatusWithRelics();
                    //        break;

                    //    case EnumManager.ITEM_TYPE.DataCube:    // データキューブの場合
                    //        getExp = (int)(this.roomContext.ExpManager.RequiredExp * 0.1f);
                    //        this.roomContext.ExpManager.nowExp += getExp == 0 ? 1 : getExp; // 要求経験値の5%を渡す(0の場合は1を渡す)
                    //        break;

                    //    case EnumManager.ITEM_TYPE.DataBox:     // データボックスの場合
                    //        getExp = (int)(this.roomContext.ExpManager.RequiredExp * 0.4f);
                    //        this.roomContext.ExpManager.nowExp += getExp == 0 ? 1 : getExp; // 要求経験値の25%を渡す(0の場合は1を渡す)
                    //        break;
                    //}

                    //// 所持経験値が必要経験値に満ちた場合
                    //if (this.roomContext.ExpManager.nowExp >= this.roomContext.ExpManager.RequiredExp)
                    //{
                    //    LevelUp(roomContext.ExpManager); // レベルアップ処理
                    //}

                    //// 取得済みアイテムリストに入れる
                    //this.roomContext.gottenItemList.Add(itemID);

                    //// リザルトデータを更新
                    //this.roomContext.resultDataList[this.ConnectionId].TotalGottenItem++;

                    //// アイテムの獲得を全員に通知
                    //ExpManager expManager = this.roomContext.ExpManager;
                    //this.roomContext.Group.All.OnGetItem(this.ConnectionId, itemID, expManager.Level, expManager.nowExp, expManager.RequiredExp);
                }
            }
        }

        /// <summary>
        /// 弾発射同期処理
        /// Author:Nishiura
        /// </summary>
        /// <param name="spawnPos">生成位置</param>
        /// <param name="shootVec">発射ベクトル</param>
        // async Task ShootBulletsAsync(params ShootBulletData[] shootBulletDatas)
        //{
        //    // 参加者全員に端末の結果を通知
        //    this.roomContext.Group.All.OnShootBullets(shootBulletDatas);
        //}

        #endregion

        /// <summary>
        /// マスタークライアント譲渡処理
        /// Author:Nishiura
        /// </summary>
        /// <param name="conID"></param>
        /// <returns></returns>
        //void MasterLostAsync(Guid conID)
        //            {
        //                // 参加者リストをループ
        //                foreach (var user in this.roomContext.JoinedUserList)
        //                {
        //                    // 対象がマスタークライアントでない場合
        //                    if (user.Value.IsMaster == false)
        //                    {
        //                        // その対象をマスタークライアントとし、通知を送る。ループを抜ける
        //                        user.Value.IsMaster = true;
        //                        //this.roomContext.Group.Only([user.Key]).OnChangeMasterClient();
        //                        break;
        //                    }
        //                }

        //                // マスタークライアントを剥奪
        //                this.roomContext.JoinedUserList[conID].IsMaster = false;
        //            }


        public async Task GameEndAsync()
        {
            roomContext.JoinedUserList[this.ConnectionId].isEnd = true;

            foreach(var joinUser in roomContext.JoinedUserList)
            {
                if (joinUser.Value.isEnd == false) return;
            }
            Result();
        }

        /// <summary>
        /// リザルト作成処理
        /// Author:Nishiura
        /// </summary>
        /// <returns></returns>
        public async void Result()
        {
            if (this.roomContext.JoinedUserList.Count == 0) return;
            foreach (var conectionId in this.roomContext.JoinedUserList.Keys)
            {
                //var playerData = this.roomContext.characterDataList[conectionId];
                //var resultData = this.roomContext.resultDataList[conectionId];
                var resultData = new ResultData();

                //スコアを仮で設定
                if (roomContext.JoinedUserList[conectionId].UserData.Id ==0)
                {
                    resultData.TotalScore = 100;
                }
                else
                {
                    resultData.TotalScore = 200;
                }

                resultData.UserId = this.roomContext.JoinedUserList[conectionId].UserData.Id;
                //// 必要な残りのデータを代入
                //resultData.PlayerClass = playerData.Class;
                //resultData.TotalClearStageCount = this.roomContext.totalClearStageCount;
                //resultData.DifficultyLevel = this.roomContext.NowDifficulty;
                //if (resultData.AliveTime == TimeSpan.Zero) resultData.AliveTime = DateTime.Now - this.roomContext.startTime;

                //// 合計スコア
                //resultData.TotalScore = (resultData.TotalGottenItem * 10) +
                //            (resultData.TotalActivedTerminal * 10) +
                //            (resultData.EnemyKillCount * 10) +
                //            (resultData.TotalGaveDamage * 2) +
                //            (resultData.TotalClearStageCount * 100);
                //resultData.TotalScore *= resultData.DifficultyLevel > 2 ? resultData.DifficultyLevel / 2 : 1;

                this.roomContext.Group.Single(conectionId).OnGameEnd(resultData);
            }
        }
    }
}
