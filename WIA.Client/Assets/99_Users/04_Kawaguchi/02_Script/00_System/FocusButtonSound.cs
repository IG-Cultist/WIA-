using UnityEngine;
using KanKikuchi.AudioManager;


public class FocusButtonSound : MonoBehaviour
{
    void FocusButton()
    {
        SEManager.Instance.Play(
            audioPath: SEPath.FOCUS_BUTTON, //再生したいオーディオのパス
             volumeRate: 1,                 //音量の倍率
             delay: 0,                      //再生されるまでの遅延時間
             pitch: 1,                      //ピッチ
             isLoop: false,                 //ループ再生するか
             callback: null                 //再生終了後の処理
        );
    }

    void PushButton()
    {
        SEManager.Instance.Play(
            audioPath: SEPath.PUSH_BUTTON,   //再生したいオーディオのパス
             volumeRate: 1,                 //音量の倍率
             delay: 0,                      //再生されるまでの遅延時間
             pitch: 1,                      //ピッチ
             isLoop: false,                 //ループ再生するか
             callback: null                 //再生終了後の処理
        );
    }
}
