using NIGHTRAVEL.Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�����N���X
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] GameObject hitPoint;    //Player�̓����蔻��
    [SerializeField] Animator animator;

    //
    [SerializeField] Rigidbody hip;
    [SerializeField] Rigidbody leftLeg;

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
    List<Transform> allChildren;

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

                ChangeBodyGravity(false);
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
                Emote(1);
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
        ChangeBodyGravity(true);
          
        hitPoint.SetActive(true);
        animator.enabled = false;
    }

    /// <summary>
    /// �G���[�g�֐�
    /// </summary>
    private void Emote(int animationID)
    {
        ChangeBodyGravity(false);
        //�����蔻����ꎞ�I�ɍ폜
        hitPoint.SetActive(false);
        
        //�����Ŏ擾�����A�j���[�V����ID�ŃA�j���[�V�����Đ�(�g�ݍ��ނƂ��ɏ��������Ă�)
    }

    /// <summary>
    /// �d�͐؂�ւ��֐�(�R�������Ɖ����x���E�˔j����)
    /// </summary>
    /// <param name="isChange"></param>
    private void ChangeBodyGravity(bool isChange)
    {

        // true��n���Ɣ�A�N�e�B�u�ȃI�u�W�F�N�g�����ׂĎ擾���܂�
        allChildren = GetAllChildTransforms(this.gameObject.transform, true);

        // �擾�����I�u�W�F�N�g�̃��X�g��\�������
        foreach (Transform child in allChildren)
        {
            // �q���I�u�W�F�N�g���g���܂܂�邽�߁A���g�����O����ꍇ��if���Ŕ��肵�܂�
            if (child != this.transform)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();

                //Rigidbody���t���Ă���I�u�W�F�N�g�ɑ΂��ďd�͐ؑ�
                if (rb != null)
                {
                    switch (isChange)
                    {
                        case true:
                            rb.useGravity = true;       //�d�͂�ON�ɂ���
                            Debug.Log("ON");
                            break;

                        case false:
                            rb.useGravity = false;      //�d�͂�OFF�ɂ���
                            Debug.Log("OFF");
                            break;
                    }

                    //Debug.Log("�q���I�u�W�F�N�g��: " + child.gameObject.name);
                }
            }
        }

        
    }


    public static List<Transform> GetAllChildTransforms(Transform parent, bool includeInactive = true)
    {
        var results = new List<Transform>();
        if (parent == null) return results;

        // �X�^�b�N���g���������i�[���D��j
        var stack = new Stack<Transform>();
        for (int i = 0; i < parent.childCount; i++)
            stack.Push(parent.GetChild(i));

        while (stack.Count > 0)
        {
            var t = stack.Pop();

            if (includeInactive || t.gameObject.activeInHierarchy)
            {
                results.Add(t);

                // �q���X�^�b�N�ɒǉ��i���ȉ����܂߂�j
                for (int i = 0; i < t.childCount; i++)
                    stack.Push(t.GetChild(i));
            }
        }

        return results;
    }
    public List<GameObject> GetAllChildGameObjects(Transform parent, bool includeInactive = true)
    {
        var transforms = GetAllChildTransforms(parent, includeInactive);
        var gos = new List<GameObject>(transforms.Count);
        foreach (var t in transforms) gos.Add(t.gameObject);
        return gos;
    }
}
