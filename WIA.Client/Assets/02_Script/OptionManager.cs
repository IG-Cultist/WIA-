using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using UnityEngine.Rendering;


public class OptionManager : MonoBehaviour
{
    [SerializeField] GameObject window;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    //各音量変数
    private float SEVolume;
    private float BGMVolume;

    bool isSetting;


    FirstPersonLook pov;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        window.SetActive(false);
        isSetting = false;

        //設定をローカルで保存するならここで取得先を変える
        BGMSlider.value = 100.0f;
        SESlider.value = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(isSetting)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;    //カーソルを表示
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }*/
        

        //スライダーの値を音量に反映
        BGMVolume = BGMSlider.value * 0.01f;
        SEVolume = SESlider.value * 0.01f;

        //BGM全体のボリュームを変更
        BGMManager.Instance.ChangeBaseVolume(BGMSlider.value);
        //SE全体のボリュームを変更
        SEManager.Instance.ChangeBaseVolume(SESlider.value);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SEManager.Instance.Play(
                audioPath: SEPath.PUSH_BUTTON,   //再生したいオーディオのパス
                volumeRate: 1,                 //音量の倍率
                delay: 0,                      //再生されるまでの遅延時間
                pitch: 1,                      //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                 //再生終了後の処理
            );

            if (window.gameObject.activeSelf == false) window.SetActive(true);

            else window.SetActive(false);

        }

        // "TargetObject"という名前のオブジェクトを探す
        GameObject targetObject = GameObject.Find("TargetObject");

        //pov = GameObject.Find("Main").gameObject.transform.GetChild(1).gameObject.transform.GetComponent<FirstPersonLook>();   //調査プレイヤーの現在地取得
    }

    /// <summary>
    /// 設定開始関数
    /// </summary>
    public void OpenOption()
    {
        window.SetActive(true);

        if (pov == null) return;

        pov.enabled = false;
    }

    /// <summary>
    /// 設定終了関数
    /// </summary>
    public void CloseOption()
    {
        window.SetActive(false);

        if (pov == null) return;

        pov.enabled = true;
    }
}
