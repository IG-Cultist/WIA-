using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int playerId = MatchingManager.UserID + 1;
        if (GameManager.Result != null)
        {
            Debug.Log("リザルトを取得できます");
        }
        else
        {
            Debug.LogError("リザルトを取得できません！！");
        }

            Debug.Log("プレイヤー"+playerId+"のリザルト："+GameManager.Result.TotalScore.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTitle()
    {
        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("01_TitleScene", endColor, 1.0f);
    }
}
