using UnityEngine;
using UnityEngine.UI;

public class ResultScoreManager : MonoBehaviour
{
    public float clearTime;      //ステージ突破にかかった時間
    public int successNum;       //タスク成功回数
    public int failureNum;       //罹災回数

    public int cruelNum;          //残酷だった回数
    public int notJudgeNum;       //判断ミス回数
    public int carelesslyNum;     //うっかりした回数
    public int notPlanNum;        //計画性がなかった回数
    public int notCoolNum;        //かっこよくなかった回数

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void ResetData()
    {
        clearTime = 0;
        successNum = 0;
        failureNum = 0;

        cruelNum = 0;
        notJudgeNum = 0;
        carelesslyNum = 0;
        notPlanNum = 0;
        notCoolNum = 0;
    }

    public void GetClearTime()
    {
        string nowClearTime = GameObject.Find("Time").GetComponent<Text>().text;
        if (nowClearTime != null) Debug.Log(nowClearTime);
        clearTime += float.Parse(nowClearTime);　　//タイマー文字列を数値に変換し加算
    }
}
