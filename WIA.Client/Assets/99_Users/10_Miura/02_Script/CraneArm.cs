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

        // ���[���h���W����ɁA��]���擾
        Vector3 worldAngle = armTransform.eulerAngles;
        float world_angle_x = worldAngle.x; // ���[���h���W����ɂ����Ax�������ɂ�����]�p�x
        float world_angle_y = worldAngle.y; // ���[���h���W����ɂ����Ay�������ɂ�����]�p�x
        float world_angle_z = worldAngle.z; // ���[���h���W����ɂ����Az�������ɂ�����]�p�x

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

        }
    }
}
