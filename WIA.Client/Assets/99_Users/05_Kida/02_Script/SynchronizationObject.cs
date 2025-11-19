using UnityEngine;

public class SynchronizationObject : MonoBehaviour
{
    string id;
    PreGameManager gameManager = new PreGameManager();

    private void Start()
    {
        if (!RoomModel.Instance) return;
        gameManager = GameObject.Find("PreGameManager").GetComponent<PreGameManager>();
        for (int i = 0; i < PreGameManager.SyncGameObjectList.Count; i++)
        {
            if (PreGameManager.SyncGameObjectList[i].name == this.gameObject.name)
            {
                id = i.ToString(); break;
            }

            if (PreGameManager.ObjList.Count == 0) continue;

            if (PreGameManager.ObjList[PreGameManager.SpawnObjId].name == this.gameObject.name)
            {
                id = PreGameManager.SpawnObjId; break;
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
