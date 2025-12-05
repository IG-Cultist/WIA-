//=============================
// クライアントからサーバーへの通信を管理するスクリプト
// Author:木田晃輔
//=============================

using MagicOnion;
using WIA.Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.EnumManager;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub:IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        //ここにクライアント～サーバー定義

        #region 入室からゲーム開始まで

        /// <summary>
        /// 接続ID取得
        /// </summary>
        /// <returns></returns>
        Task<Guid> GetConnectionIdAsync();

        /// <summary>
        /// ユーザー入室
        /// Author:Kida
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, JoinedUser>> JoinedAsync(int userId);

        /// <summary>
        /// ユーザー退室
        /// Author:Kida
        /// </summary>
        /// <returns></returns>
        Task LeavedAsync(bool isEnd);

        /// <summary>
        /// 準備完了
        /// Author:Nishiura
        /// </summary>
        /// <returns></returns>
        Task ReadyAsync(int characterID);

        #endregion

        #region ゲーム内
        #region プレイヤー関連

        /// <summary>
        /// プレイヤーの更新
        /// Author:木田晃輔
        /// </summary>
        /// <param name="playerData"></param>
        /// <returns></returns>
        Task UpdatePlayerAsync(Vector3 pos,Quaternion rot,int animState, float moveSpeed);

        ///// <summary>
        ///// プレイヤー死亡同期
        ///// Author:木田晃輔
        ///// </summary>
        ///// <returns></returns>
        //Task PlayerDeadAsync();

        ///// <summary>
        ///// プレイヤーリスポーン同期
        ///// Author:木田晃輔
        ///// </summary>
        ///// <returns></returns>
        //Task PlayerRespownAsync();


        #endregion
        #region ゲーム内UI、仕様関連

        /// <summary>
        /// ギミック起動
        /// Author:Nishiura
        /// </summary>
        /// <param name="uniqueID">ギミック識別ID</param>
        /// <param name="triggerOnce">一度しか起動できないかどうか</param>
        /// <returns></returns>
        Task BootGimmickAsync(string uniqueID, bool triggerOnce);

        /// <summary>
        /// ステージクリア
        /// Author:Nishiura
        /// </summary>
        /// <param name="isAdvance">ステージ進行判定</param>
        /// <returns></returns>
        Task StageClear(bool isAdvance);

        /// <summary>
        /// ステージ進行完了
        /// Author:Nishiura
        /// </summary>
        /// <returns></returns>
        Task AdvancedStageAsync();

        /// <summary>
        /// オブジェクト生成リクエスト
        /// </summary>
        /// <returns></returns>
        Task SpawnObjectAsync(Vector3 spawnPos);

        /// <summary>
        /// オブジェクト更新
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <returns></returns>
        Task UpdateObjectAsync(Vector3 pos,Quaternion rot,string uniqueId);

        /// <summary>
        /// オブジェクト所有権変更
        /// </summary>
        /// <returns></returns>
        Task OwnershipSwapObjectAsync(string uniqueId,int joinOrder);

        /// <summary>
        /// ゲーム終了同期
        /// </summary>
        /// <returns></returns>
        Task GameEndAsync();

        #endregion
        #endregion
    }
}
