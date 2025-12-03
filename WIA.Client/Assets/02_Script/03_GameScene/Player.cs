using KanKikuchi.AudioManager;
using NIGHTRAVEL.Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerAnimation;

/// <summary>
/// プレイヤースクリプト
/// </summary>
public class Player : MonoBehaviour
{
    [Header("プレイヤー(当たり判定・アニメーション)系")]
    [SerializeField] GameObject child;
    [SerializeField] GameObject hitPoint;    //Player(lagdoll)当たり判定
    [SerializeField] Animator animator;      //アニメーター(速度調整用)
    [SerializeField] PlayerAnimation plaAnimation;    //アニメーションスクリプト

    [Header("アクション座標系")]
    [SerializeField] Transform warpPoint; //リスポーン地点
    [SerializeField] ObjectGrabber grabber;

    //カメラ切り替え用変数
    CameraManager cameraManager;

    //フェード用変数
    FadeImage fadeImageScript;

    Rigidbody rigidbody;      //慣性取得用

    private float moveSpeed;

    [Header("フラグ系")]
    public bool isHave = false;
    public bool isFall = false;
    public bool isDead = false;     //死亡判定
    public bool isRespawn = true;   //リスポーン判定
    public bool isDebug;
    public bool isTrip = false;

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


    private void Awake()
    {
        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
        rigidbody = this.GetComponent<Rigidbody>();
    }

    void Start()
    {

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
        ////デバッグ用
        //if (Input.GetKeyDown(KeyCode.K)) player_State = PLAYER_STATE.DEATH;
        //if (Input.GetKeyDown(KeyCode.I)) player_State = PLAYER_STATE.ALIVE;

        moveSpeed = rigidbody.linearVelocity.magnitude * 3.0f;   //オブジェクト速度を元にアニメーションの速度を決定
        if(moveSpeed < 0) moveSpeed = 0;
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

                if (moveSpeed > 11)
                {
                    plaAnimation.SetAnim(ANIM_STATE.FALL, 1);

                }
                else
                {
                    if (moveSpeed <= 1)
                    {
                        if (!isHave) plaAnimation.SetAnim(ANIM_STATE.IDLE, 1);
                        else plaAnimation.SetAnim(ANIM_STATE.HAVE_IDLE, 1);
                    }
                    else if (moveSpeed > 4)
                    {
                        if (!isHave) plaAnimation.SetAnim(ANIM_STATE.RUN, moveSpeed);
                        else plaAnimation.SetAnim(ANIM_STATE.HAVE_RUN, moveSpeed);
                    }
                    else if (moveSpeed > 1)
                    {
                        if (!isHave) plaAnimation.SetAnim(ANIM_STATE.WALK, moveSpeed);
                        else plaAnimation.SetAnim(ANIM_STATE.HAVE_RUN, moveSpeed);
                    }

                }

                break;

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
    private void Death()
    {
        // すでに死亡している場合、処理しない
        if (isDead) return;

        // 死亡済みとする
        isDead = true;

        isFall = false;
        //ResetAnimation();    //アニメーターリセット

        // 手に持っているものを離す（VR時は無視）
        if (!UnityEngine.XR.XRSettings.isDeviceActive && grabber != null) grabber.Release();

        // メインカメラを非アクティブ化
        cameraManager.TurnOffPlayerCam();
        // 0.8秒後にフェードアウトを開始
        Invoke("FadeOut", 0.8f);

        hitPoint.SetActive(true);
        ChangeBodyGravity(true);    //重力をtrueに

        animator.enabled = false;
        isRespawn = false;

        // 死亡回数を加算
        if(!isDebug)deathCnt++;
        // 死亡回数テキストを取得し、死亡回数を反映
        GameObject.Find("DeathCount").GetComponent<Text>().text = ": " + deathCnt + "/3";

        // 画面の毒々しさを解除する
        tripPanel.SetActive(false);
        isTrip = false;

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
        if (collision.gameObject.CompareTag("Container"))
        {
            Debug.Log("コンテナ衝突");
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
        if (collision.gameObject.name =="knife")
        { // 触れたオブジェクトがナイフの場合、死ぬ
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
        if(collision.gameObject.CompareTag("Pot"))
        {//触れたオブジェクトが植木鉢だった場合
            player_State = PLAYER_STATE.DEATH; //死亡状態にする
        }
        if (collision.gameObject.name =="Injector")
        {//触れたオブジェクトが注射器だった場合

            // 注射器を破壊する
            Destroy(collision.gameObject);
            isTrip = true;

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
}