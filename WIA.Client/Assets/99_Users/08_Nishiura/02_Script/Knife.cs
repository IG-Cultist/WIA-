using UnityEngine;

public class Knife : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            Destroy(this.gameObject);
            //collision.transform.GetComponent<Player>().Death();
        }
    }
}
