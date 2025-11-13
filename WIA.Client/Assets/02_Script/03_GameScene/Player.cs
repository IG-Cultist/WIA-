using NIGHTRAVEL.Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// プレイヤー処理クラス
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] GameObject child;

    [SerializeField] GameObject hitPoint;    //Playerの当たり判定
    [SerializeField] Animator animator;

    //
    [SerializeField] Rigidbody hip;
    [SerializeField] Rigidbody leftLeg;

    [SerializeField] Transform warpPoint;

    // 三人称視点カメラ
    [SerializeField] GameObject thirdPersonCamera;

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
    List<Transform> allChildren;

    int deathCnt = 0; //死亡回数の変数

    void Start()
    {
        thirdPersonCamera.GetComponent<CinemachineCamera>().Priority = 1;
        this.gameObject.transform.GetChild(1).GetComponent<CinemachineCamera>().Priority = 10;

        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) player_State = PLAYER_STATE.DEATH;
        if (Input.GetKeyDown(KeyCode.I)) player_State = PLAYER_STATE.ALIVE;

        switch (player_State)
        {
            //生成前の状態
            case PLAYER_STATE.STOP:

                break;

            //生存中の場合
            case PLAYER_STATE.ALIVE:
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);

                // a) 角度指定(v3Angle → trQ)
                child.transform.eulerAngles = this.gameObject.gameObject.transform.eulerAngles; // Z軸を10°に設定 parent

                ChangeBodyGravity(false);

                Debug.Log("いきてる");

                hitPoint.SetActive(false);
                animator.enabled = true;

                //この下に生存中の処理を書く




                break;

            //死亡中の場合
            case PLAYER_STATE.DEATH:
                Death();
                break;

            //エモート再生中の場合
            case PLAYER_STATE.EMOTE:
                Emote(1);
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
        ChangeBodyGravity(true);

        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);

        hitPoint.SetActive(true);
        animator.enabled = false;
    }

    /// <summary>
    /// エモート関数
    /// </summary>
    private void Emote(int animationID)
    {
        ChangeBodyGravity(false);
        //当たり判定を一時的に削除
        hitPoint.SetActive(false);

        //引数で取得したアニメーションIDでアニメーション再生(組み込むときに書き換えてね)
    }

    /// <summary>
    /// 重力切り替え関数(コレ無いと加速度限界突破する)
    /// </summary>
    /// <param name="isChange"></param>
    private void ChangeBodyGravity(bool isChange)
    {

        // trueを渡すと非アクティブなオブジェクトもすべて取得します
        allChildren = GetAllChildTransforms(this.gameObject.transform, true);

        // 取得したオブジェクトのリストを表示する例
        foreach (Transform child in allChildren)
        {
            // 子孫オブジェクト自身も含まれるため、自身を除外する場合はif文で判定します
            if (child != this.transform)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();

                //Rigidbodyが付いているオブジェクトに対して重力切替
                if (rb != null)
                {
                    switch (isChange)
                    {
                        case true:
                            rb.useGravity = true;       //重力をONにする
                            Debug.Log("ON");
                            break;

                        case false:
                            rb.useGravity = false;      //重力をOFFにする
                            Debug.Log("OFF");
                            break;
                    }

                    //Debug.Log("子孫オブジェクト名: " + child.gameObject.name);
                }
            }
        }


    }


    public static List<Transform> GetAllChildTransforms(Transform parent, bool includeInactive = true)
    {
        var results = new List<Transform>();
        if (parent == null) return results;

        // スタックを使った反復（深さ優先）
        var stack = new Stack<Transform>();
        for (int i = 0; i < parent.childCount; i++)
            stack.Push(parent.GetChild(i));

        while (stack.Count > 0)
        {
            var t = stack.Pop();

            if (includeInactive || t.gameObject.activeInHierarchy)
            {
                results.Add(t);

                // 子をスタックに追加（孫以下を含める）
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
            //プレイヤーの状態を死亡状態にする
            player_State = PLAYER_STATE.DEATH;

            deathCnt++;

            //2秒後に復活する
            Invoke("RespawnPlayer", 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Container"))
        {
            //プレイヤーの状態を死亡状態にする
            player_State = PLAYER_STATE.DEATH;

            deathCnt ++;

            //2秒後に復活する
            Invoke("RespawnPlayer", 2);
        }
    }

    void RespawnPlayer()
    {
        //プレイヤーの状態を生存状態にする
        player_State = PLAYER_STATE.ALIVE;
        // ワープポイントに移動する
        transform.position = new Vector3(warpPoint.position.x, warpPoint.position.y, warpPoint.position.z);
    }
}