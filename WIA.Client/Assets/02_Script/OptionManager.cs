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

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        window.SetActive(false);

        //設定をローカルで保存するならここで取得先を変える
        BGMSlider.value = 100.0f;
        SESlider.value = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {

        //スライダーの値を音量に反映
        BGMVolume = BGMSlider.value * 0.01f;
        SEVolume = SESlider.value * 0.01f;

        //BGM全体のボリュームを変更
        BGMManager.Instance.ChangeBaseVolume(BGMSlider.value);
        //SE全体のボリュームを変更
        SEManager.Instance.ChangeBaseVolume(SESlider.value);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // エスケープキーが押された時の処理をここに記述する
            Debug.Log("Escapeキーが押されました！");
            // 例：ゲームを一時停止する、メニュー画面を開くなど
        }
    }

    /// <summary>
    /// 設定開始関数
    /// </summary>
    public void OpenOption()
    {
        window.SetActive(true);
    }

    /// <summary>
    /// 設定終了関数
    /// </summary>
    public void CloseOption()
    {
        window.SetActive(false);
    }
}
