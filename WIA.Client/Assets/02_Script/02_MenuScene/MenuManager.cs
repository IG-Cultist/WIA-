using UnityEngine;
using TMPro;


public class MenuManager : MonoBehaviour
{

    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);

    [SerializeField] GameObject guideText;
    [SerializeField] GameObject textPrefab;

    private enum NEXTSCENE_STATE
    {
        MENU = 0,                  //メニュー(初期)
        GAME,                      //生存中
        DICTIONARY,                //死亡中
        PROFILE,                   //プロフィール
        ERROR,                     //エラー(切断)
    }

    NEXTSCENE_STATE scene_State = NEXTSCENE_STATE.MENU;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectPlayGame()
    {
        scene_State = NEXTSCENE_STATE.GAME;

    }

    public void StartGame()
    {
      
        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("03_GameScene", endColor, 1.0f);
    }
}
