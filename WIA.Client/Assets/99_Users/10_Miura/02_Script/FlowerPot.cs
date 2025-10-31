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
        GameObject fragment;
        fragment = Instantiate(potFragmentObj, potObj.transform.position, potObj.transform.rotation);

        //êAñÿî´Çè¡Ç∑
        Destroy(potObj);

        for (int i = 0; i < fragment.transform.childCount; i++)
        {//potFragmentObjÇÃéqÇÃêîÇæÇØÉãÅ[Év
         //êAñÿî´ÇÃîjï–Çê∂ê¨Ç∑ÇÈ
            fragment.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(new Vector2(50, 50));

            FadeFragment(fragment.transform.GetChild(i));
        }
    }

    public void FadeFragment(Transform fragment)
    {
        fragment.GetComponent<Renderer>().material.DOFade(0, 6);

        DestroyFragment(fragment);
    }

    public async void DestroyFragment(Transform fragment)
    {
        await Task.Delay(6000);
        Destroy(fragment.gameObject);
    }
}