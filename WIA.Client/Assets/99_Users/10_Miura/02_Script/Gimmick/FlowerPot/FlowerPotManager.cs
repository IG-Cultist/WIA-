using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using UnityEngine.UIElements;


public class FlowerPotManager : MonoBehaviour
{
    [SerializeField] public GameObject potObj; //植木鉢オブジェクト
    [SerializeField] public GameObject potFragmentObj; //植木鉢の破片オブジェクト

    public List<GameObject> potList = new List<GameObject>(); //植木鉢の生成個数を格納するリスト
    public List<GameObject> randomSpawnPoint =new List<GameObject>(); //植木鉢がスポーンする場所のリスト

    public bool isThreePot = false; //植木鉢が3個存在するかどうかの変数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {//Enterキーを押されたら
            if (isThreePot == true) { return; } //植木鉢が3個以上ある場合、以下の処理を実行しない

            GeneratePot(); //植木鉢を生成する

            if (potList.Count >= 3)
            {//potListの要素が3以上だったら
                isThreePot = true; // 植木鉢が3個ある状態にする
            }
            if (potList.Count <= 2)
            {//potListの要素が2以下だったら
                isThreePot = false; // 植木鉢が3個存在しない状態
            }
        }
    }

    /// <summary>
    /// 植木鉢を生成する処理
    /// </summary>
    private void GeneratePot()
    {
        if(isThreePot==false)
        {//植木鉢が三個無い時
            for (int i = 0; i < 3; i++)
            {//三回分繰り返す
                // 生成位置を決める
                int generatNumber = Random.Range(0, randomSpawnPoint.Count); // 生成位置をspawnPointPosの中から決める

                    // potObjを生成する
                    Instantiate(potObj, randomSpawnPoint[generatNumber].transform.position, randomSpawnPoint[generatNumber].transform.rotation); // randomSpawnPointに格納されたgameObjectのgeneratNumberの場所に生成

                //potListに要素を追加する
                    potList.Add(potObj);
            }
        }
    }
}