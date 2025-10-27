using UnityEngine;
using UnityEngine.UIElements;

public class CraneArm : MonoBehaviour
{
    [SerializeField] GameObject ropeObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform armTransform = this.transform;

        // ワールド座標を基準に、回転を取得
        Vector3 worldAngle = armTransform.eulerAngles;
        float world_angle_x = worldAngle.x; // ワールド座標を基準にした、x軸を軸にした回転角度
        float world_angle_y = worldAngle.y; // ワールド座標を基準にした、y軸を軸にした回転角度
        float world_angle_z = worldAngle.z; // ワールド座標を基準にした、z軸を軸にした回転角度

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

        }
    }
}
