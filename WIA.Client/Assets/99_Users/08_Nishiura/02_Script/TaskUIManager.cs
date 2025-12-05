/// ------------------------------
/// タスクのUIマネージャー
/// Author:Nishiura Date:25/11/19
/// ------------------------------
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    // 経過時間カウント変数
    float count = 0;
    // 制限時間カウント変数
    public float limitCount = 60;
    // 現在のシーン名
    string nowSceneName;
    // 終了判定
    public bool isFinish = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nowSceneName = SceneManager.GetActiveScene().name;
        Texture2D texture;

        deathCount.text = ": 0/3";
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
                taskExplanation.text = "任務:指定位置まで箱を運搬";
                break;
            case "Stage_2":
                // リソースからアイコンを取得
                texture = Resources.Load("Icons/Icon_Key") as Texture2D;
                taskIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                // タスクカウントを設定
                taskCount.text = ": 0/1";
                // 説明文を変更
                taskExplanation.text = "任務:鍵を見つけ、アパートへ帰宅";
                break;
            case "Stage_3":
                // リソースからアイコンを取得
                texture = Resources.Load("Icons/Icon_Coffee") as Texture2D;
                taskIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                // タスクカウントを設定
                taskCount.text = ": 0/5";
                // 説明文を変更
                taskExplanation.text = "任務:コーヒーを配達";
                break;
            case "Stage_4":

                break;
            case "Stage_5":

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
        count += Time.deltaTime;
        timerText.text = count.ToString("n2");

        if(nowSceneName == "Stage_3" && !isFinish)
        {
            if(limitCount <= 0) limitCount = 0;
            else limitCount -= Time.deltaTime;
            limitTimerText.text = limitCount.ToString("n2");
        }
    }
}
