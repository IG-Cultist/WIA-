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
    
    void Start()
    {
        //------{成功数,失敗数}-----//
        float[] list = { successNum, failureNum };        //ここで労災成功数・失敗数を代入
        pie.SetPieChartAnimation(list);

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

    }

    public void StartMenu()
    {
        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("02_MenuScene", endColor, 1.0f);
    }
}
