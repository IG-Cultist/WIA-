//================================
// アニメーションの管理をするスクリプト
// Author:y-miura
// Date:2026/01/14
//================================

using UnityEngine;

public class AnimationController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// アニメーション再生が終わったら
    /// </summary>
    public void OnFinishAnimation()
    {
        Destroy(this.gameObject);
    }
}
