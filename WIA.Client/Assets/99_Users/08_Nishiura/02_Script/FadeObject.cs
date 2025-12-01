using DG.Tweening;
using UnityEngine;

public class FadeObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FadeMySelf();
    }


    public void FadeMySelf()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).GetComponent<Renderer>().material.DOFade(0, 10); // マテリアルを取得してフェードアウト
        }

        Invoke("DestroyThisObject", 10f);
    }


    void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
}
