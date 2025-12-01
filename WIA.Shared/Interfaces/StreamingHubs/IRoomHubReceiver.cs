////////////////////////////////////////////////////////////////
///
/// サーバーからクライアントへの通信を管理するスクリプト
/// 
/// Aughter:木田晃輔
///
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using MagicOnion;
using Shared.Interfaces.StreamingHubs;
using System.Collections.Concurrent;
using static Shared.Interfaces.StreamingHubs.EnumManager;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WIA.Shared.Interfaces.StreamingHubs;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        //ここにサーバー～クライアントの定義
        #region 入室からゲーム開始まで
        /// <summary>
        /// ルーム作成通知
        /// Author:Kida
        /// </summary>
        void OnRoom();

        /// <summary>
        /// 参加失敗の通知
        /// Author:Kida
        /// </summary>
        void OnFailedJoin(int errorId);

        /// <summary>
        /// ユーザーの入室通知
        /// Author:Kida
        /// </summary>
        /// <param name="joindUserList">参加者リスト</param>
        void Onjoin(JoinedUser joindUser);

        /// <summary>
        /// ユーザーの退室通知
        /// Author:Kida
        /// </summary>
        /// <param name="user">対象者</param>
        //void OnLeave(JoinedUser user);

        void OnLeave(Dictionary<Guid,JoinedUser> user,Guid targetUser);

        /// <summary>
        /// 準備完了通知
        /// </summary>
        /// <param name="conID">接続ID</param>
        void OnReady(JoinedUser joinedUser);

        /// <summary>
        /// ゲーム開始通知
        /// Author:Nishiura
        /// </summary>
        void OnStartGame();

        #endregion

        #region ゲーム内
        #region プレイヤー関連
        /// <summary>
        /// プレイヤー動作通知
        /// Author:Nishiura
        /// </summary>
        void OnUpdatePlayer(Vector3 pos,Quaternion rot);

        /// <summary>
        /// プレイヤー死亡通知
        /// Author:木田晃輔
        /// </summary>
        /// <param name="guid"></param>
        void OnPlayerDead(Guid guid);


        #endregion
        #region ゲーム内UI、仕様

        /// <summary>
        /// 同時開始通知
        /// Author:木田晃輔
        /// </summary>
        //void OnSameStart(List<TerminalData> list);

        /// <summary>
        /// ギミック起動通知
        /// Author:Nishiura
        /// </summary>
        /// <param name="gimID">ギミックID</param>
        void OnBootGimmick(string uniqueID, bool triggerOnce);

        /// <summary>
        /// 難易度上昇通知
        /// Author:Nishiura
        /// </summary>
        /// <param name="dif">増加後難易度</param>
        void OnAscendDifficulty(int dif);

        /// <summary>
        /// 次ステージ進行通知
        /// Author:Nishiura
        /// </summary>
        /// <param name="conID">接続ID</param>
        /// <param name="isAdvance">次ステージ進行判定</param>
        /// <param name="stageType">次ステージ</param>
        void OnAdanceNextStage(STAGE_TYPE stageType);

        /// <summary>
        /// オブジェクト生成通知
        /// </summary>
        /// <returns></returns>
        void OnSpawnObject(Vector3 spawnPos, string uniqueId);

        /// <summary>
        /// オブジェクト更新通知
        /// </summary>
        /// <returns></returns>
        void OnUpdateObject(Vector3 pos,Quaternion rot, string uniqueId);

        /// <summary>
        /// オブジェクト所有権変更通知
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="joinOrder"></param>
        void OnOwnershipSwapObject(string uniqueId,int joinOrder);

        #endregion
        #endregion

        /// <summary>
        /// ゲーム終了通知
        /// </summary>
        /// <param name="result"></param>
        void OnGameEnd(ResultData result);

        /// <summary>
        /// アイテム獲得通知
        /// </summary>
        /// <param name="conId">獲得したユーザーの接続ID</param>
        /// <param name="itemID">アイテムの識別用ID</param>
        /// <param name="nowLevel">現在の経験値</param>
        /// <param name="nowExp">現在のEXP</param>
        /// <param name="nextLevelExp">レベルアップに必要なEXP</param>
        void OnGetItem(Guid conId, string itemID, int nowLevel, int nowExp, int nextLevelExp);

    }
}
