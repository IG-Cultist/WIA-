using DG.Tweening;
using UnityEngine;

public class MoveArm : MonoBehaviour
{
    [SerializeField] GameObject crane;
    //[SerializeField] GameObject hingePoint;
    //[SerializeField] GameObject hook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Transform armTransform = crane.transform;
        //Transform hingeTransform = hingePoint.transform;

        if (Input.GetKey(KeyCode.D))
        {
            armTransform.Rotate(0, 0.3f, 0); //回転を更新
            //hingeTransform.Rotate(0, 3f, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            armTransform.Rotate(0, -0.3f, 0); //回転を更新
            //hingeTransform.Rotate(0, -3f, 0);
        }
    }
}