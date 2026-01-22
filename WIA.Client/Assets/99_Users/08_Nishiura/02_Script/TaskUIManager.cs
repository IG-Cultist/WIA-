/// ------------------------------
/// タスクのUIマネージャー
/// Author:Nishiura Date:25/11/19
/// ------------------------------
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.ProBuilder.MeshOperations;

public class TaskUIManager : MonoBehaviour
{
    // タスク説明文
    [SerializeField] Text taskExplanation;
    // タスク進捗度
    [SerializeField] Text taskCount;
    // タスクアイコン
    [SerializeField] Image taskIcon;
    // 死亡回数
    [SerializeField] Text deathCount;
    // 経過時間テキスト
    [SerializeField] Text timerText;
    // 制限時間テキスト
    [SerializeField] Text limitTimerText;
    // 変動数テキスト
    [SerializeField] Text fluctTimeText;
    //左クリック画像
    [SerializeField] GameObject leftCrickIcon;
    //右クリック画像
    [SerializeField] GameObject rightCrickIcon;
    //クロスヘアー
    [SerializeField] GameObject Crosshair;
    // 経過時間カウント変数
    float count = 0;
    // 制限時間カウント変数
    public float limitCount = 180;
    // 現在のシーン名
    string nowSceneName;
    // 終了判定
    public bool isFinish = false;

    //通信用
    string position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        leftCrickIcon.SetActive(false);
        nowSceneName = SceneManager.GetActiveScene().name;
        Texture2D texture;

        deathCount.text = ": 0/3";

        if (RoomModel.Instance)
        {
            await RoomModel.Instance.GetPositionAsync();
            position = RoomModel.Instance.posision;
        }

            // 現在のシーンに応じてテクスチャ、文言を変更
            switch (SceneManager.GetActiveScene().name)
            {
                case "Stage_1":
                    // リソースからアイコンを取得
                    texture = Resources.Load("Icons/Icon_Box") as Texture2D;
                    taskIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    // タスクカウントを設定
                    taskCount.text = ": 0/5";
                    // 説明文を変更
                    if(position =="Worker")taskExplanation.text = "任務:指定位置まで箱を運搬";
                    else taskExplanation.text = "任務:コンテナで労働者を粉砕";
                break;
                case "Stage_2":
                    // リソースからアイコンを取得
                    texture = Resources.Load("Icons/Icon_Key") as Texture2D;
                    taskIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    // タスクカウントを設定
                    taskCount.text = ": 0/1";
                    // 説明文を変更
                    if (position == "Worker") taskExplanation.text = "任務:鍵を見つけ、アパートへ帰宅";
                    else taskExplanation.text = "任務:花瓶を下層の人にぶつける";
                break;
                case "Stage_3":
                    // リソースからアイコンを取得
                    texture = Resources.Load("Icons/Icon_Coffee") as Texture2D;
                    taskIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    // タスクカウントを設定
                    taskCount.text = ": 0/5";
                    // 説明文を変更
                    if (position == "Worker") taskExplanation.text = "任務:コーヒーを配達";
                    else taskExplanation.text = "任務:コーヒー配達を妨害";

                //ホストがタイムを同期
                RoomModel.Instance.OnTimeSyn += OnTimeSyn;
                    if (RoomModel.Instance.IsMaster == true) InvokeRepeating("TimeAsync", 0.01f, 0.01f);

                break;
                default:
                    // リソースからアイコンを取得
                    texture = Resources.Load("Icons/Icon_Grave") as Texture2D;
                    taskIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    // タスクカウントを設定
                    taskCount.text = "0/0";
                    // 説明文を変更
                    taskExplanation.text = "Task:missing!";
                    break;
            }
    }

    void Update()
    {
        count += Time.deltaTime;    // カウントアップ
        timerText.text = count.ToString("n2");  // 小数第二位真で表示

        // ステージ3の場合かつステージを終了していない場合
        if (nowSceneName == "Stage_3" && !isFinish)
        {
            if (limitCount <= 0) limitCount = 0; // カウントが0になっている場合、0で固定

            if (RoomModel.Instance.IsMaster == true)
            {
                limitCount -= Time.deltaTime;  //カウントダウン
            }

            limitTimerText.text = limitCount.ToString("n2");    // 小数第二位真で表示
        }
    }


    private void OnDisable()
    {
        if(RoomModel.Instance)
        RoomModel.Instance.OnTimeSyn -= OnTimeSyn;
    }

    /// <summary>
    /// 時間変動処理
    /// </summary>
    /// <param name="num"></param>
    public void FluctNowTime(int num)
    {
        // 受け取った数値に応じて表示を変更
        if (num > 0) fluctTimeText.text = "+" + num;
        else fluctTimeText.text =num.ToString();

        limitCount += num; // 現在の制限時間に受け取った値を加算/減算

        // 表示させた後、非表示にする
        var sequence = DOTween.Sequence(); 
        sequence.Append(fluctTimeText.GetComponent<Text>().DOFade(1f, 0.3f))
                .Append(fluctTimeText.GetComponent<Text>().DOFade(1f, 0.8f))
                .Append(fluctTimeText.GetComponent<Text>().DOFade(0f, 0.3f));
    }

    /// <summary>
    /// クロスヘアーを表示する処理
    /// </summary>
    public void ShowCrossHair()
    {
        Crosshair.SetActive(true);
        leftCrickIcon.SetActive(false);
        rightCrickIcon.SetActive(false);
    }

    /// <summary>
    /// 左クリックアイコンを表示する処理
    /// </summary>
    public void ShowLeftCrickIcon()
    {
        Crosshair.SetActive(false);
        leftCrickIcon.SetActive(true);
        rightCrickIcon.SetActive(false);
    }

    /// <summary>
    /// 右クリックアイコンを表示する処理
    /// </summary>
    public void ShowRightCrickIcon()
    {
        Crosshair.SetActive(false);
        leftCrickIcon.SetActive(false);
        rightCrickIcon.SetActive(true);
    }

    /// <summary>
    /// タイム同期
    /// </summary>
    public async void TimeAsync()
    {
        await RoomModel.Instance.TimeAsync(limitCount);
    }

    /// <summary>
    /// タイム通知
    /// </summary>
    /// <param name="time"></param>
    void OnTimeSyn(float time)
    {
        limitCount = time;
    }
}
