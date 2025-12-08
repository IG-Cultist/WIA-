using KanKikuchi.AudioManager;
using UnityEngine;

public class PlaySE : MonoBehaviour
{
    private AudioSource source;//AudioSource型の変数aを宣言
    [SerializeField] private AudioClip se;//AudioClip型の変数bを宣言

    float delayTime = 0;


    private void Start()
    {
        source = this.GetComponent<AudioSource>();
    }
    private void Update()
    {
        delayTime += Time.deltaTime;   //調
    }

    void OnCollisionEnter(Collision collision)
    {
        //フィールドに当たった場合
        if (collision.gameObject.GetComponent<Player>().isMain == true)
        {
            if (delayTime <= 0.5f) return;

            source.PlayOneShot(se);
        }
        

    }
}
