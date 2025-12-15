using UnityEngine;
using TMPro;
using System.Collections;
using WIA.Shared.Interfaces.Model.Entity;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(255, 255, 255, 255);

    [SerializeField] GameObject pcGuideText;
    [SerializeField] GameObject vrGuideText;

    [SerializeField] GameObject textPrefab;
    [SerializeField] Image pcViewImage;
    [SerializeField] Image vrViewImage;

    [Header("プレビュー画像")]
    [SerializeField] Sprite view1;
    [SerializeField] Sprite view2;
    [SerializeField] Sprite view3;

    /////////////////////////
    ///説明テキスト  

    string GameText = "二名の体験者がそれぞれ罹災者側・被災者側に分かれて、実際に発生した労働災害の事例を体験することが出来ます。";
    string DictionaryText = "本プロトコルで体験した労働災害について詳しく振り返ることが出来ます。再発防止に努めましょう。";
    string ProfileText = "登録されているユーザー情報を確認・変更することが出来ます。名前変更はこちらから。";
    
    /////////////////////////

    private enum NEXTSCENE_STATE
    {
        MENU = 0,                  //メニュー(初期)
        GAME,                      //ゲーム
        DICTIONARY,                //名鑑
        PROFILE,                   //プロフィール
        ERROR,                     //エラー
    }

    NEXTSCENE_STATE scene_State = NEXTSCENE_STATE.MENU;    //シーンステート(遷移先を管理)


    public void SelectGame()
    {
        scene_State = NEXTSCENE_STATE.GAME;
        pcViewImage.sprite = view1;
        vrViewImage.sprite = view1;

        ChangeText(GameText);
    }

    public void SelectDictionary()
    {
        scene_State = NEXTSCENE_STATE.DICTIONARY;
        pcViewImage.sprite = view2;
        vrViewImage.sprite = view2;

        ChangeText(DictionaryText);
    }

    public void SelectProfile()
    {
        scene_State = NEXTSCENE_STATE.PROFILE;
        pcViewImage.sprite = view3;
        vrViewImage.sprite = view3;

        ChangeText(ProfileText);
    }

    public void BackTitle()
    {
        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("01_TitleScene", endColor, 2.0f);
    }

    /// <summary>
    /// 説明テキスト変更関数
    /// </summary>
    /// <param name="text">代入テキスト</param>
    private void ChangeText(string text)
    {
        Destroy(pcGuideText.transform.GetChild(0).gameObject);   //既存テキストを削除
        Destroy(vrGuideText.transform.GetChild(0).gameObject);   //既存テキストを削除

        GameObject pcTxt = Instantiate(textPrefab, new Vector3(610f, 195f, 0f), Quaternion.identity, pcGuideText.gameObject.transform); //インスタンス生成
        GameObject vrTxt = Instantiate(textPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, vrGuideText.gameObject.transform); //インスタンス生成

        Vector3 pos = vrTxt.transform.position;
        pos.x += -350.0001f;
        pos.y += -909.5871f;
        pos.z -= 2147.96f;

        pcTxt.GetComponent<TextMeshProUGUI>().text = text;
        vrTxt.GetComponent<TextMeshProUGUI>().text = text;

        pcTxt.transform.SetAsFirstSibling();   //先頭に並び替え
        vrTxt.transform.SetAsFirstSibling();   //先頭に並び替え
    }

    /// <summary>
    /// スタート関数
    /// </summary>
    public void StartSceneChange()
    {
        // シーン遷移
        Initiate.DoneFading();

        switch (scene_State)
        {

            case NEXTSCENE_STATE.MENU:  //メニュー(初期)状態

                break;

            case NEXTSCENE_STATE.GAME:  //ゲーム選択状態

                //ロビーシーンに遷移
                //Initiate.Fade("Stage_1", endColor, 1.0f);
                Initiate.Fade("Exp_Worker_1", endColor, 1.0f);

                break;

            case NEXTSCENE_STATE.DICTIONARY:   //名鑑選択状態

                //名鑑シーンに遷移
                Initiate.Fade("04_DictionaryScene", endColor, 1.0f);

                break;

            case NEXTSCENE_STATE.PROFILE:   //登録情報選択状態

                //プロフィールシーンに遷移
                Initiate.Fade("05_ProfileScene", endColor, 1.0f);

                break;


        }

        
    }
}
