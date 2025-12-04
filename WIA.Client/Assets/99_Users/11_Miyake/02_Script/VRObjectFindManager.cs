//=============================================
//VRでオブジェクトを探す判定
//三宅歩人:2025/12/4
//=============================================
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRObjectFindManager : MonoBehaviour
{
    //レイ取得
    [SerializeField] NearFarInteractor nearFarInteractor_L;         //左手
    [SerializeField] NearFarInteractor nearFarInteractor_R;         //右手

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //レイが調査課のオブジェクトに当たったら調査できるようにする

}
