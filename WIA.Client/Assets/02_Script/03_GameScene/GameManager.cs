using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartResult()
    {
        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("04_ResultScene", endColor, 1.0f);
    }
}
