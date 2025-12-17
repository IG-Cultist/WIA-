using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private int potCount; //植木鉢カウント
    private int maxPots = 3;//植木鉢の最大数
    private int spawnConditionsPots = 2; //植木鉢が生成する条件の値
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
        }
        else
        {
            //植木鉢を生成する
            GeneratePot();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (potCount >= maxPots)
        {//potListの要素が3以上だったら
            isThreePot = true; // 植木鉢が3個ある状態にする
            //RemoveList(generatNumber);
        }
        if (potCount <= spawnConditionsPots)
        {//potListの要素が2以下だったら
            if (RoomModel.Instance)
            {//通信時のみ
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
    /// 植木鉢がなくなった時の処理
    /// </summary>
    public void PotLost(GameObject gameObject)
    {
        FlowerPot flowerPot = gameObject.GetComponent<FlowerPot>();
        GameObject fragment = Instantiate(potFragmentObj);
        fragment.transform.position = gameObject.transform.position;
        flowerPot.SpawnFragment(fragment);
        Destroy(gameObject);
        potCount--;
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
                                                                                                                                         //potListに要素を追加する
            potList.Add(potObj);
        }

        potCount++;
    }

    public void RemoveList(int generateNumber)
    {
        if(nowSpawnList.Contains(generateNumber))
        {
            nowSpawnList.Remove(generateNumber);
        }
        if(RoomModel.Instance)
        {
            //gameManager.DeliteSynObj();
        }
    }
}