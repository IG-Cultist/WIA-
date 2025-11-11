using UnityEngine;

public class LineTest : MonoBehaviour
{
    public LineRenderer line;

    void Start()
    {
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * 5f);
    }
}
