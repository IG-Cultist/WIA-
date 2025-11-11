using UnityEngine;

public class RadarChart : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    private Vector3[] positions = new Vector3[5];

    /// <summary>
    /// 基準数値
    /// </summary>
    private Vector3 baseCruel = new Vector3(0f,13.5f,0f);   
    private Vector3 baseJudge = new Vector3(13.3f, 3.7f, 0f);
    private Vector3 baseCarelessly = new Vector3(8.4f, -12f, 0f);
    private Vector3 basePlan = new Vector3(-8.4f, -12f, 0f); 
    private Vector3 baseCool = new Vector3(-13.3f, 3.7f, 0f);

    private float point = 0.5f;

    public void DrawRadar()
    {
        ///数値変更(減法方式で)
        positions[0] = baseCruel * point;
        positions[1] = baseJudge * point;
        positions[2] = baseCarelessly * point;
        positions[3] = basePlan * point;
        positions[4] = baseCool * point;

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);

        lineRenderer.enabled = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DrawRadar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
