//=============================
// ルームコンテキストスクリプト
// Author:木田晃輔
//=============================
using Cysharp.Runtime.Multicast;
using WIA.Server.StreamingHubs;
using Shared.Interfaces.StreamingHubs;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using WIA.Shared.Interfaces.StreamingHubs;

namespace WIA.Server.Model.Context
{
    public class RoomContext
    {
        #region RoomContext基本構造
        /// <summary>
        /// コンテキストID
        /// Author:Kida
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// ルーム名
        /// Author:Kida
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// パスワード
        /// Author:Kida
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 難易度
        /// Author:Nishiura
        /// </summary>
        public int NowDifficulty { get; set; }


        /// <summary>
        /// ステージ進行リクエスト変数
        /// Author:Nishiura
        /// </summary>
        public bool isAdvanceRequest;

        /// <summary>
        /// 合計クリアステージ数
        /// Author:Nishiura
        /// </summary>
        public int totalClearStageCount = 0;

        /// <summary>
        /// ゲーム開始時の時刻
        /// </summary>
        public DateTime startTime;

        /// <summary>
        /// グループ
        /// Author:Kida
        /// </summary>
        public IMulticastSyncGroup<Guid, IRoomHubReceiver> Group { get; }

        /// <summary>
        /// ゲームスタート
        /// </summary>
        public bool IsStartGame { get; set; } = false;

        #region マスタデータ

        /// <summary>
        /// マスタデータを読み込み済みかどうか
        /// </summary>
        public bool IsLoadMasterDatas { get; private set; } = false;


        #endregion

        #endregion

        #region コンテキストに保存する情報のリスト一覧
        /// <summary>
        /// 参加者リスト
        /// Author:Kida
        /// </summary>
        public Dictionary<Guid, JoinedUser> JoinedUserList { get; } = new Dictionary<Guid, JoinedUser>();

        /// <summary>
        /// キャラクターデータリスト
        /// Author:Nishiura
        /// </summary>
        public Dictionary<Guid, PlayerData> characterDataList = new Dictionary<Guid, PlayerData>();

        /// <summary>
        /// 取得アイテムリスト
        /// Author:Nishiura
        /// </summary>
        public List<string> gottenItemList { get; } = new List<string>();

        /// <summary>
        /// リザルトデータリスト
        /// </summary>
        public Dictionary<Guid, ResultData> resultDataList { get; } = new Dictionary<Guid, ResultData>();

        #endregion

        //RoomContextの定義
        public RoomContext(IMulticastGroupProvider groupProvider, string roomName)
        {
            Id = Guid.NewGuid();
            Name = roomName;
            Group =
                groupProvider.GetOrAddSynchronousGroup<Guid, IRoomHubReceiver>(roomName);
        }

        #region 独自関数



        /// <summary>
        /// グループ退室処理
        /// Author:木田晃輔
        /// </summary>
        public void Dispose()
        {
            Group.Dispose();
        }

        /// <summary>
        /// ユーザーの退出処理
        /// Aughter:Kida
        /// </summary>
        /// <returns></returns>
        public void RemoveUser(Guid guid)
        {
            int joinOrder = 1;
            if (JoinedUserList != null)
            { //参加者リストが存在している場合
                // 退出したユーザーを特定して削除
                JoinedUserList.Remove(guid);
                foreach (var joinUser in JoinedUserList)
                {
                    joinUser.Value.JoinOrder = joinOrder;
                    joinOrder++;
                }
            }
        }
        #endregion
    }
}
