using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputSettings;


public class OptionManager : MonoBehaviour
{
    [SerializeField] GameObject window;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    [Header("調査プレイヤー用")]
    FirstPersonMovement playerMove;    //座標固定用
    FirstPersonLook playerCamera;      //視点固定用
                                       //VR
    private XRControllerButtonEvents xrControllerButtonEvents;

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
        if(!isSetting)
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

        //pov = GameObject.Find(player.name).gameObject.transform.GetChild(1).gameObject.transform.GetComponent<FirstPersonLook>();   //調査プレイヤーの現在地取得
    }

    /// <summary>
    /// 設定開始関数
    /// </summary>
    public void OpenOption()
    {
        window.SetActive(true);

        if (SceneManager.GetActiveScene().name == "Stage_1" || SceneManager.GetActiveScene().name == "Stage_2" || SceneManager.GetActiveScene().name == "Stage_3")
        {

            if (RoomModel.Instance)
            {
                //メインキャラクターのカメラを取る
                playerCamera = GameObject.Find(OnlineGameManager.Player.name).transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();
                //プレイヤー移動処理スクリプト取得
                playerMove = GameObject.Find(OnlineGameManager.Player.name).GetComponent<FirstPersonMovement>();
                //VRのスクリプト取得
                xrControllerButtonEvents = GameObject.Find(OnlineGameManager.Player.name).GetComponent<XRControllerButtonEvents>();
            }
            else
            {
                //メインキャラクターのカメラを取る
                playerCamera = GameObject.Find("Main").transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();
                //プレイヤー移動処理スクリプト取得
                playerMove = GameObject.Find("Main").GetComponent<FirstPersonMovement>();
                //VRのスクリプト取得
                xrControllerButtonEvents = GameObject.Find("Main").GetComponent<XRControllerButtonEvents>();
            }
            

            playerCamera.enabled = false;   //カメラアングル固定化
            playerMove.enabled = false;     //プレイヤー座標固定化
        }

        isSetting = true;

    }

    /// <summary>
    /// 設定終了関数
    /// </summary>
    public void CloseOption()
    {
        window.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Stage_1" || SceneManager.GetActiveScene().name == "Stage_2" || SceneManager.GetActiveScene().name == "Stage_3")
        {

            if (RoomModel.Instance)
            {
                //メインキャラクターのカメラを取る
                playerCamera = GameObject.Find(OnlineGameManager.Player.name).transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();
                //プレイヤー移動処理スクリプト取得
                playerMove = GameObject.Find(OnlineGameManager.Player.name).GetComponent<FirstPersonMovement>();
                //VRのスクリプト取得
                xrControllerButtonEvents = GameObject.Find(OnlineGameManager.Player.name).GetComponent<XRControllerButtonEvents>();
            }
            else
            {
                //メインキャラクターのカメラを取る
                playerCamera = GameObject.Find("Main").transform.Find("First Person Camera").gameObject.GetComponent<FirstPersonLook>();
                //プレイヤー移動処理スクリプト取得
                playerMove = GameObject.Find("Main").GetComponent<FirstPersonMovement>();
                //VRのスクリプト取得
                xrControllerButtonEvents = GameObject.Find("Main").GetComponent<XRControllerButtonEvents>();
            }
            

            playerCamera.enabled = true;   //カメラアングル固定化
            playerMove.enabled = true;     //プレイヤー座標固定化
        }

        isSetting = false;
    }
}
