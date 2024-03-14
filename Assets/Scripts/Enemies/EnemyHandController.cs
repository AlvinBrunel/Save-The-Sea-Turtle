using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHandController : MonoBehaviour
{
    [SerializeField] Transform movePositionTransform;
    [SerializeField] float FloatingSpeed;
    [SerializeField] GameObject GameController;
    [SerializeField] GameObject Bubble;
    [SerializeField] Animator anim;
    [SerializeField] bool TargettingPlayer;

    public bool PlayableArea = false;
    [SerializeField] int Health;

    [SerializeField] float Speed;
    float MaxSpeed;

    [SerializeField] float MaxHeight;

    bool HandAttackable = true;
    bool CanAttack = true;
    bool CanBreakOut;

    NavMeshAgent navMeshAgent;
    Rigidbody RB;

    AudioSource AS;
    [SerializeField] AudioClip[] AC;
    [SerializeField] float pitchdiff;
    [SerializeField] float Volumediff;

    // Start is called before the first frame update
    void Start()
    {
        if (TargettingPlayer == true)
        {
            movePositionTransform = GameObject.Find("Player").transform;
        }
        else
        {
            movePositionTransform = GetClosest(GameObject.FindGameObjectsWithTag("Animal"));
            if(movePositionTransform == null)
            {
                movePositionTransform = GameObject.Find("Player").transform;
            }
        }
        InvokeRepeating("ChooseTarget", 0.25f, 0.25f);
        InvokeRepeating("StepSounds", 0.1f, (1/2f) *(Speed/6f));

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
            CancelInvoke("StepSounds");
            navMeshAgent.enabled = false;
        }
        else
        {
            if (Bubble.activeSelf)
            {
                if (Health <= 0)
                {
                    transform.position += Vector3.up * FloatingSpeed * Time.deltaTime;
                }
                if (IsInvoking("StepSounds"))
                {
                    AS.clip = AC[1];
                    AS.Play();
                    CancelInvoke("StepSounds");
                }

            }
            else
            {
                navMeshAgent.enabled = true;
                if(movePositionTransform == null)
                {
                    movePositionTransform = GetClosest(GameObject.FindGameObjectsWithTag("Animal"));
                }
                navMeshAgent.destination = movePositionTransform.position;
                anim.speed = Speed/MaxSpeed;
                if (!IsInvoking("StepSounds"))
                {
                    InvokeRepeating("StepSounds", 0.1f, (1 / 2f) * (Speed / 6f));
                }
            }
        }
        if(transform.position.y > 140f)
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
            Bubble.SetActive(true);
            AS.clip = AC[1];
            AS.Play();
            CancelInvoke("StepSounds");
            navMeshAgent.enabled = false;
            anim.speed = 0f;
            GetComponent<BoxCollider>().enabled = false;
            transform.tag = "BubbledEnemy";

            if (Health <= 0)
            {
                Destroy(navMeshAgent);
                GameController.GetComponent<GameController>().EnemiesLeft--;
                GameController.GetComponent<GameController>().EnemiesExpelled++;
                if (GameController.GetComponent<GameController>().EnemiesLeft <= 0)
                {
                    GameController.GetComponent<GameController>().ChangeRound();
                }
                RB.useGravity = false;
                if (IsInvoking("BreakFree"))
                {
                    CancelInvoke("BreakFree");
                }
                Invoke("DestroyEnemy", 10f);
            }
            else
            {
                Invoke("BreakFree", 5f);
            }
        }
        if (collision.transform.name == "Player")
        {
            GameController.GetComponent<GameController>().GameOver("You have been killed");
        }
        if(collision.transform.tag == "Terrain")
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.destination = transform.position;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.name == "Friendly Hand" && HandAttackable == true && Bubble.activeSelf == false && Speed > 0f)
        {
            HandAttackable = false;
            Speed -= 0.5f;
            navMeshAgent.speed = Speed;
            anim.speed = Speed / MaxSpeed;
            Invoke("ResetAttack", 0.5f);

            if (Speed == 0f)
            {
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
            collision.transform.gameObject.GetComponent<TurtleController>().Health--;
            if(collision.transform.gameObject.GetComponent<TurtleController>().Health <= 0)
            {
                GameController.GetComponent<GameController>().GameOver("A Turtle has died");
            }
            else
            {
                if (collision.gameObject.GetComponent<TurtleController>().GUIAlert.transform.parent.GetChild(0).gameObject.activeSelf)
                {
                    collision.gameObject.GetComponent<TurtleController>().GUIAlert.transform.parent.GetChild(0).gameObject.SetActive(false);
                    collision.gameObject.GetComponent<TurtleController>().GUIAlert.SetActive(true);
                    collision.gameObject.GetComponent<TurtleController>().Invoke("GUIAlertOff", 5f);
                }
            }
            Invoke("ResetCanAttack", 1f);
        }

    }
    void BreakFree()
    {
        Bubble.SetActive(false);
        AS.clip = AC[2];
        AS.Play();
        AS.clip = AC[0];
        InvokeRepeating("StepSounds", 0.1f, (1 / 2f) * (Speed / 6f));
        RB.useGravity = true;
        navMeshAgent.enabled = true;
        Invoke("DestroyEnemy", 10f);
        anim.speed = 0f;
        GetComponent<BoxCollider>().enabled = true;
        transform.tag = "Enemy";
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
        if(PlayableArea == true)
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
        if(other.transform.name == "Unplayable Area")
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
    void ChooseTarget()
    {
        if (TargettingPlayer)
        {
            if (Vector3.Distance(GameObject.Find("Player").transform.position, transform.position) < 15f)
            {
                movePositionTransform = GameObject.Find("Player").transform;
            }
            else
            {
                movePositionTransform = GetClosest(GameObject.FindGameObjectsWithTag("Animal"));
            }
        }
        else
        {
            if (GetClosest(GameObject.FindGameObjectsWithTag("Animal")) == null)
            {
                movePositionTransform = GameObject.Find("Player").transform;
            }
        }
    }
    void StepSounds()
    {
        if(anim.speed > 0)
        {
            AS.pitch = Random.Range(1 - pitchdiff, 1 + pitchdiff);
            AS.volume = Random.Range(0.25f - Volumediff, 0.25f);

            AS.Play();
        }
    }
}
