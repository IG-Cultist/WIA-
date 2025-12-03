using DG.Tweening;
using UnityEngine;

public class MoveArm : MonoBehaviour
{
    [SerializeField] GameObject crane;

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
            armTransform.DORotate(new Vector3(0, 45, 0), 40);
        }
        if (Input.GetKey(KeyCode.L))
        {
            armTransform.DORotate(new Vector3(0, -45, 0), 40);
        }
    }
}