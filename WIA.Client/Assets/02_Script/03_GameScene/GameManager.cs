using Shared.Interfaces.StreamingHubs;
using UnityEngine;
using WIA.Shared.Interfaces.StreamingHubs;

public class GameManager : MonoBehaviour
{
    [Header("�J�ڃt�F�[�h�J���[")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);

    private static ResultData resultData;

    public static  ResultData Result
    {  get { return resultData; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (RoomModel.Instance)
        {
            RoomModel.Instance.OnGameEndSyn += this.OnGameEndSyn;
            RoomModel.Instance.OnLeavedUser += this.OnLeavedUser;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        if (RoomModel.Instance) 
        {
            RoomModel.Instance.OnGameEndSyn -= this.OnGameEndSyn;
            RoomModel.Instance.OnLeavedUser -= this.OnLeavedUser;
        }
    }

    public async void GameEnd()
    {
        await RoomModel.Instance.GameEndAsync();
    }

    public�@async void StartResult()
    {
        if (RoomModel.Instance) 
        {
            await RoomModel.Instance.LeavedAsync();
        }
        // �V�[���J��
        Initiate.DoneFading();
        Initiate.Fade("04_ResultScene", endColor, 1.0f);
    }

    public void OnGameEndSyn(ResultData resultData)
    {
        GameManager.resultData = resultData;
        StartResult();
    }

    /// <summary>
    /// �ގ��ʒm
    /// </summary>
    public void OnLeavedUser(JoinedUser joinedUser)
    {
        //�ގ������Ƃ��̏���������
        Debug.Log(joinedUser.ConnectionId + "���ގ����܂����B");
    }

}
