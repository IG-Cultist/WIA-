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
        //A–Ø”«‚ğÁ‚·
        Destroy(potObj);


        for (int i = 0; i < potFragmentObj.transform.childCount; i++)
        {//potFragmentObj‚Ìq‚Ì”‚¾‚¯ƒ‹[ƒv

            potFragmentObj.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(new Vector2(1000, 200)); //‰E‘¤‚É”j•Ğ‚ğ”ò‚Î‚·

            FadeFragment(potFragmentObj.transform.GetChild(i));
        }

        //A–Ø”«‚Ì”j•Ğ‚ğ¶¬‚·‚é
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