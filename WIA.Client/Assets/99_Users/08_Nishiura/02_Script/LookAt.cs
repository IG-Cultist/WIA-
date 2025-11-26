using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
     GameObject p;
    private void Start()
    {
        p = GameObject.Find("First Person Camera").gameObject;
    }

    void Update()
    {
        if (p.gameObject == true)
        {

            Vector3 pos = p.transform.position;
            pos.y = transform.position.y;
            transform.LookAt(pos);
        }
    }
}