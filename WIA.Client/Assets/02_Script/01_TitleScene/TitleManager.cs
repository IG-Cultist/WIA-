using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KanKikuchi.AudioManager;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text guideText;
    [SerializeField] GameObject optionWindow;
    [SerializeField] GameObject licenseWindow;

    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);

    private void Awake()
    {
    }

    void Start()
    {
        optionWindow.SetActive(false);
        licenseWindow.SetActive(false);

        DontDestroyOnLoad(optionWindow);

        BGMManager.Instance.Play(
            audioPath: BGMPath.TITLE_MUKISHITU, //再生したいオーディオのパス
            volumeRate: 0.5f,                   //音量の倍率
            delay: 0,                           //再生されるまでの遅延時間
            pitch: 1,                           //ピッチ
            isLoop: true,                       //ループ再生するか
            allowsDuplicate: false              //他のBGMと重複して再生させるか
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartMenu()
    {
        SEManager.Instance.Play(
            audioPath: SEPath.GAME_START, //再生したいオーディオのパス
             volumeRate: 1,                 //音量の倍率
             delay: 0,                      //再生されるまでの遅延時間
             pitch: 1,                      //ピッチ
             isLoop: false,                 //ループ再生するか
             callback: null                 //再生終了後の処理
        );

        guideText.gameObject.SetActive(false);

        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("02_MenuScene", endColor, 2.0f);
    }

    public void OpenOption()
    {
        optionWindow.SetActive(true);
    }

    public void OpenLicense()
    {
        licenseWindow.SetActive(true);
    }

    public void CloseLicense()
    {
        licenseWindow.SetActive(false);
    }
}
