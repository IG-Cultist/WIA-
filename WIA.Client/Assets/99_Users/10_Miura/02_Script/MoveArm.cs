using DG.Tweening;
using UnityEngine;

public class MoveArm : MonoBehaviour
{
    [SerializeField] GameObject craneArm;
    [SerializeField] float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; //水平方向の移動

        transform.position += new Vector3(0, 0, moveZ); // オブジェクトの位置を更新

    }
}