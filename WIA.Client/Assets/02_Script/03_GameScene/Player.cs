using NIGHTRAVEL.Shared.Interfaces.StreamingHubs;
using UnityEngine;

/// <summary>
/// �v���C���[�����N���X
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] GameObject hitPoint;
    [SerializeField] Animator animator;

    //�v���C���[���
    private enum PLAYER_STATE
    {
        STOP = 0,             //��~��(�����O)
        ALIVE,                //������
        DEATH,                //���S��
        EMOTE,                //�G���[�g��
        ERROR,                //�G���[(�ؒf)
    }

    [SerializeField] PLAYER_STATE player_State;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (player_State)
        {
            //�����O�̏��
            case PLAYER_STATE.STOP:

            break;

            //�������̏ꍇ
            case PLAYER_STATE.ALIVE:

                hitPoint.SetActive(true);
                animator.enabled = true;

                //���̉��ɐ������̏���������




                break;

            //���S���̏ꍇ
            case PLAYER_STATE.DEATH:
                Death();
            break;

            //�G���[�g�Đ����̏ꍇ
            case PLAYER_STATE.EMOTE:
                Emote();
            break;

            //�G���[�̏ꍇ
            case PLAYER_STATE.ERROR:

            break;

        }

    }

    /// <summary>
    /// ���S�֐�
    /// </summary>
    private void Death()
    {
        hitPoint.SetActive(true);
        animator.enabled = false;
    }

    /// <summary>
    /// �G���[�g�֐�
    /// </summary>
    private void Emote(/*�����ɃA�j���[�V�����̌^��錾*/)
    {
        //�����蔻����ꎞ�I�ɍ폜
        hitPoint.SetActive(false);
        
        //�����Ŏ擾�����A�j���[�V����ID�ŃA�j���[�V�����Đ�(�g�ݍ��ނƂ��ɏ��������Ă�)
    }
}
