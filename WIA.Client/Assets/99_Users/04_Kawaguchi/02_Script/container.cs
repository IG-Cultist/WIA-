using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class container : MonoBehaviour
{
    //[SerializeField] ParticleSystem m_particle;
    [SerializeField] float m_force = 120;
    [SerializeField] float m_radius = 50;
    [SerializeField] float m_upwards = 0;
    Vector3 m_position;

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("PlayerÇ…Ç‘Ç¬Ç©Ç¡ÇΩÅI");
            Explosion();
        }
    }

    public void Explosion()
    {
        /*m_particle.Play();
        m_position = m_particle.transform.position;*/

        // îÕàÕì‡ÇÃRigidbodyÇ…AddExplosionForce
        Collider[] hitColliders = Physics.OverlapSphere(m_position, m_radius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            var rb = hitColliders[i].GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddExplosionForce(m_force, m_position, m_radius, m_upwards, ForceMode.Impulse);
            }
        }
    }
}
