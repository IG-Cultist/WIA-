//=================================
// ステージ1のキューブのスクリプト
// Aouther:y-miura
// Date:2025/11/06
//=================================

using UnityEngine;

public class Cube : MonoBehaviour
{
    public Transform warpPoint; // ワープポイントの位置
    public Transform minPoint;
    public Transform maxPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Abyss"))
        {
            // rangeAとrangeBのx座標の範囲内でランダムな数値を作成
            float x = Random.Range(minPoint.position.x, maxPoint.position.x);
            float z= Random.Range(minPoint.position.z, maxPoint.position.z);

            // ワープポイントに移動する
            transform.position = new Vector3(x,warpPoint.position.y,z);
        }
    }
}
