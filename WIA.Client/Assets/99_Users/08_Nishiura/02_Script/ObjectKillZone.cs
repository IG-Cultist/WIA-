/// ------------------------------
/// オブジェクト破壊領域マネージャー
/// Author:Nishiura Date:25/12/22
/// ------------------------------
using UnityEngine;

public class ObjectKillZone : MonoBehaviour
{
    Player player;
    private void Start()
    {
        if (RoomModel.Instance)
        {
            // プレイヤースクリプト
            player = GameObject.Find(OnlineGameManager.Player.name).gameObject.GetComponent<Player>();
        }
        else
        {
            player = GameObject.Find("Main").GetComponent<Player>();
        }
    }

    private async void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.name.Contains("Cup")   && other.gameObject.tag == "Item")
            || (other.gameObject.name.Contains("Injector") && other.gameObject.tag == "Item"))
        {
            if (player && player.isHave)
            {
                player.isHave = false;
            }
            if(RoomModel.Instance)
            {
                await RoomModel.Instance.DeliteObjectAsync(other.name,other.tag);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
