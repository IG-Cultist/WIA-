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

    [SerializeField] GameObject guideText;
    [SerializeField] GameObject textPrefab;

    /////////////////////////
    ///説明テキスト  
    
    string GameText = "実際に発生した労働災害の事例を再現したミニゲームを体験できます。加害者・被害者に分かれてそれぞれのタスクをこなしていきます。";
    string DictionaryText = "本プロトコルで体験した労働災害について詳しく振り返ることが出来ます。再発防止に努めましょう。";
    string ProfileText = "登録されているユーザー情報を確認・変更することが出来ます。";
    
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

        ChangeText(GameText);
    }

    public void SelectDictionary()
    {
        scene_State = NEXTSCENE_STATE.DICTIONARY;

        ChangeText(DictionaryText);
    }

    public void SelectProfile()
    {
        scene_State = NEXTSCENE_STATE.PROFILE;

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
        Destroy(guideText.transform.GetChild(0).gameObject);   //既存テキストを削除

        GameObject txt = Instantiate(textPrefab, new Vector3(610f, 195f, 0f), Quaternion.identity, guideText.gameObject.transform); //インスタンス生成
        
        txt.GetComponent<TextMeshProUGUI>().text = text;

        txt.transform.SetAsFirstSibling();   //先頭に並び替え

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
                Initiate.Fade("03_GameScene", endColor, 1.0f);

                break;

            case NEXTSCENE_STATE.DICTIONARY:   //名鑑選択状態

                //名鑑シーンに遷移

                break;

            case NEXTSCENE_STATE.PROFILE:   //登録情報選択状態

                //プロフィールシーンに遷移

                break;


        }

        
    }
}
