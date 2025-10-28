using UnityEngine;
using KanKikuchi.AudioManager;


public class FocusButtonSound : MonoBehaviour
{
    void FocusButton()
    {
        SEManager.Instance.Play(
            audioPath: SEPath.FOCUS_BUTTON, //�Đ��������I�[�f�B�I�̃p�X
             volumeRate: 1,                 //���ʂ̔{��
             delay: 0,                      //�Đ������܂ł̒x������
             pitch: 1,                      //�s�b�`
             isLoop: false,                 //���[�v�Đ����邩
             callback: null                 //�Đ��I����̏���
        );
    }

    void PushButton()
    {
        SEManager.Instance.Play(
            audioPath: SEPath.PUSH_BUTTON,   //�Đ��������I�[�f�B�I�̃p�X
             volumeRate: 1,                 //���ʂ̔{��
             delay: 0,                      //�Đ������܂ł̒x������
             pitch: 1,                      //�s�b�`
             isLoop: false,                 //���[�v�Đ����邩
             callback: null                 //�Đ��I����̏���
        );
    }
}
