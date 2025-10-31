using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] GameObject potObj;
    [SerializeField] GameObject potFragmentObj;
    [SerializeField] float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //�A�ؔ�������
        Destroy(potObj);


        for (int i = 0; i < potFragmentObj.transform.childCount; i++)
        {//potFragmentObj�̎q�̐��������[�v

            potFragmentObj.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(new Vector2(1000, 200)); //�E���ɔj�Ђ��΂�

            FadeFragment(potFragmentObj.transform.GetChild(i));
        }

        //�A�ؔ��̔j�Ђ𐶐�����
        Instantiate(potFragmentObj, this.transform.position, this.transform.rotation);
    }

    public void FadeFragment(Transform fragment)
    {
        fragment.GetComponent<Renderer>().material.DOFade(0, 6);

        //DestroyFragment(fragment);
    }

    public async void DestroyFragment(Transform fragment)
    {
        await Task.Delay(6000);
        Destroy(fragment.gameObject);
    }
}