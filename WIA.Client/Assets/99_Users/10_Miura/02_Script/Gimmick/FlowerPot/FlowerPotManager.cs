using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Purchasing;
using UnityEngine;


public class FlowerPotManager : MonoBehaviour
{
    [SerializeField] public GameObject potObj; //植木鉢オブジェクト
    [SerializeField] public GameObject potFragmentObj; //植木鉢の破片オブジェクト

    public List<GameObject> potList = new List<GameObject>(); //植木鉢の生成個数を格納するリスト
    public GameObject[] randomSpawnPoint; //植木鉢がスポーンする場所のリスト

    public bool isPot = false; //植木鉢が存在するかどうかの変数
    public int potCnt; //植木鉢がいくつあるかの変数
    bool collitionCheck;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {//Enterキーを押されたら

            if (isPot == true) { return; } //植木鉢が3個以上ある場合、以下の処理を実行しない

            for (int i = 0;i<3;i++)
            {//3回繰り返す
                GeneratePot(); //植木鉢を生成する
                potCnt++; //植木鉢の個数カウントを増やす

                if (potCnt>=3)
                {//potCuntが3以上になったら
                    isPot = true; //植木鉢が3個ある状態にする
                }
                if(potCnt<=2)
                {
                    isPot= false; //植木鉢が3個存在しない状態
                }
            }
        }
    }

    /// <summary>
    /// 植木鉢を生成する処理
    /// </summary>
    private void GeneratePot()
    {
        //生成位置を決める
        int generatNumber = Random.Range(0, randomSpawnPoint.Length); //生成位置をrandomSpawnPointの中から決める

        Instantiate(potObj, randomSpawnPoint[generatNumber].transform.position, randomSpawnPoint[generatNumber].transform.rotation); //植木鉢を生成する


        potList.Add(potObj);


    }
}
