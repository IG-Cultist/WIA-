using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using UnityEngine.Rendering;


public class OptionManager : MonoBehaviour
{
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    //�e���ʕϐ�
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

        //�X���C�_�[�̒l�����ʂɔ��f
        BGMVolume = BGMSlider.value * 0.01f;
        SEVolume = SESlider.value * 0.01f;

        //BGM�S�̂̃{�����[����ύX
        BGMManager.Instance.ChangeBaseVolume(BGMSlider.value);
        //SE�S�̂̃{�����[����ύX
        SEManager.Instance.ChangeBaseVolume(SESlider.value);
    }

    /// <summary>
    /// �ݒ�I���֐�
    /// </summary>
    public void CloseOption()
    {
        this.gameObject.SetActive(false);
    }
}
