using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] GameObject potObj;
    [SerializeField] GameObject potFragmentObj;
    [SerializeField] float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Base")
        {//Baseタグのオブジェクトに触れたら
            GameObject fragment; //破片オブジェクト
            fragment = Instantiate(potFragmentObj, potObj.transform.position, potObj.transform.rotation);

            //植木鉢を消す
            Destroy(this.gameObject);

            for (int i = 0; i < fragment.transform.childCount; i++)
            {//potFragmentObjの子の数だけループ
             //植木鉢の破片を生成する
                fragment.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(new Vector2(50, 50)); //子を取得　

                FadeFragment(fragment.transform.GetChild(i)); //破片をフェードアウトさせる
            }
        }
    }

    /// <summary>
    /// 破片をフェードアウトさせる処理
    /// </summary>
    /// <param name="fragment"></param>
    public void FadeFragment(Transform fragment)
    {
        fragment.GetComponent<Renderer>().material.DOFade(0, 6);

        DestroyFragment(fragment);
    }

    /// <summary>
    /// 破片を消す処理
    /// </summary>
    /// <param name="fragment"></param>
    public async void DestroyFragment(Transform fragment)
    {
        await Task.Delay(6000); //6秒待つ　
        Destroy(fragment.gameObject);　//破片を消す
    }
}