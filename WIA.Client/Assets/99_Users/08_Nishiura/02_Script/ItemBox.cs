/// ------------------------------
/// アイテムボックススクリプト
/// Author:Nishiura Date:25/12/04
/// ------------------------------
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    // クールダウン用スライダー
    [SerializeField] GameObject coolDownSlider;
 
    // 生成オブジェクト
    GameObject createdObj;
    // クールダウン中判定
    bool isCooldown = false;

    /// <summary>
    /// 通信用
    /// </summary>
    
    OnlineGameManager gameManager;

    void Start()
    {
        // クールダウンUIを非表示
        coolDownSlider.SetActive(false);

        if(RoomModel.Instance)
        //ゲームマネージャー取得
        gameManager = GameObject.Find("OnlineGameManager").GetComponent<OnlineGameManager>();
    }
    /// <summary>
    /// アイテム獲得処理
    /// </summary>
    public async void GetItem()
    {
        // 生成したオブジェクトがある間、処理しない
        if (createdObj != null || isCooldown) return;


        // 乱数を生成
        int rnd = Random.Range(0, 2);
        GameObject prefab;

        switch (rnd)    // 乱数に応じて生成するオブジェクトを分ける
        {
            case 0:
                prefab = Resources.Load("Items/Injector") as GameObject;
                break;

            case 1:
                prefab = Resources.Load("Items/Cup") as GameObject;
                break;

            default:
                prefab = null;
                break;
        }

        //通信時のみ
        if (RoomModel.Instance)
        {
            Debug.Log("rndは" + rnd);
            await RoomModel.Instance.SpawnItemAsync(rnd, new Vector3(this.gameObject.transform.position.x, -0.36f, this.gameObject.transform.position.y));
        }
        else
        {
            // 読み込まれたプレハブを生成する
            createdObj = Instantiate(prefab, new Vector3(this.gameObject.transform.position.x, -0.36f, this.gameObject.transform.position.y), Quaternion.identity);
            createdObj.name = prefab.name;
        }

        isCooldown = true;  // クールダウン中とする
        coolDownSlider.SetActive(true); //クールダウンUIを表示
        coolDownSlider.GetComponent<Slider>().value = 1f;
        coolDownSlider.GetComponent<Slider>().DOValue(0, 10f).SetEase(Ease.Linear); // 10秒かけて値を減らす
        // 10秒後、クールダウン終了
        Invoke("ResetCooldown", 10f);
    }

    /// <summary>
    /// クールダウン終了処理
    /// </summary>
    void ResetCooldown()
    {
        isCooldown = false;
        createdObj = null;
        coolDownSlider.SetActive(false);
    }
}