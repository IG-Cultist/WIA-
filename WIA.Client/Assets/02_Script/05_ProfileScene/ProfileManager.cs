using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(255, 255, 255, 255);

    [SerializeField] Text currentName;     //変更前ネーム
    [SerializeField] Text changedName;     //変更後ネーム

    void Start()
    {
        //DBから取得して代入
        currentName.text = "いまのなまえ";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// もどる関数
    /// </summary>
    public void BackMenu()
    {
        // シーン遷移
        Initiate.DoneFading();
        //VRかどうかでシーン選別
        if (UnityEngine.XR.XRSettings.isDeviceActive) Initiate.Fade("VR_02_MenuScene", endColor, 2.0f);
        else Initiate.Fade("02_MenuScene", endColor, 2.0f);
    }

    /// <summary>
    /// 登録情報更新関数
    /// </summary>
    public void UpdateInfo()
    {

    }
}
