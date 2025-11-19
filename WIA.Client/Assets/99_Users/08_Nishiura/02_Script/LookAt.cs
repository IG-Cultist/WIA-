using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{

    void Update()
    {
        Vector3 p = GameObject.Find("First Person Camera").GetComponent<Camera>().transform.position;
        p.y = transform.position.y;
        transform.LookAt(p);
    }
}