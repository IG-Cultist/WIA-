using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text guideText;
    [SerializeField] GameObject optionMenu;


    [Header("遷移フェードカラー")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);

    private void Awake()
    {
        /*
        SceneManager.LoadScene("20_OptionScene", LoadSceneMode.Additive);

        GameObject obj = GameObject.Find("OptionCanvas");
        Debug.Log(obj);*/
        //obj.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartMenu()
    {
        guideText.gameObject.SetActive(false);

        // シーン遷移
        Initiate.DoneFading();
        Initiate.Fade("02_MenuScene", endColor, 1.0f);
    }

    public void OnClickOptionButton()
    {

    }
}
