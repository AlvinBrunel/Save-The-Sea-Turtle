using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBulletController : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] float Duration;
    [SerializeField] GameObject Particle;
    Vector3 InitialPos;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyBullet",Duration);
        InitialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        if(Vector3.Distance(InitialPos,transform.position) > 150f)
        {
            DestroyBubble();
        }
    }
    void DestroyBubble()
    {
        Instantiate(Particle, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Terrain")
        {
            DestroyBubble();
        }
    }
}
