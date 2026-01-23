using UnityEngine;

public class Knife : MonoBehaviour
{
    private async void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player" && collision.gameObject.name == "Worker")
        {
            if(RoomModel.Instance)
            {
                await RoomModel.Instance.DeliteObjectAsync(this.name, this.tag);
            }
            else
            {
                Destroy(this.gameObject);
            }
            //collision.transform.GetComponent<Player>().Death();
        }
    }
}
