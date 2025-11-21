using UnityEngine;

public class SynchronizationObject : MonoBehaviour
{
    string id;
    OnlineGameManager gameManager = new OnlineGameManager();

    private void Start()
    {
        if (!RoomModel.Instance) return;
        gameManager = GameObject.Find("OnlineGameManager").GetComponent<OnlineGameManager>();
        for (int i = 0; i < OnlineGameManager.SyncGameObjectList.Count; i++)
        {
            if (OnlineGameManager.SyncGameObjectList[i].name == this.gameObject.name)
            {
                id = i.ToString(); break;
            }

            if (OnlineGameManager.ObjList.Count == 0) continue;

            if (OnlineGameManager.ObjList[OnlineGameManager.SpawnObjId].name == this.gameObject.name)
            {
                id = OnlineGameManager.SpawnObjId; break;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!RoomModel.Instance) return;

        if (collision.collider.tag == "Player")
        {
            gameManager.ObjectOwnershipSwap(id.ToString(), RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder);
        }
    }
}
