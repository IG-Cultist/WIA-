using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Animator animator;

    private const string moveSpeed = "MoveSpeed";


    GameObject checkObj;
    CheckableObjManager objManager;
    //アニメ状態
    public enum ANIM_STATE
    {
        IDLE = 0,             //停止状態(アイドル)
        WALK,                 //歩行状態
        RUN,                  //走行状態
        HAVE_IDLE,            //オブジェクト取得状態
        HAVE_RUN,             //取得中走行状態
        FALL,                 //落下状態
        SEARCH_BIG,           //調査状態(大)
        SEARCH_SMALL,         //調査状態(小)
        EMOTE = 50,           //エモート再生状態
        ERROR,                //上記非該当状態
    }

    //アニメステートを初期値に設定
    public ANIM_STATE anim_State = ANIM_STATE.IDLE;

    //コンポーネント付与処理
    public void Awake()
    {
        // GameObject.Find() を実行し、見つかった場合は変数に格納
        checkObj = GameObject.Find("CheckableObjManager");

        if (checkObj == null) return;
        objManager = checkObj.GetComponent<CheckableObjManager>();

        //objManager = GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>();
    }

    void Update()
    {
        Debug.Log(GetAnimId());
    }
    /// <summary>
    /// アニメーションID取得
    /// </summary>
    /// <returns>アニメーションID</returns>
    public int GetAnimId()
    {
        return animator.GetInteger("AnimID");
    }

    public void SetAnim(ANIM_STATE animId,float animSpeed)
    {
        animator.SetInteger("AnimID", (int)animId);
        animator.SetFloat("MoveSpeed", animSpeed);        //アニメーションの速度を[WalkSpeed]パラメータに設定
    }

    //敵アニメーション反映関数
    public void SetEnemyAnim(int id,float speed)
    {
        animator.SetInteger("AnimID", id);
        animator.SetFloat("MoveSpeed", speed);        //アニメーションの速度を[WalkSpeed]パラメータに設定
    }
}
