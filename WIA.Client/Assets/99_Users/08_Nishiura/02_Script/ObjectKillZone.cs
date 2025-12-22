/// ------------------------------
/// オブジェクト破壊領域マネージャー
/// Author:Nishiura Date:25/12/22
/// ------------------------------
using UnityEngine;

public class ObjectKillZone : MonoBehaviour
{
    // プレイヤースクリプト
    Player player = GameObject.Find(OnlineGameManager.Player.name).gameObject.GetComponent<Player>();
    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.name =="Cup" && other.gameObject.tag == "Item")
            || (other.gameObject.name == "Injector" && other.gameObject.tag == "Item"))
        {
            if (player && player.isHave)
            {
                player.isHave = false;
            }
            Destroy(other.gameObject);
        }
    }
}
