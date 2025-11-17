using DG.Tweening;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] GameObject leverObj;
    [SerializeField] GameObject craneObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateLever();
    }

    public void RotateLever()
    {
        Transform leverTransform=leverObj.transform;
        Transform armTransform = craneObj.transform;

        if (Input.GetKeyDown(KeyCode.L))
        {//Lキーを押したら
            leverTransform.DORotate(new Vector3(-20, armTransform.rotation.y, 0), 3); //レバーが左に傾く
        }
        if(Input.GetKeyDown(KeyCode.R))
        {//Rキーを押したら
            leverTransform.DORotate(new Vector3(20, armTransform.rotation.y, 0), 3); //レバーが右に傾く
        }
    }
}
