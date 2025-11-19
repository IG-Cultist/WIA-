using System.Security.Cryptography;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.ParticleSystem;
using KanKikuchi.AudioManager;

public class container : MonoBehaviour
{
    //[SerializeField] ParticleSystem m_particle;
    [SerializeField] float m_force ;
    [SerializeField] float m_radius ;
    [SerializeField] float m_upwards ;
    Vector3 m_position;

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.tag == "Player")
       {
            Explosion();
       }
    }

    public void Explosion()
    {
        /*m_particle.Play();
        m_position = m_particle.transform.position;*/

        // 範囲内のRigidbodyにAddExplosionForce
        Collider[] hitColliders = Physics.OverlapSphere(m_position, m_radius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.tag == "Player")
            {

                if (hitColliders[i].GetComponent<Player>().player_State == Player.PLAYER_STATE.STRICKER) return;
                SEManager.Instance.Play(
                    audioPath: SEPath.DEATH, //再生したいオーディオのパス
                    volumeRate: 1,                 //音量の倍率
                    delay: 0,                      //再生されるまでの遅延時間
                    pitch: 1,                      //ピッチ
                    isLoop: false,                 //ループ再生するか
                    callback: null                 //再生終了後の処理
                );

                Debug.Log("Playerにぶつかった！");

                var rb = hitColliders[i].GetComponent<Rigidbody>();
                if (rb)
                {


                    //吹き飛ばす方向を求める(プレイヤーから触れたものの方向)
                    Vector3 toVec = GetAngleVec(hitColliders[i].gameObject,this.gameObject);

                    //Y方向を足す
                    toVec = toVec + new Vector3(0, m_upwards, 0);

                    //ふきとべええ
                    rb.AddForce(toVec * m_force, ForceMode.Impulse);

                }

            }
           
        }
    }


    Vector3 GetAngleVec(GameObject _from, GameObject _to)
    {
        //高さの概念を入れないベクトルを作る
        Vector3 fromVec = new Vector3(_from.transform.position.x, 0, _from.transform.position.z);
        Vector3 toVec = new Vector3(_to.transform.position.x, 0, _to.transform.position.z);

        return Vector3.Normalize(toVec - fromVec);
    }
}
