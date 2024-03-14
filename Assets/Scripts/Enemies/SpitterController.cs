using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;

public class SpitterController : MonoBehaviour
{
    [SerializeField] Transform movePositionTransform;
    [SerializeField] float FloatingSpeed;
    [SerializeField] GameObject GameController;
    [SerializeField] GameObject Bubble;
    [SerializeField] Animator anim;

    public bool PlayableArea = false;
    [SerializeField] int Health;

    [SerializeField] float Speed;
    float MaxSpeed;

    [SerializeField] float MaxHeight;

    bool HandAttackable = true;
    bool CanAttack = true;

    NavMeshAgent navMeshAgent;
    Rigidbody RB;

    AudioSource AS;
    [SerializeField] AudioClip[] AC;
    [SerializeField] float pitchdiff;
    [SerializeField] float Volumediff;

    [SerializeField] float MinShakeStrength;
    [SerializeField] float MaxShakeStrength;

    // Start is called before the first frame update
    void Start()
    {
        movePositionTransform = GameObject.Find("Nest").transform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        RB = GetComponent<Rigidbody>();
        anim = gameObject.transform.GetChild(1).GetComponent<Animator>();
        AS = GetComponent<AudioSource>();
        GameController = GameObject.Find("Game");

        navMeshAgent.speed = Speed;
        MaxSpeed = Speed;

        Physics.IgnoreCollision(transform.GetChild(0).GetComponent<Collider>(), GetComponent<Collider>());
        Physics.IgnoreCollision(GameObject.Find("Barrier").GetComponent<Collider>(), GetComponent<Collider>());
    }
    // Update is called once per frame
    void Update()
    {
        if (GameController.GetComponent<GameController>().inCutscene == true)
        {
            anim.speed = 0;
            navMeshAgent.enabled = false;
        }
        else
        {
            if (Bubble.activeSelf)
            {
                if (Health <= 0)
                {
                    transform.position += Vector3.up * FloatingSpeed * Time.deltaTime;
                    anim.speed = 0f;
                }

            }
            else
            {
                navMeshAgent.enabled = true;
                navMeshAgent.destination = movePositionTransform.position;
                anim.speed = Speed / MaxSpeed;
                if(Mathf.Round(anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 100) == 23 || Mathf.Round(anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 100) == 73)
                {
                    float DistancetoPlayer = Vector3.Distance(GameObject.Find("Player").transform.position, transform.position);
                    float Shakestrength =  50 * (1 / DistancetoPlayer);
                    if (Shakestrength < MinShakeStrength)
                    {
                        ///
                    }
                    else if (Shakestrength > MaxShakeStrength)
                    {
                        CameraShaker.Instance.ShakeOnce(7f, 10, 0, 1.5f);
                        AS.Play();
                    }
                    else
                    {
                        CameraShaker.Instance.ShakeOnce(50 * (1 / DistancetoPlayer), 10, 0, 1.5f);
                        AS.Play();
                    }

                }
            }
        }
        if (transform.position.y > 140f)
        {
            DestroyEnemy();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Projectile")
        {
            Destroy(collision.gameObject);
            Health--;
            if (Health <= 0)
            {
                Destroy(GetComponent<NavMeshAgent>());
                Bubble.SetActive(true);
                AS.clip = AC[1];
                AS.Play();
                RB.useGravity = false;
                navMeshAgent.enabled = false;
                Invoke("DestroyEnemy", 10f);
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
        if (collision.transform.name == "Nest")
        {
            GameController.GetComponent<GameController>().GameOver("A turtle egg has been eaten");
        }
        if (collision.transform.tag == "Terrain")
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.name == "Friendly Hand" && HandAttackable == true && Bubble.activeSelf == false && Speed > 0f)
        {
            HandAttackable = false;
            Health -= 1;
            navMeshAgent.speed = Speed;
            anim.speed = Speed / MaxSpeed;
            Invoke("ResetAttack", 0.5f);

            if (Speed == 0f)
            {
                Destroy(GetComponent<NavMeshAgent>());
                Bubble.SetActive(true);
                AS.clip = AC[1];
                AS.Play();
                RB.useGravity = false;
                Destroy(navMeshAgent);
                Invoke("DestroyEnemy", 10f);
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
        if (collision.transform.name == "Turtle" && CanAttack == true && Bubble.activeSelf == false && Speed > 0f)
        {
            CanAttack = false;
            collision.transform.gameObject.GetComponent<TurtleController>().Health=0;
            if (collision.transform.gameObject.GetComponent<TurtleController>().Health <= 0)
            {
                GameController.GetComponent<GameController>().GameOver("A Turtle has died");
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
    void DestroyEnemy()
    {
        Transform[] AllTrees = GameObject.Find("Groups Of Trees").GetComponentsInChildren<Transform>();
        List<GameObject> Trees = new List<GameObject>();
        foreach (Transform Tree in AllTrees)
        {
            Trees.Add(Tree.gameObject);
        }


        if (PlayableArea == true)
        {
            int RandNum = Random.Range(1, 100);

            if (RandNum <= 5)
            {
                GameObject InsPowerUp = Instantiate(GameController.GetComponent<GameController>().PowerUpList[0], transform.position, Quaternion.Euler(0, 0, 0));
            }
            else if (RandNum <= 10)
            {
                GameObject InsPowerUp = Instantiate(GameController.GetComponent<GameController>().PowerUpList[1], transform.position, Quaternion.Euler(0, 0, 0));
            }
            else if (RandNum <= 15)
            {
                GameObject InsPowerUp = Instantiate(GameController.GetComponent<GameController>().PowerUpList[2], transform.position, Quaternion.Euler(0, 0, 0));
            }
        }
        Destroy(gameObject);
    }
    Transform GetClosest(GameObject[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = gameObject.transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
        return bestTarget;
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
}