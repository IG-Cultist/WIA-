using DG.Tweening;
using UnityEngine;

public class MoveArm : MonoBehaviour
{
    [SerializeField] GameObject crane;
    Transform updateRot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Transform armTransform = crane.transform;

        if (Input.GetKey(KeyCode.R))
        {
            armTransform.Rotate(0, 0.3f, 0); //回転を更新
        }
        if (Input.GetKey(KeyCode.L))
        {
            armTransform.Rotate(0, -0.3f, 0); //回転を更新
        }
    }
}