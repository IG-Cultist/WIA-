using UnityEngine;


public class FlowerPotManager : MonoBehaviour
{
    [SerializeField] GameObject potObj; //植木鉢オブジェクト

    public GameObject[] randomSpawnPoint; //植木鉢がスポーンする場所のリスト
    private GameObject[] useSpawnList; //既に使用しているスポーン場所のリスト
    bool isPot = false; //植木鉢が存在するかどうかの変数
    int potCnt; //植木鉢がいくつあるかの変数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; //水平方向の移動
        //transform.position += new Vector3(moveX, 0, 0); //オブジェクトの位置を更新

        if (isPot == true) { return; }  //植木鉢がフィールドに三個存在している場合以下の処理を実行しない

        if (Input.GetKeyUp(KeyCode.Return))
        {//Enterキーを押されたら

            for(int i = 0;i<3;i++)
            {//3回繰り返す
                GeneratePot(); //植木鉢を生成する
                potCnt++; //植木鉢の数をカウントする

                if(potCnt==3)
                {//potCuntが3になったら
                    isPot = true; //植木鉢が三個ある状態にする
                }
                if(potCnt<=2)
                {
                    isPot= false; //植木鉢が三個存在しない状態
                }
            }
        }
    }

    /// <summary>
    /// 植木鉢を生成する処理
    /// </summary>
    private void GeneratePot()
    {
        if(potCnt<=2)
        {//potCntが2以下だったら新しく生成する
         //生成位置を決める

            int generatNumber = Random.Range(0, randomSpawnPoint.Length); //生成位置をrandomSpawnPointの中から決める
            
            for(int i = 0; i< generatNumber;i++)
            {

            }

            Instantiate(potObj, randomSpawnPoint[generatNumber].transform.position, randomSpawnPoint[generatNumber].transform.rotation); //植木鉢を生成する
        }

        //Vector3 genetratePos = new Vector3(0, 5.5f, 0);
        //Instantiate(potObj, genetratePos, Quaternion.identity);
    }
}
