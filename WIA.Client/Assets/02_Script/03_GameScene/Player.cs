using NIGHTRAVEL.Shared.Interfaces.StreamingHubs;
using UnityEngine;

/// <summary>
/// プレイヤー処理クラス
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] GameObject hitPoint;
    [SerializeField] Animator animator;

    //プレイヤー状態
    private enum PLAYER_STATE
    {
        STOP = 0,             //停止中(生成前)
        ALIVE,                //生存中
        DEATH,                //死亡中
        EMOTE,                //エモート中
        ERROR,                //エラー(切断)
    }

    [SerializeField] PLAYER_STATE player_State;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (player_State)
        {
            //生成前の状態
            case PLAYER_STATE.STOP:

            break;

            //生存中の場合
            case PLAYER_STATE.ALIVE:

                hitPoint.SetActive(true);
                animator.enabled = true;

                //この下に生存中の処理を書く




                break;

            //死亡中の場合
            case PLAYER_STATE.DEATH:
                Death();
            break;

            //エモート再生中の場合
            case PLAYER_STATE.EMOTE:
                Emote();
            break;

            //エラーの場合
            case PLAYER_STATE.ERROR:

            break;

        }

    }

    /// <summary>
    /// 死亡関数
    /// </summary>
    private void Death()
    {
        hitPoint.SetActive(true);
        animator.enabled = false;
    }

    /// <summary>
    /// エモート関数
    /// </summary>
    private void Emote(/*ここにアニメーションの型を宣言*/)
    {
        //当たり判定を一時的に削除
        hitPoint.SetActive(false);
        
        //引数で取得したアニメーションIDでアニメーション再生(組み込むときに書き換えてね)
    }
}
