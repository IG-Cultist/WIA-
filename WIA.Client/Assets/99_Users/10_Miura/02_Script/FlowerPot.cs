using UnityEngine;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] GameObject potObj;
    [SerializeField] GameObject breakPotObj;
    [SerializeField] float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; //水平方向の移動

        transform.position += new Vector3(moveX, 0, 0); //オブジェクトの位置を更新
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(potObj);
        Instantiate(breakPotObj,this.transform.position,this.transform.rotation);
    }
}
