using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitterController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject GameController;
    GameObject Bubble;
    Rigidbody RB;
    AudioSource AS;
    [SerializeField] AudioClip[] AC;

    [SerializeField] float FloatingSpeed;
    [SerializeField] int Health;
    [SerializeField] bool HandAttackable = true;
    [SerializeField] bool CanAttack = false;
    bool Leaked = false;
    [SerializeField] GameObject SeaController;
    [SerializeField] Vector3 Vel;

    float initialspeed;
    float initialhealth;
    float speed;
    public bool PlayableArea = false;
    void Start()
    {
        GameController = GameObject.Find("Game");
        SeaController = GameObject.Find("Sea");
        RB = GetComponent<Rigidbody>();
        AS = GetComponent<AudioSource>();
        List<GameObject> Trees = new List<GameObject>();
        for(int i = 0;i< GameObject.Find("Groups Of Trees").transform.childCount;i++)
        {
            Physics.IgnoreCollision(GameObject.Find("Groups Of Trees").transform.GetChild(i).GetComponent<Collider>(), GetComponent<Collider>());
        }

        Physics.IgnoreCollision(GameObject.Find("Barrier").GetComponent<Collider>(), GetComponent<Collider>());

        Bubble = transform.GetChild(0).gameObject;

        initialspeed = 1;
        speed = initialspeed;

        initialhealth = Health;
        Vel = RB.velocity;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Bubble.activeSelf && RB.constraints != RigidbodyConstraints.None)
        {
            transform.GetChild(1).transform.rotation *= (Quaternion.Euler((RB.velocity.z)/2, 0, (RB.velocity.x)));
        }
        if(transform.position.z < -35f && transform.position.y < 100.75f && Leaked == false)
        {
            Leaked = true;
            SeaController.GetComponent<SeaController>().SeaPollution();
            Destroy(gameObject);
        }
        if(GameController.GetComponent<GameController>().StormStrength == 0f && transform.position.z >= 37f)
        {
            Destroy(gameObject);
        }
        if (GameController.GetComponent<GameController>().StormStrength == 0f && RB.constraints != RigidbodyConstraints.None)
        {
            RB.constraints = RigidbodyConstraints.None;
        }
        //if()
        {

        }

    }
    void FixedUpdate()
    {
        if (!Bubble.activeSelf)
        {
            RB.AddForce(Vector3.back * speed * GameController.GetComponent<GameController>().StormStrength);
            Vel = Vector3.back * speed * GameController.GetComponent<GameController>().StormStrength;
        }
        else
        {
            transform.position += Vector3.up * FloatingSpeed * Time.deltaTime;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Projectile")
        {
            Destroy(collision.gameObject);
            Bubble.SetActive(true);
            AS.Play();
            RB.useGravity = false;
            Invoke("DestroyLitter", 10f);
            GetComponent<BoxCollider>().enabled = false;
            transform.tag = "BubbledEnemy";

            GameController.GetComponent<GameController>().EnemiesLeft--;
            GameController.GetComponent<GameController>().EnemiesExpelled++;
            if (GameController.GetComponent<GameController>().EnemiesLeft <= 0)
            {
                GameController.GetComponent<GameController>().ChangeRound();
            }
        }
    }
    void DestroyLitter()
    {
        AS.clip = AC[1];
        AS.Play();
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Unplayable Area")
        {
            PlayableArea = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Unplayable Area")
        {
            PlayableArea = true;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.name == "Friendly Hand" && HandAttackable == true && Bubble.activeSelf == false)
        {
            HandAttackable = false;
            speed -= 0.25f;
            Invoke("ResetAttack", 0.5f);

            if (speed <= 0)
            {
                Bubble.SetActive(true);
                AS.clip = AC[0];
                AS.Play();
                RB.useGravity = false;
                Invoke("DestroyLitter", 10f);
                GetComponent<BoxCollider>().enabled = false;
                transform.tag = "BubbledEnemy";

                GameController.GetComponent<GameController>().EnemiesLeft--;
                GameController.GetComponent<GameController>().EnemiesExpelled++;
                if (GameController.GetComponent<GameController>().EnemiesLeft <= 0)
                {
                    GameController.GetComponent<GameController>().ChangeRound();
                }
            }

        }
        if (collision.transform.name == "Turtle" && CanAttack == true && Bubble.activeSelf == false)
        {
            CanAttack = false;
            collision.transform.gameObject.GetComponent<TurtleController>().Health--;
            speed = initialspeed * (Health / initialhealth);
            if (collision.transform.gameObject.GetComponent<TurtleController>().Health <= 0)
            {
                GameController.GetComponent<GameController>().GameOver("A turtle has died");
            }
            Invoke("ResetCanAttack", 1f);
        }

    }
    void ResetAttack()
    {
        HandAttackable = true;
    }
    void ResetCanAttack()
    {
        CanAttack = true;
    }
}
