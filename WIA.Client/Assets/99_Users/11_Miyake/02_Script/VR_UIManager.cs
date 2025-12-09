//==============================================
//VRのUI表示
//2025/12/8：三宅歩人
//==============================================
using UnityEngine;
using UnityEngine.UI;

public class VR_UIManager : MonoBehaviour
{
    // タスク説明文
    [SerializeField] Text taskExplanation;
    // タスク進捗度
    [SerializeField] Text taskCount;
    // タスクアイコン
    [SerializeField] Image taskIcon;
    // 死亡回数
    [SerializeField] Text deathCount;
    // 経過時間テキスト
    [SerializeField] Text timerText;
    // 制限時間テキスト
    [SerializeField] Text limitTimerText;
    // 変動数テキスト
    [SerializeField] Text fluctTimeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
