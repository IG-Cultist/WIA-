using DG.Tweening;
using UnityEngine;

public class FadeObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("FadeMySelf", 10f);
    }


    public void FadeMySelf()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).GetComponent<Renderer>().material.DOFade(0, 1f); // マテリアルを取得してフェードアウト
            this.gameObject.transform.GetChild(i).GetComponent<MeshCollider>().enabled = false;
        }

        Invoke("DestroyThisObject", 1f);
    }


    void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
}
