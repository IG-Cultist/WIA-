using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FlowerPotManager : MonoBehaviour
{
    [SerializeField] public GameObject potObj; //植木鉢オブジェクト
    [SerializeField] public GameObject potFragmentObj; //植木鉢の破片オブジェクト

    public List<GameObject> potList = new List<GameObject>(); //植木鉢の生成個数を格納するリスト
    public List<GameObject> randomSpawnPoint =new List<GameObject>(); //植木鉢がスポーンする場所のリスト
    public List<int> nowSpawnList=new List<int>();

    public bool isThreePot = false; //植木鉢が3個存在するかどうかの変数
    public int generatNumber;

    //通信用
    OnlineGameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(RoomModel.Instance)
        {//通信時のみ
            //オンラインゲームマネージャー取得
            gameManager = GameObject.Find("OnlineGameManager").GetComponent<OnlineGameManager>();
            if(OnlineGameManager.Player.name == "Stricker")
            {//労災側のみ
                //植木鉢を生成する
                GeneratePot();
            }
        }
        else
        {
            //植木鉢を生成する
            GeneratePot();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (potList.Count >= 3)
        {//potListの要素が3以上だったら
            isThreePot = true; // 植木鉢が3個ある状態にする
            RemoveList(generatNumber);
        }
        if (potList.Count <= 2)
        {//potListの要素が2以下だったら
            if (RoomModel.Instance)
            {//通信時のみ
             //オンラインゲームマネージャー取得
                if (OnlineGameManager.Player.name == "Stricker")
                {
                    GeneratePot();
                }
            }
            else
            {
                GeneratePot(); //植木鉢を生成する
            }
            isThreePot = false; // 植木鉢が3個存在しない状態
        }
    }

    /// <summary>
    /// 植木鉢を生成する処理
    /// </summary>
    private void GeneratePot()
    {
        while (true)
        {        
            // 生成位置を決める
            generatNumber = Random.Range(0, randomSpawnPoint.Count); // 生成位置をspawnPointPosの中から決める

            if (!nowSpawnList.Contains(generatNumber))
            {
                break;
            }
        }
        nowSpawnList.Add(generatNumber);

        if(RoomModel.Instance)
        {
            gameManager.SpawnObj(randomSpawnPoint[generatNumber].transform.position);
        }
        else
        {
            Instantiate(potObj, randomSpawnPoint[generatNumber].transform.position, randomSpawnPoint[generatNumber].transform.rotation); // randomSpawnPointに格納されたgameObjectのgeneratNumberの場所に生成
        }

        //potListに要素を追加する
        potList.Add(potObj);
    }

    public void RemoveList(int generateNumber)
    {
        if(nowSpawnList.Contains(generateNumber))
        {
            nowSpawnList.Remove(generateNumber);
        }
    }
}