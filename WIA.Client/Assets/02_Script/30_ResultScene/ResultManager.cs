using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;

public class ResultManager : MonoBehaviour
{
    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(255, 255, 255, 255);

    [SerializeField] GameObject evaluation; //ランク表示
    [SerializeField] Text rankText;
    [SerializeField] PieChart pie;          //円グラフ
    [SerializeField] RadarChart radar;      //五角形グラフ

    [Header("AI評価用")]
    [SerializeField] Text messageText;      //AI評価表示用テキスト
    [SerializeField] public int successNum;    //労災回避成功数
    [SerializeField] public int failureNum;    //労災回避失敗数

    //デプロイキー
    private string deployId = "AKfycbyxWrtaB3lg7yRcLLVDPuPr6fRYAkY4rruxPHzuIbtOucyGJpUeCB3lmEJbNaM84gmo";
    //前提プロンプト(ここでAIの性格・出力形式・文字数を設定)
    private string questionText = "あなたは労働災害啓発協会というゲームの架空の監督官であり、労働災害防止に関する取り組みを行っています。" +
        "あなたには次の数値を基準に対象作業員を評価していただきます。評価項目は、労災回避成功数,労災回避失敗数,残酷だった回数,正常に判断出来なかった回数,うっかりしてしまった回数,計画性がなかった回数,ださかった回数です。" +
        "労災回避成功数は高ければ高いほど優秀、他の項目は高ければ高いほど無能と判断されます。プレイヤーのゲーム内での動きや行動を解析して、労働災害に対する意識や良かったところを総評シミュレートしてください。" +
        "評価の後、できるのであれば一言メッセージを添えてください。" +
        "尚、評価をする際は、250文字以内で提示してください。これは絶対厳守です。要所要所でブラックジョークも挟めると良いです。また、改行コード('\n')は入れないでください。)" +
        "以下が、対象の数値です。";
    

    [Header("レーダーグラフ変数(回数加算)")]
    [SerializeField] public int CruelNum;       //残酷だった回数
    [SerializeField] public int notJudgeNum;       //判断ミス回数
    [SerializeField] public int CarelesslyNum;  //うっかりした回数
    [SerializeField] public int notPlanNum;        //計画性がなかった回数
    [SerializeField] public int notCoolNum;        //かっこよくなかった回数

    [Header("減点倍率")]
    [SerializeField] public float radarMagnification;

    [SerializeField]Animator animator;

    void Start()
    {
        //------{成功数,失敗数}-----//
        float[] pieList = { successNum, failureNum };        //ここで労災成功数・失敗数を代入
        pie.SetPieChartAnimation(pieList);

        //ここで最終評価ランクを設定
        //rankText.text = "";

        StartCoroutine(CallGoogleAppsScript());
        evaluation.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeIntType();
    }


    /// <summary>
    /// グラフ表示用数値変換関数
    /// </summary>
    private void ChangeIntType()
    {
        int loop = 0;
        int[] changeList = { CruelNum, notJudgeNum, CarelesslyNum, notPlanNum, notCoolNum };        //ここで労災成功数・失敗数を代入
        float[] radarList = new float[changeList.Length];

        //グラフ初期値設定
        foreach (int v in radarList)
        {
            radarList[loop] = 1.0f;

            loop++;
        }

        loop = 0;

        //各グラフの項目内容に応じて減算
        foreach (int num in changeList)
        {
            radarList[loop] -= num * radarMagnification;   //項目回数 * 減算倍率

            //グラフ表示上限を設定
            if (radarList[loop] >= 1) radarList[loop] = 1f;
            else if (radarList[loop] < 0) radarList[loop] = 0f;

            loop++;
        }

        radar.SetRadarChart(radarList);     //変換した数値リストを反映
    }


    public void StartMenu()
    {
        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("02_MenuScene", endColor, 1.0f);
    }



    // 入力欄
    [SerializeField] InputField inputField;
    public void SendQuestion()
    {
        StartCoroutine(CallGoogleAppsScript());
    }

    public void Send60Question()
    {
        for (int i = 0; i < 60; i++)
        {
            StartCoroutine(CallGoogleAppsScript());
        }
    }


    IEnumerator CallGoogleAppsScript()
    {
        questionText += string.Format("[労災回避成功数:{0}回],[労災回避失敗数:{1}回],[残酷だった回数:{2}回],[正常に判断出来なかった回数:{3}回],[うっかりしてしまった回数:{4}回],[計画性がなかった回数:{5}回],[ださかった回数:{6}回]"
            , successNum,failureNum,CruelNum,notJudgeNum,CarelesslyNum,notPlanNum,notCoolNum);

        string url = $"https://script.google.com/macros/s/{deployId}/exec?question=" + UnityWebRequest.EscapeURL(questionText);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            messageText.text = "監督官との通信中にエラーが発生しました。";
            Debug.LogError("エラー: " + request.error);
        }
        else
        {
            string resultText = request.downloadHandler.text;


            //最後尾の2文字を削除する
            resultText = resultText.Remove(0, 11);
            resultText = resultText.Remove(resultText.Length - 2, 2);

            string viewText = resultText.Replace("\n", "");


            messageText.text = viewText;
            Debug.Log("レスポンス: " + request.downloadHandler.text);
        }
        animator.SetBool("isChange", true);
    }

   

}
