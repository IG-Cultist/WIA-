using UnityEngine;

public class MenuManager : MonoBehaviour
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

    public void StartGame()
    {
      
        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("03_GameScene", endColor, 1.0f);
    }
}
