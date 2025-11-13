using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(255, 255, 255, 255);

    [SerializeField] PieChart pie;      //円グラフ
    [SerializeField] RadarChart radar;      //五角形グラフ

    [SerializeField] public int successNum = 4;
    [SerializeField] public int failureNum = 1;

    [Header("レーダーグラフ変数(1.0fが上限値)")]
    [SerializeField] public float cruelNum = 0.2f;
    [SerializeField] public float judgeNum = 0.2f;
    [SerializeField] public float carelesslyNum = 0.2f;
    [SerializeField] public float planNum = 0.2f;
    [SerializeField] public float coolNum = 0.2f;


    void Start()
    {
        //------{成功数,失敗数}-----//
        float[] pieList = { successNum, failureNum };        //ここで労災成功数・失敗数を代入
        pie.SetPieChartAnimation(pieList);

        


        /*
        int playerId = MatchingManager.UserID + 1;
        if (GameManager.Result != null)
        {
            Debug.Log("リザルトを取得できます");
        }
        else
        {
            Debug.LogError("リザルトを取得できません！！");
        }

        Debug.Log("プレイヤー"+playerId+"のリザルト："+GameManager.Result.TotalScore.ToString());*/

    }

    // Update is called once per frame
    void Update()
    {
        float[] radarList = { cruelNum, judgeNum, carelesslyNum, planNum, coolNum };        //ここで労災成功数・失敗数を代入
        radar.SetRadarChart(radarList);
    }

    public void StartMenu()
    {
        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("02_MenuScene", endColor, 1.0f);
    }
}
