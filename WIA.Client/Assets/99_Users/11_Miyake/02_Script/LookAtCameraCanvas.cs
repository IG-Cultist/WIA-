using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 常にカメラの方を見るキャンバス
/// </summary>
public class LookAtCameraCanvas : MonoBehaviour
{
    [SerializeField]
    private Transform _camera;

    private void Start()
    {

    }

    private void Update()
    {
        //自機のカメラを探してアタッチする
        _camera = GameObject.Find("Main").transform.Find("First Person Camera").gameObject.transform;

        transform.LookAt(_camera);
    }
}
