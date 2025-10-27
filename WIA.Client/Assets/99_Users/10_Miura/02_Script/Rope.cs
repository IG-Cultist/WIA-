using UnityEngine;
using UnityEngine.UIElements;

public class Rope : MonoBehaviour
{
    [SerializeField] LineRenderer ropeLineRenderer;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var positions = new Vector3[] { startPoint.position, endPoint.position, };
        ropeLineRenderer?.SetPositions(positions);
    }
}
