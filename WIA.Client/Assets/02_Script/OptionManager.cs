using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using UnityEngine.Rendering;


public class OptionManager : MonoBehaviour
{
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    //各音量変数
    private float SEVolume;
    private float BGMVolume;

    void Start()
    {
        this.gameObject.SetActive(false);

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
    }

    /// <summary>
    /// 設定終了関数
    /// </summary>
    public void CloseOption()
    {
        this.gameObject.SetActive(false);
    }
}
