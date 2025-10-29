////////////////////////////////////////////////////////////////
///
/// �}�b�`���O��ʂ̏������Ǘ�����X�N���v�g
/// 
/// Aughter:�ؓc�W��
///
////////////////////////////////////////////////////////////////

#region using�ꗗ
using Cysharp.Net.Http;
//using Cysharp.Threading.Tasks.Triggers;
using Grpc.Net.Client;
using MagicOnion.Client;
using NIGHTRAVEL.Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#endregion

public class MatchingManager : MonoBehaviour
{
    #region [SerializeField]�F�ؓc�W��
    //[SerializeField] GameObject userPrefab; //���[�U�[�̏��
    [SerializeField] Text inputFieldRoomName; //���[���̖��O���̓t�B�[���h
    [SerializeField] Text inputFieldSerchRoomName; //���[���̖��O���̓t�B�[���h
    [SerializeField] Text inputFieldCreatePassWord; //�p�X���[�h�̍쐬�t�B�[���h
    [SerializeField] Text inputFieldPassWord; //�p�X���[�h�̓��̓t�B�[���h
    [SerializeField] Text roomSerchField;   //���[���̖��O����
    [SerializeField] GameObject roomPrefab; //���[���̃v���n�u
    [SerializeField] GameObject Content;
    [SerializeField] Transform rooms;
    //[SerializeField] SceneConducter conducter;
    [SerializeField] GameObject CreateButton; //�����{�^��
    [SerializeField] GameObject PrivateUI;
    [SerializeField] GameObject[] ErrorUI;
    [SerializeField] GameObject fade;
    [SerializeField] GameObject roomModelPrefab;
    #endregion
    public List<GameObject> createdRoomList; //���ꂽ���[��
    EventSystem eventSystem;
    JoinedUser joinedUser;                  //���̃N���C�A���g���[�U�[�̏��
    Text text;
    BaseModel model;
    Text roomNameText; //���[���̖��O
    Text userNameText; //���[�U�[�̖��O
    Text passText;      //�p�X���[�h
    string joinRoomName;
    string roomSerchName;
    int errorId;

    //�����������̔��ʗp
    private static string joinMode;

    public static string JoinMode
    {
        get { return joinMode; }
    }

    //�V�}�b�`���O�p�̃��[�U�[ID
    private static int userId;
    public static int UserID
    {
        get { return userId; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        //�������ɂ͂��̕ϐ��Ń��[�U�[�𔻒f����
        //userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        ////���S����̂��߂̏��񃍁[�f�B���O
        //conducter.Loading();

        #region RoomModel��`
        //���[�����f��������Ȃ�폜
        //Destroy(GameObject.Find("RoomModel"));
        //Destroy(GameObject.Find("RoomModel(Clone)"));
        //Invoke("NewRoomModel", 0.3f);
        #endregion

        await RoomModel.Instance.ConnectAsync();
        RoomModel.Instance.OnCreatedRoom += this.OnCreatedRoom;
        RoomModel.Instance.OnFailedJoinSyn += this.OnFailedJoinSyn;
        //���[�U�[��������������OnJoinedUser���\�b�h�����s����悤�A���f���ɓo�^
        RoomModel.Instance.OnJoinedUser += this.OnJoinedUser;
        RoomModel.Instance.OnLeavedUser += this.OnLeavedUser;

    }

    private void OnDisable()
    {
        //�V�[���J�ڂ����ꍇ�ɒʒm�֐������f���������
        RoomModel.Instance.OnCreatedRoom -= this.OnCreatedRoom;
        RoomModel.Instance.OnFailedJoinSyn -= this.OnFailedJoinSyn;
        RoomModel.Instance.OnJoinedUser -= this.OnJoinedUser;
        RoomModel.Instance.OnLeavedUser -= this.OnLeavedUser;
    }

    //void NewRoomModel()
    //{
    //    if (GameObject.Find("RoomModel") != null) return;
    //    //���[�����f����������x�쐬
    //    Instantiate(roomModelPrefab);
    //    Invoke("Connecting", 0.3f);

    //}

    async void Connecting()
    {

    }

    //void SarchRoom()
    //{

    //    //���[������
    //    SerchRoom();

    //    //���[�f�B���O��~
    //    Invoke("Loaded", 2.0f);

    //}

    public void ReturnTitle()
    {
        Initiate.DoneFading();
        Initiate.Fade("1_TitleScene", Color.black, 1.0f);   // �t�F�[�h����1�b
    }

    public void ErrorClose()
    {
        ErrorUI[errorId].SetActive(false);
    }


    private void Loaded()
    {
        //conducter.Loaded();
    }

    #region ���������ꗗ�F�ؓc�W��

    /// <summary>
    /// ���[���쐬
    /// </summary>
    public async void CreateRoom()
    {
        //conducter.Loading();

        if (Re_RoomManager.IsCreater == true)
        {//���[���쐬�̏ꍇ
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
    /// ��������
    /// Aughter:�ؓc�W��
    /// </summary>
    public async void JoinRoom()
    {
      await RoomModel.Instance.JoinedAsync(1);
    }

    /// <summary>
    /// �ގ�����
    /// Aughter:�ؓc�W��
    /// </summary>
    public async void LeaveRoom()
    {
      await RoomModel.Instance.LeavedAsync();
    }

    /// <summary>
    /// �v���C�x�[�g���[������
    /// Aughter:�ؓc�W��
    /// </summary>
    public async void PrivateRoomJoin()
    {
        joinMode = "join";
        //conducter.Loading();
        string pass = inputFieldPassWord.text;
        //await RoomModel.Instance.JoinedAsync(joinRoomName, userId,TitleManagerk.SteamUserName, pass,TitleManagerk.GameMode);
    }

    #endregion

    #region �ʒm�ꗗ�F�ؓc�W��

    /// <summary>
    /// �������s�ʒm
    /// </summary>
    /// <param name="errorId"></param>
    public void OnFailedJoinSyn(int errorId)
    {
        this.errorId = errorId;
        if(this.errorId == 0) 
        {//�Q���\�l������
            ErrorUI[this.errorId].SetActive(true);
            //conducter.Loaded();
        }
        if(this.errorId == 1)
        {//�p�X���[�h���Ⴄ�ꍇ
            PrivateUI.SetActive(false);
            ErrorUI[this.errorId].SetActive(true);
            //conducter.Loaded();
        }
        if(this.errorId == 2)
        {//������������
            PrivateUI.SetActive(false);
            ErrorUI[this.errorId].SetActive(true);
            //conducter.Loaded();
        }
        if(this.errorId == 3)
        {//���������݂��Ȃ�
            PrivateUI.SetActive(false);
            ErrorUI[this.errorId].SetActive(true);
            //conducter.Loaded();
        }
    }

    public void OnCreatedRoom()
    {
        SceneManager.LoadScene("3_StandbyRoom");
    }

    /// <summary>
    /// ���������ʒm
    /// Aughter:�ؓc�W��
    /// </summary>
    public void OnJoinedUser(JoinedUser joinedUser)
    {
        foreach (var data in RoomModel.Instance.joinedUserList.Values)
        {
            //���������Ƃ��̏���������
            Debug.Log(data.ConnectionId + "���������܂����B");

        }     
    }

    /// <summary>
    /// �ގ��ʒm
    /// </summary>
    public void OnLeavedUser(JoinedUser joinedUser)
    {
            //���������Ƃ��̏���������
            Debug.Log(joinedUser.ConnectionId + "���ގ����܂����B");
    }
    #endregion
}
