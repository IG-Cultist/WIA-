using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Player;

public class FadeImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // フェードアウト終了後の処理
    public void FadeOut()
    {
        this .gameObject.SetActive(true);
    }
}