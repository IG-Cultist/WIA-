/// ------------------------------
/// デリバリーマネージャー
/// Author:Nishiura Date:25/11/10
/// ------------------------------
using UnityEngine;

public class Coffee : MonoBehaviour
{
    DeliveryManager deliveryManager;

    private void Start()
    {
        deliveryManager = GameObject.Find("DeliveryManager").GetComponent<DeliveryManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            deliveryManager.isCreated = false;
            Destroy(this.gameObject);
        }
    }
}
