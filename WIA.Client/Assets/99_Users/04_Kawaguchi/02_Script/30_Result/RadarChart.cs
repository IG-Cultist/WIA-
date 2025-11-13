using UnityEngine;

public class RadarChart : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    private Vector3[] positions = new Vector3[5];

    /// <summary>
    /// 基準数値
    /// </summary>
    private Vector3 baseCruel = new Vector3(0f, 13.5f, 0f);
    private Vector3 baseJudge = new Vector3(13.3f, 3.7f, 0f);
    private Vector3 baseCarelessly = new Vector3(8.4f, -12f, 0f);
    private Vector3 basePlan = new Vector3(-8.4f, -12f, 0f);
    private Vector3 baseCool = new Vector3(-13.3f, 3.7f, 0f);

    private float point = 0.5f;

    public void SetRadarChart(float[] radarList)
    {
        ///数値変更(減法方式で)
        positions[0] = baseCruel * radarList[0];
        positions[1] = baseJudge * radarList[1];
        positions[2] = baseCarelessly * radarList[2];
        positions[3] = basePlan * radarList[3];
        positions[4] = baseCool * radarList[4];

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);

        lineRenderer.enabled = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //DrawRadar();
    }
}
