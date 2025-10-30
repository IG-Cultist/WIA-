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

        //A–Ø”«‚Ì”j•Ğ‚ğ¶¬‚·‚é
        Instantiate(potFragmentObj, this.transform.position, this.transform.rotation);
    }
}