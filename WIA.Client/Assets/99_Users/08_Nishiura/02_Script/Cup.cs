/// ------------------------------
/// コップマネージャー
/// Author:Nishiura Date:25/12/01
/// ------------------------------
using UnityEngine;

public class Cup : MonoBehaviour
{
    [SerializeField] GameObject waterPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground" || collision.transform.tag == "Trap")
        {
            Instantiate(waterPrefab, new Vector3(this.gameObject.transform.position.x, -0.47f, this.gameObject.transform.position.z), waterPrefab.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
