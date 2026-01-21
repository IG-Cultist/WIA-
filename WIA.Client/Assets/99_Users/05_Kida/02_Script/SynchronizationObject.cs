using UnityEngine;

public class SynchronizationObject : MonoBehaviour
{
    string id;
    OnlineGameManager gameManager = new OnlineGameManager();

    private void Start()
    {
        if (!RoomModel.Instance) return;
        gameManager = GameObject.Find("OnlineGameManager").GetComponent<OnlineGameManager>();
        var synObjList = gameManager.GetSynObj();
        for (int i = 0; i < synObjList.Count; i++)
        {
            if (synObjList[i].gameObject == this.gameObject)
            {
                id = i.ToString(); break;
            }

            if (OnlineGameManager.ObjList.Count == 0) continue;

            if (OnlineGameManager.ObjList[OnlineGameManager.SpawnObjId] == this.gameObject)
            {
                id = OnlineGameManager.SpawnObjId; break;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!RoomModel.Instance) return;
        if (collision.gameObject.tag != "Player") return;
        if (collision.gameObject.GetComponent<Player>().isMain == true)
        {
            gameManager.ObjectOwnershipSwap(id.ToString(), RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!RoomModel.Instance) return;
        if (other.gameObject.tag != "Player") return;
        if (other.gameObject.GetComponent<Player>().isMain == true)
        {
            gameManager.ObjectOwnershipSwap(id.ToString(), RoomModel.Instance.joinedUserList[RoomModel.Instance.ConnectionId].JoinOrder);
        }
    }
}
