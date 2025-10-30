using UnityEngine;

public class FlowerPotManager : MonoBehaviour
{
    [SerializeField] GameObject potObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            GeneratePot();
        }
    }

    private void GeneratePot()
    {
        Vector3 genetratePos = new Vector3(0, 5.5f, 0);
        Instantiate(potObj, genetratePos, Quaternion.identity);
    }
}
