using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class GroundCheck : MonoBehaviour
{
    BoxCollider collider;
    [SerializeField] Player player;

    //接地した場合の処理
    public UnityEvent OnEnterGround;
    //地面から離れた場合の処理
    public UnityEvent OnExitGround;
    //接地数
    private int enterNum = 0;
    private Rigidbody rb;
    private int upForce;
    private float distance;

    //OnCollisionStay関数
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject)
        {
            player.isFall = false;
        }
    }

    //OnCollisionExit関数
    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject)
        {
            player.isFall = true;
        }
    }
}
