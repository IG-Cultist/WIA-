using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KanKikuchi.AudioManager;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text guideText;
    [SerializeField] GameObject optionWindow;
    [SerializeField] GameObject licenseWindow;

    [Header("�J�ڃt�F�[�h�J���[")]
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
            audioPath: BGMPath.TITLE_MUKISHITU, //�Đ��������I�[�f�B�I�̃p�X
            volumeRate: 0.5f,                   //���ʂ̔{��
            delay: 0,                           //�Đ������܂ł̒x������
            pitch: 1,                           //�s�b�`
            isLoop: true,                       //���[�v�Đ����邩
            allowsDuplicate: false              //����BGM�Əd�����čĐ������邩
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartMenu()
    {
        SEManager.Instance.Play(
            audioPath: SEPath.GAME_START, //�Đ��������I�[�f�B�I�̃p�X
             volumeRate: 1,                 //���ʂ̔{��
             delay: 0,                      //�Đ������܂ł̒x������
             pitch: 1,                      //�s�b�`
             isLoop: false,                 //���[�v�Đ����邩
             callback: null                 //�Đ��I����̏���
        );

        guideText.gameObject.SetActive(false);

        // �V�[���J��
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
