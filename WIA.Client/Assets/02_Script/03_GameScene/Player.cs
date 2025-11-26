using KanKikuchi.AudioManager;
using NIGHTRAVEL.Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// プレイヤースクリプト
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] GameObject child;

    [SerializeField] GameObject hitPoint;    //Player(lagdoll)当たり判定
    [SerializeField] Animator animator;

    //
    [SerializeField] Rigidbody hip;
    [SerializeField] Rigidbody leftLeg;

    [SerializeField] Transform warpPoint; //リスポーン地点

    [SerializeField] ObjectGrabber grabber;
    //カメラ切り替え用変数
    CameraManager cameraManager;

    //フェード用変数
    FadeImage fadeImageScript;

    //死亡判定
    public bool isDead = false;
    //リスポーン判定
    public bool isRespawn = true;

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

    [SerializeField] public PLAYER_STATE player_State;
    List<Transform> allChildren;

    public int deathCnt = 0; //死亡回数

    private void Awake()
    {
        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
    }

    void Start()
    {
        cameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        // �t�F�[�h�C���[�W����X�N���v�g���擾
        fadeImageScript = GameObject.Find("FadeImage").GetComponent<FadeImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) player_State = PLAYER_STATE.DEATH;
        if (Input.GetKeyDown(KeyCode.I)) player_State = PLAYER_STATE.ALIVE;

        switch (player_State)
        {
            //�����O�̏��
            case PLAYER_STATE.STOP:

                break;

            //�������̏ꍇ
            case PLAYER_STATE.ALIVE:

                this.gameObject.GetComponent<CapsuleCollider>().enabled = true;

                isDead = false;
                //cameraManager.TurnOnPlayerCam();

                //プレイヤーとプレイヤーオブジェクトの角度を同期
                child.transform.eulerAngles = this.gameObject.gameObject.transform.eulerAngles; // Z����10���ɐݒ� parent

                ChangeBodyGravity(false);

           

                hitPoint.SetActive(false);
                animator.enabled = true;

                //以下に生存中の処理を記入
                //----------------------------------------------------

                if(!isRespawn)
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


                break;

            //���S���̏ꍇ
            case PLAYER_STATE.DEATH:
               

                Death();
                break;

            //�G���[�g�Đ����̏ꍇ
            case PLAYER_STATE.EMOTE:
                //プレイヤーとプレイヤーオブジェクトの角度を同期
                child.transform.eulerAngles = this.gameObject.gameObject.transform.eulerAngles; // Z����10���ɐݒ� parent
                Emote(1);
                break;

            case PLAYER_STATE.STRICKER:

                //プレイヤーとプレイヤーオブジェクトの角度を同期
                child.transform.eulerAngles = this.gameObject.gameObject.transform.eulerAngles; // Z����10���ɐݒ� parent
                break;
            //�G���[�̏ꍇ
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
        // 手に持っているものを離す
        grabber.Release();

        // メインカメラを非アクティブ化
        cameraManager.TurnOffPlayerCam();
        // 0.8秒後にフェードアウトを開始
        Invoke("FadeOut", 0.8f);

        hitPoint.SetActive(true);
        ChangeBodyGravity(true);

        animator.enabled = false;
        isRespawn = false;

        // 死亡回数を加算
        deathCnt++;
        // 死亡回数テキストを取得し、死亡回数を反映
        GameObject.Find("DeathCount").GetComponent<Text>().text = ": " + deathCnt + "/3";


        if (deathCnt <= 3)
        {
            // まだ3回死んでいない場合
            Invoke("RespawnPlayer", 2);  //2秒後にリスポーン
        }
        
    }

    /// <summary>
    /// �G���[�g�֐�
    /// </summary>
    private void Emote(int animationID)
    {
        ChangeBodyGravity(false);
        //�����蔻����ꎞ�I�ɍ폜
        hitPoint.SetActive(false);

        //�����Ŏ擾�����A�j���[�V����ID�ŃA�j���[�V�����Đ�(�g�ݍ��ނƂ��ɏ��������Ă�)
    }

    /// <summary>
    /// �d�͐؂�ւ��֐�(�R�������Ɖ����x���E�˔j����)
    /// </summary>
    /// <param name="isChange"></param>
    private void ChangeBodyGravity(bool isChange)
    {
        // true��n���Ɣ�A�N�e�B�u�ȃI�u�W�F�N�g�����ׂĎ擾���܂�
        allChildren = GetAllChildTransforms(this.gameObject.transform, true);

        // �擾�����I�u�W�F�N�g�̃��X�g��\�������
        foreach (Transform child in allChildren)
        {
            // �q���I�u�W�F�N�g���g���܂܂�邽�߁A���g�����O����ꍇ��if���Ŕ��肵�܂�
            if (child != this.transform)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();

                //Rigidbody���t���Ă���I�u�W�F�N�g�ɑ΂��ďd�͐ؑ�
                if (rb != null)
                {
                    switch (isChange)
                    {
                        case true:
                            rb.useGravity = true;       //�d�͂�ON�ɂ���
                     
                            break;

                        case false:
                            rb.useGravity = false;      //�d�͂�OFF�ɂ���
 
                            break;
                    }

                    //Debug.Log("�q���I�u�W�F�N�g��: " + child.gameObject.name);
                }
            }
        }


    }


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
            //�v���C���[�̏�Ԃ����S��Ԃɂ���
            player_State = PLAYER_STATE.DEATH;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Container"))
        {
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
    }

    void RespawnPlayer()
    {
        // ���[�v�|�C���g�Ɉړ�����
        //transform.position = new Vector3(warpPoint.position.x, warpPoint.position.y, warpPoint.position.z);
        this.gameObject.transform.position = new Vector3(warpPoint.position.x, warpPoint.position.y, warpPoint.position.z);

        //�v���C���[�̏�Ԃ𐶑���Ԃɂ���
        player_State = PLAYER_STATE.ALIVE;

        fadeImageScript.FadeIn();
    }

    void FadeOut()
    {
        fadeImageScript.FadeOut();

        //cameraManager.TurnOnPlayerCam();
    }

}