/// ------------------------------
/// オブジェクト破壊領域マネージャー
/// Author:Nishiura Date:25/12/22
/// ------------------------------
using UnityEngine;

public class ObjectKillZone : MonoBehaviour
{
    // プレイヤースクリプト
    Player player = GameObject.Find(OnlineGameManager.Player.name).gameObject.GetComponent<Player>();
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
