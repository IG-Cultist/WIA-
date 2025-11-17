using UnityEngine;


public class FlowerPotManager : MonoBehaviour
{
    [SerializeField] GameObject potObj;
    [SerializeField] float moveSpeed;

    //public GameObject[] randomSpawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; //水平方向の移動
        transform.position += new Vector3(moveX, 0, 0); //オブジェクトの位置を更新

        if (Input.GetKeyUp(KeyCode.Return))
        {
            GeneratePot();
        }
    }

    private void GeneratePot()
    {
        ////生成位置を決める
        //int genetNumber = Random.Range(0, randomSpawnPoint.Length); //生成位置をrandomSpawnPointの中から決める
        //Instantiate(potObj, randomSpawnPoint[genetNumber].transform.position, randomSpawnPoint[genetNumber].transform.rotation);

        Vector3 genetratePos = new Vector3(0, 5.5f, 0);
        Instantiate(potObj, genetratePos, Quaternion.identity);
    }
}
