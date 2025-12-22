using KanKikuchi.AudioManager;
using WIA.Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerAnimation;
using System;

/// <summary>
/// プレイヤースクリプト
/// </summary>
public class Player : MonoBehaviour
{
    [Header("プレイヤー(当たり判定・アニメーション)系")]
    [SerializeField] GameObject child;
    [SerializeField] GameObject hitPoint;    //Player(lagdoll)当たり判定
    [SerializeField] Animator animator;      //アニメーター(速度調整用)
    [SerializeField] public PlayerAnimation plaAnimation;    //アニメーションスクリプト

    [Header("アクション座標系")]
    [SerializeField] Transform warpPoint; //リスポーン地点
    [SerializeField] ObjectGrabber grabber;

    //カメラ切り替え用変数
    CameraManager cameraManager;

    //フェード用変数
    FadeImage fadeImageScript;

    Rigidbody rigidbody;      //慣性取得用

    private float moveSpeed;
    public float subMoveSpeed;

    [Header("フラグ系")]
    public bool isHave = false;     //荷物所持判定
    public bool isFall = false;     //落下状態判定
    public bool isDead = false;     //死亡判定
    public bool isRespawn = true;   //リスポーン判定
    public bool isSearch = false;
    public bool isLow = false;      //調査オブジェクト高さ判定
    public bool isDebug;
    public bool isTrip = false;     //転倒判定
    public bool isSliped = false;   //転倒後判定
    public bool isMain = false;    //操作本人か
    //private static bool isMain = false;//操作本人か(初期は本人ではないと判断)
    //public static bool IsMain
    //{
    //    get { return isMain; }
    //}


    //プレイヤーステート
    public enum PLAYER_STATE
    {
        STOP = 0,             //停止中(生成前)
        ALIVE,                //生存状態
        DEATH,                //死亡状態
        EMOTE,                //エモート状態
        STRICKER,             //罹災者状態
        ERROR,                //上記非該当状態
    }

    [Header("プレイヤー状態系")]
    [SerializeField] public PLAYER_STATE player_State;  //プレイヤー状態

    List<Transform> allChildren;
    GameObject tripPanel;

    public int deathCnt; //死亡回数

    private ResultScoreManager resultScoreManager;

    private void Awake()
    {
        if (RoomModel.Instance)
        {
            if (this.name == "Player_1(Clone)")
            {
                //操作キャラクター分のUIを表示
                SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
                isMain = true;
            }

            rigidbody = this.GetComponent<Rigidbody>();
        }
        else
        {
            isMain = true;
            //操作キャラクター分のUIを表示
            SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
            rigidbody = this.GetComponent<Rigidbody>();
        }
    }

    void Start()
    {
        if (RoomModel.Instance)
        {
            //ワープ地点設定通信中のみ処理する
            if (RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder == 1)
            {
                warpPoint = GameObject.Find("PlayerSpawnPoint_1").transform;
            }
            else
            {
                warpPoint = GameObject.Find("PlayerSpawnPoint_2").transform;
            }
        }

        if (!isMain) return;

        //resultScoreManager = GameObject.Find("ResultScoreManager").GetComponent<ResultScoreManager>();

        deathCnt = 0; //死亡回数
        //VR時は無視
        if (!UnityEngine.XR.XRSettings.isDeviceActive && grabber != null)
        {
            //ステージ3のみY座標固定
            if (SceneManager.GetActiveScene().name == "Stage_3") this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            else this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }

        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        fadeImageScript = GameObject.Find("FadeImage").GetComponent<FadeImage>();
        tripPanel = GameObject.Find("TripPanel");
        tripPanel.SetActive(false);

        isHave = false;
    }
    // Update is called once per frame
    void Update()
    {
        //デバッグ用
        if (Input.GetKeyDown(KeyCode.K)) player_State = PLAYER_STATE.DEATH;
        if (Input.GetKeyDown(KeyCode.I)) player_State = PLAYER_STATE.ALIVE;

        moveSpeed = rigidbody.linearVelocity.magnitude * 3.0f;   //オブジェクト速度を元にアニメーションの速度を決定
        if (moveSpeed < 0) moveSpeed = 0;
        //else if(moveSpeed >= 7) moveSpeed = 7;      //再生最大速度を設定

        //プレイヤー状態分岐
        switch (player_State)
        {
            //生成前状態
            case PLAYER_STATE.STOP:

                break;

            //生存状態
            case PLAYER_STATE.ALIVE:

                this.gameObject.GetComponent<CapsuleCollider>().enabled = true;

                isDead = false;
                //cameraManager.TurnOnPlayerCam();

                //プレイヤーとプレイヤーオブジェクトの角度を同期
                child.transform.eulerAngles = this.gameObject.gameObject.transform.eulerAngles; //親オブジェクトに角度を統一

                //LagDoll
                ChangeBodyGravity(false);    //LagDollパーツに掛かる重力をfalseに
                hitPoint.SetActive(false);   //当たり判定をfalseに
                animator.enabled = true;

                //以下に生存中の処理を記入
                //----------------------------------------------------

                //リスポーン位置調整処理
                if (!isRespawn)
                {
                    if (player_State == PLAYER_STATE.STRICKER) return;

                    //加速度をリセット
                    Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();

                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    child.transform.position = this.gameObject.transform.position;
                    this.gameObject.transform.position = child.transform.position;

                    isRespawn = true;
                    Debug.Log("リセット完了");
                }

                #region プレイヤーアニメーション
                #region オンライン環境
                if (RoomModel.Instance)
                {//オンライン環境
                    if (OnlineGameManager.Player == this.gameObject)
                    {//操作しているプレイヤー
                        if (isSearch)
                        {//探している
                            if (isLow) plaAnimation.SetAnim(ANIM_STATE.SEARCH_LOW, 1);
                            else plaAnimation.SetAnim(ANIM_STATE.SEARCH_HIGH, 1);
                        }
                        else
                        {//探していない
                            if (moveSpeed > 11) plaAnimation.SetAnim(ANIM_STATE.FALL, 1); //落ちる
                            else
                            {
                                if (moveSpeed <= 1)
                                {//待機
                                    if (!isHave) plaAnimation.SetAnim(ANIM_STATE.IDLE, 1); //持っていない
                                    else plaAnimation.SetAnim(ANIM_STATE.HAVE_IDLE, 1);     //持っている
                                }
                                else if (moveSpeed > 4)
                                {
                                    if (!isHave) plaAnimation.SetAnim(ANIM_STATE.RUN, moveSpeed); //持っていない
                                    else plaAnimation.SetAnim(ANIM_STATE.HAVE_RUN, moveSpeed); //持っている
                                }
                                else if (moveSpeed > 1)
                                {
                                    if (!isHave) plaAnimation.SetAnim(ANIM_STATE.WALK, moveSpeed);//持っていない
                                    else plaAnimation.SetAnim(ANIM_STATE.HAVE_RUN, moveSpeed); //持っている
                                }

                            }
                        }
                    }
                    else
                    {//操作していないプレイヤー
                        if ((int)plaAnimation.anim_State == 0)
                        {//待機
                            plaAnimation.SetAnim(ANIM_STATE.IDLE, 1);
                        }
                        else if ((int)plaAnimation.anim_State == 1)
                        {//歩く
                            plaAnimation.SetAnim(ANIM_STATE.WALK, subMoveSpeed);
                        }
                        else if ((int)plaAnimation.anim_State == 2)
                        {//走る
                            plaAnimation.SetAnim(ANIM_STATE.RUN, subMoveSpeed);
                        }
                        else if ((int)plaAnimation.anim_State == 3)
                        {//持ちながら待機
                            plaAnimation.SetAnim(ANIM_STATE.HAVE_IDLE, 1);
                        }
                        else if ((int)plaAnimation.anim_State == 4)
                        {//持ちながら走る
                            plaAnimation.SetAnim(ANIM_STATE.HAVE_RUN, subMoveSpeed);
                        }
                        else if ((int)plaAnimation.anim_State == 5)
                        {//落ちてる
                            plaAnimation.SetAnim(ANIM_STATE.FALL, subMoveSpeed);
                        }
                    }
                }
                #endregion
                #region オフライン環境
                else if (!RoomModel.Instance)
                {//オフライン環境
                    if(this.name == "Main")
                    {//操作しているプレイヤー
                        if (isSearch)
                        {//探している
                            if (isLow) plaAnimation.SetAnim(ANIM_STATE.SEARCH_LOW, 1);
                            else plaAnimation.SetAnim(ANIM_STATE.SEARCH_HIGH, 1);
                        }
                        else
                        {//探していない
                            if (moveSpeed > 11) plaAnimation.SetAnim(ANIM_STATE.FALL, 1); //落ちる
                            else
                            {
                                if (moveSpeed <= 1)
                                {//待機
                                    if (!isHave) plaAnimation.SetAnim(ANIM_STATE.IDLE, 1); //持っていない
                                    else plaAnimation.SetAnim(ANIM_STATE.HAVE_IDLE, 1);     //持っている
                                }
                                else if (moveSpeed > 4)
                                {
                                    if (!isHave) plaAnimation.SetAnim(ANIM_STATE.RUN, moveSpeed); //持っていない
                                    else plaAnimation.SetAnim(ANIM_STATE.HAVE_RUN, moveSpeed); //持っている
                                }
                                else if (moveSpeed > 1)
                                {
                                    if (!isHave) plaAnimation.SetAnim(ANIM_STATE.WALK, moveSpeed);//持っていない
                                    else plaAnimation.SetAnim(ANIM_STATE.HAVE_RUN, moveSpeed); //持っている
                                }

                            }
                        }
                    }
                }
                if (this.name == "Sub")
                {//操作していないプレイヤー
                    if ((int)plaAnimation.anim_State == 0)
                    {//待機
                        plaAnimation.SetAnim(ANIM_STATE.IDLE, 1);
                    }
                    else if ((int)plaAnimation.anim_State == 1)
                    {//歩く
                        plaAnimation.SetAnim(ANIM_STATE.WALK, subMoveSpeed);
                    }
                    else if ((int)plaAnimation.anim_State == 2)
                    {//走る
                        plaAnimation.SetAnim(ANIM_STATE.RUN, subMoveSpeed);
                    }
                    else if ((int)plaAnimation.anim_State == 3)
                    {//持ちながら待機
                        plaAnimation.SetAnim(ANIM_STATE.HAVE_IDLE, 1);
                    }
                    else if ((int)plaAnimation.anim_State == 4)
                    {//持ちながら走る
                        plaAnimation.SetAnim(ANIM_STATE.HAVE_RUN, subMoveSpeed);
                    }
                    else if ((int)plaAnimation.anim_State == 5)
                    {//落ちてる
                        plaAnimation.SetAnim(ANIM_STATE.FALL, subMoveSpeed);
                    }
                }
                break;
            #endregion
            #endregion

            //死亡状態
            case PLAYER_STATE.DEATH:

                Death();
                break;

            //エモート状態
            case PLAYER_STATE.EMOTE:

                //プレイヤーとプレイヤーオブジェクトの角度を同期
                child.transform.eulerAngles = this.gameObject.gameObject.transform.eulerAngles;
                Emote(1);
                break;

            case PLAYER_STATE.STRICKER:

                //プレイヤーとプレイヤーオブジェクトの角度を同期
                child.transform.eulerAngles = this.gameObject.gameObject.transform.eulerAngles;
                break;
            //上記非該当状態
            case PLAYER_STATE.ERROR:

                break;

        }

    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    private async void Death()
    {
        // すでに死亡している場合、処理しない
        if (isDead) return;

        // 死亡済みとする
        isDead = true;

        isFall = false;
        //ResetAnimation();    //アニメーターリセット

        // 手に持っているものを離す（VR時は無視）
        if (!UnityEngine.XR.XRSettings.isDeviceActive && grabber != null) grabber.Release();

        if (isMain == true)
        {
            // メインカメラを非アクティブ化
            cameraManager.TurnOffPlayerCam();
            // 0.8秒後にフェードアウトを開始
            Invoke("FadeOut", 0.8f);
        }

        hitPoint.SetActive(true);
        ChangeBodyGravity(true);    //重力をtrueに

        animator.enabled = false;
        isRespawn = false;

        if (isMain == true)
        {
            // 死亡回数を加算
            if (!isDebug) deathCnt++;
            if (resultScoreManager) resultScoreManager.failureNum++;

            // 死亡回数テキストを取得し、死亡回数を反映
            GameObject.Find("DeathCount").GetComponent<Text>().text = ": " + deathCnt + "/3";

            if(RoomModel.Instance && isMain)
            {//通信中
                await RoomModel.Instance.CountAsync(false);
            }

            // 画面の毒々しさを解除する
            tripPanel.SetActive(false);
            isTrip = false;
        }

        if (deathCnt <= 3)
        {
            // まだ3回死んでいない場合
            Invoke("RespawnPlayer", 2);  //2秒後にリスポーン
        }
        
    }

    /// <summary>
    /// エモート再生処理
    /// </summary>
    private void Emote(int animationID)
    {
        ChangeBodyGravity(false);    //重力削除
        hitPoint.SetActive(false);   //当たり判定削除

        //plaAnimation.SetAnim(PlayerAnimation.ANIM_STATE.EMOTE,moveSpeed);
    }

    /// <summary>
    /// lagdoll重力切り替え関数
    /// </summary>
    /// <param name="isChange"></param>
    private void ChangeBodyGravity(bool isChange)
    {
        //Amateur子オブジェクト全取得
        allChildren = GetAllChildTransforms(this.gameObject.transform, true);

        //子オブジェクト分周回
        foreach (Transform child in allChildren)
        {
            //子オブジェクトが存在したら
            if (child != this.transform)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();

                //Rigidbodyを取得出来たら
                if (rb != null)
                {
                    switch (isChange)
                    {
                        case true:
                            rb.useGravity = true;       //重力ON
                     
                            break;

                        case false:
                            rb.useGravity = false;      //重力OFF
 
                            break;
                    }
                }
            }
        }


    }

    /// <summary>
    /// 子オブジェクトリスト格納処理
    /// </summary>
    /// <param name="parent">対象親オブジェクト</param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static List<Transform> GetAllChildTransforms(Transform parent, bool includeInactive = true)
    {
        var results = new List<Transform>();
        if (parent == null) return results;

        // �X�^�b�N���g���������i�[���D��j
        var stack = new Stack<Transform>();
        for (int i = 0; i < parent.childCount; i++)
            stack.Push(parent.GetChild(i));

        while (stack.Count > 0)
        {
            var t = stack.Pop();

            if (includeInactive || t.gameObject.activeInHierarchy)
            {
                results.Add(t);

                // �q���X�^�b�N�ɒǉ��i���ȉ����܂߂�j
                for (int i = 0; i < t.childCount; i++)
                    stack.Push(t.GetChild(i));
            }
        }

        return results;
    }
    public List<GameObject> GetAllChildGameObjects(Transform parent, bool includeInactive = true)
    {
        var transforms = GetAllChildTransforms(parent, includeInactive);
        var gos = new List<GameObject>(transforms.Count);
        foreach (var t in transforms) gos.Add(t.gameObject);
        return gos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Abyss"))
        {
            if(resultScoreManager) resultScoreManager.carelesslyNum++;

            SEManager.Instance.Play(
            audioPath: SEPath.FALL, //再生したいオーディオのパス
                volumeRate: 1,                 //音量の倍率
                delay: 0,                      //再生されるまでの遅延時間
                pitch: 1,                      //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                 //再生終了後の処理
            );
            //死亡状態
            player_State = PLAYER_STATE.DEATH;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(RoomModel.Instance && OnlineGameManager.Player == this.gameObject)
        {
            Debug.Log("タグ：" + collision.gameObject.tag);
        }
        if (collision.gameObject.CompareTag("Container"))
        {
            Debug.Log("コンテナ衝突");

            if (resultScoreManager) resultScoreManager.carelesslyNum++;

            SEManager.Instance.Play(
                audioPath: SEPath.DEATH, //再生したいオーディオのパス
                 volumeRate: 1,                 //音量の倍率
                 delay: 0,                      //再生されるまでの遅延時間
                 pitch: 1,                      //ピッチ
                 isLoop: false,                 //ループ再生するか
                 callback: null                 //再生終了後の処理
            );
            //死亡状態に変更
            player_State = PLAYER_STATE.DEATH;
        }
        if (collision.gameObject.CompareTag("Trap"))
        { // 触れたオブジェクトがトラップの場合、死ぬ

            if (resultScoreManager) resultScoreManager.notCoolNum++;

            if (!isSliped)
            {
                SEManager.Instance.Play(
                   audioPath: SEPath.SLIP, //再生したいオーディオのパス
                    volumeRate: 1,                 //音量の倍率
                    delay: 0,                      //再生されるまでの遅延時間
                    pitch: 1,                      //ピッチ
                    isLoop: false,                 //ループ再生するか
                    callback: null                 //再生終了後の処理
                   );
                isSliped = true;
            }
            //死亡状態に変更
            player_State = PLAYER_STATE.DEATH;
        }
        if (collision.gameObject.name =="knife")
        { // 触れたオブジェクトがナイフの場合、死ぬ

            if (resultScoreManager) resultScoreManager.cruelNum++;

            SEManager.Instance.Play(
                audioPath: SEPath.DEATH, //再生したいオーディオのパス
                 volumeRate: 1,                 //音量の倍率
                 delay: 0,                      //再生されるまでの遅延時間
                 pitch: 1,                      //ピッチ
                 isLoop: false,                 //ループ再生するか
                 callback: null                 //再生終了後の処理
                );
            //死亡状態に変更
            player_State = PLAYER_STATE.DEATH;
        }
        if (collision.gameObject.CompareTag("Pot"))
        {//触れたオブジェクトが植木鉢だった場合
            if (resultScoreManager) resultScoreManager.carelesslyNum++;

            CheckableObjManager checkableObjManager = GameObject.Find("CheckableObjManager").GetComponent<CheckableObjManager>();
            if (checkableObjManager.isCheckNow) if (resultScoreManager) resultScoreManager.notCoolNum++; ;

            player_State = PLAYER_STATE.DEATH; //死亡状態にする
        }
        if (collision.gameObject.name =="Injector")
        {//触れたオブジェクトが注射器だった場合

            if (resultScoreManager) resultScoreManager.notJudgeNum++;

            SEManager.Instance.Play(
                 audioPath: SEPath.STAB, //再生したいオーディオのパス
                 volumeRate: 0.8f,                 //音量の倍率
                 delay: 0,                      //再生されるまでの遅延時間
                 pitch: 1,                      //ピッチ
                 isLoop: false,                 //ループ再生するか
                 callback: null                 //再生終了後の処理
                );
            CancelInvoke("ResetTrip");
            // 注射器を破壊する
            Destroy(collision.gameObject);
            isTrip = true;
            isHave = false;
            Invoke("ResetTrip", 10f);
            // 画面を毒々しくする
            tripPanel.SetActive(true);
        }
    }

    void RespawnPlayer()
    {

        this.gameObject.transform.position = new Vector3(warpPoint.position.x, warpPoint.position.y, warpPoint.position.z);

        //生存状態に
        player_State = PLAYER_STATE.ALIVE;
        isSliped = false;
        if (isMain)
            fadeImageScript.FadeIn();
    }

    private void ResetAnimation()
    {
    
    }
    void FadeOut()
    {
        fadeImageScript.FadeOut();

        //cameraManager.TurnOnPlayerCam();
    }

    void ResetTrip()
    {
        if (!isTrip) return;
        isTrip = false;
        // 画面の毒々しさを解除する
        tripPanel.SetActive(false);
    }

    /// <summary>
    /// オンライン限定死亡処理
    /// </summary>
    public void OnlineDeath()
    {
        //死亡状態に変更
        player_State = PLAYER_STATE.DEATH;
    }
}