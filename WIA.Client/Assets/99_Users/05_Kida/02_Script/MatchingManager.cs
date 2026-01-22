////////////////////////////////////////////////////////////////
///
/// マッチング画面の処理を管理するスクリプト
/// 
/// Aughter:木田晃輔
///
////////////////////////////////////////////////////////////////

#region using一覧
using Cysharp.Net.Http;
using DG.Tweening;

//using Cysharp.Threading.Tasks.Triggers;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;
using System.Collections;

#endregion

public class MatchingManager : MonoBehaviour
{
    #region [SerializeField]：木田晃輔
    //[SerializeField] GameObject userPrefab; //ユーザーの情報
    [SerializeField] Text inputFieldRoomName; //ルームの名前入力フィールド
    [SerializeField] Text inputFieldSerchRoomName; //ルームの名前入力フィールド
    [SerializeField] Text inputFieldCreatePassWord; //パスワードの作成フィールド
    [SerializeField] Text inputFieldPassWord; //パスワードの入力フィールド
    [SerializeField] Text roomSerchField;   //ルームの名前検索
    [SerializeField] GameObject roomPrefab; //ルームのプレハブ
    [SerializeField] GameObject Content;
    [SerializeField] Transform rooms;
    //[SerializeField] SceneConducter conducter;
    [SerializeField] GameObject CreateButton; //生成ボタン
    [SerializeField] GameObject PrivateUI;
    [SerializeField] GameObject[] ErrorUI;
    [SerializeField] GameObject fade;
    [SerializeField] GameObject roomModelPrefab;
    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);
    Color32 backMenuendColor = new Color32(255, 255, 255, 255);

    #endregion
    public List<GameObject> createdRoomList; //作られたルーム
    public int userID;
    EventSystem eventSystem;
    JoinedUser joinedUser;                  //このクライアントユーザーの情報
    Text text;
    [SerializeField] private TMP_Text dotText; // マッチング中のドット
    [SerializeField] private TMP_Text matchingText; // マッチング中/マッチング完了テキスト
    [SerializeField] private TMP_Text guideText; // ガイドテキスト

    BaseModel model;
    Text roomNameText; //ルームの名前
    Text userNameText; //ユーザーの名前
    Text passText;      //パスワード

    string joinRoomName;
    string roomSerchName;
    int errorId;

    int dotCount = 0;

    //入室か生成の判別用
    private static string joinMode;

    public static string JoinMode
    {
        get { return joinMode; }
    }

    //新マッチング用のユーザーID
    private static int userId;
    public static int UserID
    {
        get { return userId; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        //実装時にはこの変数でユーザーを判断する
        //userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        ////安全動作のための初回ローディング
        //conducter.Loading();

        InvokeRepeating("StartEffect", 0.5f, 0.8f);
        #region RoomModel定義
        Instantiate(roomModelPrefab).name = "RoomModel";
        await RoomModel.Instance.ConnectAsync();

        RoomModel.Instance.OnFailedJoinSyn += this.OnFailedJoinSyn;
        RoomModel.Instance.OnMatched += this.OnMatched;
        //ユーザーが入室した時にOnJoinedUserメソッドを実行するよう、モデルに登録
        RoomModel.Instance.OnJoinedUser += this.OnJoinedUser;
        RoomModel.Instance.OnLeavedUser += this.OnLeavedUser;
        RoomModel.Instance.OnReadySyn += this.OnReadySyn;
        RoomModel.Instance.OnStartedGame += this.OnStartedGame;
        #endregion

        //if (SceneManager.GetActiveScene().name == "MatchingScene")
            InvokeRepeating("Matching",0,1f);
    }

    private void OnDisable()
    {
        //シーン遷移した場合に通知関数をモデルから解除
        RoomModel.Instance.OnFailedJoinSyn -= this.OnFailedJoinSyn;
        RoomModel.Instance.OnMatched -= this.OnMatched;
        RoomModel.Instance.OnJoinedUser -= this.OnJoinedUser;
        RoomModel.Instance.OnLeavedUser -= this.OnLeavedUser;
        RoomModel.Instance.OnReadySyn -= this.OnReadySyn;
        RoomModel.Instance.OnStartedGame -= this.OnStartedGame;
    }

    async void Matching()
    {
        Debug.Log("マッチング中");
        await RoomModel.Instance.MatchingAsync();
    }


    public async void Ready()
    {
        await RoomModel.Instance.ReadyAsync(1);
    }

    public void ReturnTitle()
    {
        Initiate.DoneFading();
        Initiate.Fade("1_TitleScene", Color.black, 1.0f);   // フェード時間1秒
    }

    public void ErrorClose()
    {
        ErrorUI[errorId].SetActive(false);
    }


    private void Loaded()
    {
        //conducter.Loaded();
    }

    #region 同期処理一覧：木田晃輔

    /// <summary>
    /// ルーム作成
    /// </summary>
    public async void CreateRoom()
    {
        //conducter.Loading();

        if (Re_RoomManager.IsCreater == true)
        {//ルーム作成の場合
            passText = inputFieldCreatePassWord;
            roomNameText = inputFieldRoomName;
            if (roomNameText.text == "")
            {
                errorId = 2;
                OnFailedJoinSyn(errorId);
                Invoke("Loaded", 1.0f);
            }
            else
            {
                joinMode = "create";
                //await RoomModel.Instance.JoinedAsync(roomNameText.text, userId, TitleManagerk.SteamUserName, passText.text,TitleManagerk.GameMode);
            }
        }
    }

    /// <summary>
    /// 入室処理
    /// Aughter:木田晃輔
    /// </summary>
    public async void JoinRoom(string roomName)
    {
        userId = userID;
        await RoomModel.Instance.JoinedAsync(roomName);
    }

    /// <summary>
    /// 退室処理
    /// Aughter:木田晃輔
    /// </summary>
    public async void LeaveRoom()
    {
      await RoomModel.Instance.LeavedAsync();
    }

    /// <summary>
    /// プライベートルーム入室
    /// Aughter:木田晃輔
    /// </summary>
    public async void PrivateRoomJoin()
    {
        joinMode = "join";
        //conducter.Loading();
        string pass = inputFieldPassWord.text;
        //await RoomModel.Instance.JoinedAsync(joinRoomName, userId,TitleManagerk.SteamUserName, pass,TitleManagerk.GameMode);
    }

    #endregion

    #region 通知一覧：木田晃輔

    /// <summary>
    /// 入室失敗通知
    /// </summary>
    /// <param name="errorId"></param>
    public void OnFailedJoinSyn(int errorId)
    {
        this.errorId = errorId;
        if(this.errorId == 0) 
        {//参加可能人数超過
            ErrorUI[this.errorId].SetActive(true);
            //conducter.Loaded();
        }
        if(this.errorId == 1)
        {//パスワードが違う場合
            PrivateUI.SetActive(false);
            ErrorUI[this.errorId].SetActive(true);
            //conducter.Loaded();
        }
        if(this.errorId == 2)
        {//部屋名未入力
            PrivateUI.SetActive(false);
            ErrorUI[this.errorId].SetActive(true);
            //conducter.Loaded();
        }
        if(this.errorId == 3)
        {//部屋が存在しない
            PrivateUI.SetActive(false);
            ErrorUI[this.errorId].SetActive(true);
            //conducter.Loaded();
        }
    }

    //public void OnCreatedRoom()
    //{
    //    SceneManager.LoadScene("3_StandbyRoom");
    //}

    /// <summary>
    /// 入室完了通知
    /// Aughter:木田晃輔
    /// </summary>
    public void OnJoinedUser(JoinedUser joinedUser)
    {
        foreach (var data in RoomModel.Instance.joinedUserList.Values)
        {
            //入室したときの処理を書く
            Debug.Log(data.ConnectionId + "が入室しました。");

        }     
    }

    /// <summary>
    /// 退室通知
    /// </summary>
    public void OnLeavedUser(JoinedUser joinedUser)
    {
        //退室したときの処理を書く
        Debug.Log(joinedUser.ConnectionId + "が退室しました。");
    }

    public void OnReadySyn(Guid guid)
    {
        //準備完了したときの処理を書く
        Debug.Log(guid + "が準備完了！！");
    }

    public void OnStartedGame()
    {
        //ゲーム開始時の処理をする
        // シーン遷移

        Initiate.DoneFading(); 
        if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 1)
        {
            Initiate.Fade("01_Exp_Worker_1", endColor, 2.0f);
        }
        else if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 2)
        {
            Initiate.Fade("01_Exp_Stricker_1", endColor, 2.0f);
        }
    }

    public async void OnMatched(string roomName)
    {
        Debug.Log("マッチングしました");
        LeaveRoom();
        CancelInvoke("Matching");
        await RoomModel.Instance.JoinedAsync(roomName);

        // テキストの内容を変更する
        matchingText.text = "マッチング完了";
        guideText.text = "マッチングしました。まもなく開始します。";

        // dotTextを削除する
        Destroy(dotText);

        //OnStartedGameを呼び出す
        //OnStartedGame();
        Invoke("OnStartedGame", 2); // 2秒後にゲーム開始

        //Initiate.DoneFading();
        //Initiate.Fade("PreMatchingScene", endColor, 2.0f);
    }
    #endregion

    /// <summary>
    /// メニュー画面に戻る処理
    /// </summary>
    public void BackMenu()
    {
        //メニューシーンに遷移する
        if (UnityEngine.XR.XRSettings.isDeviceActive) Initiate.Fade("VR_02_MenuScene", backMenuendColor, 2.0f); //VR時
        else Initiate.Fade("02_MenuScene", backMenuendColor, 2.0f);
    }

    // 繰り返しのためのメソッド（例：ボタンクリック時など）
    public void StartEffect()
    {
        switch(dotCount)
        {
            case 0:
                dotText.text = ".";
                dotCount++;
                    break;

            case 1:
                dotText.text = "..";
                dotCount++;
                break;

            case 2:
                dotText.text = "...";
                dotCount++;
                break;

            case 3:
                dotText.text = "";
                dotCount = 0;
                break;
        }
    }
}
