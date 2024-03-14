using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendlyHandController : MonoBehaviour
{
    [SerializeField] Transform movePositionTransform;
    [SerializeField] float FloatingSpeed;
    [SerializeField] GameObject GameController;
    [SerializeField] GameObject Bubble;
    [SerializeField] Animator anim;
    [SerializeField] GameObject Player;

    [SerializeField] float MaxHeight;

    NavMeshAgent navMeshAgent;
    Rigidbody RB;

    AudioSource AS;
    [SerializeField] AudioClip[] AC;
    [SerializeField] float pitchdiff;
    [SerializeField] float Volumediff;

    float initialspeed;
    // Start is called before the first frame update
    void Start()
    {
        movePositionTransform = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Enemy"));
        navMeshAgent = GetComponent<NavMeshAgent>();
        RB = GetComponent<Rigidbody>();
        anim = gameObject.transform.GetChild(1).GetComponent<Animator>();
        AS = GetComponent<AudioSource>();

        Player = GameObject.Find("Player");
        GameController = GameObject.Find("Game");
        navMeshAgent.enabled = false;
        anim.speed = navMeshAgent.speed / 3.5f;

        Physics.IgnoreCollision(Player.transform.GetChild(0).GetComponent<Collider>(), GetComponent<Collider>());

        initialspeed = anim.speed;
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
                transform.position += Vector3.up * FloatingSpeed * Time.deltaTime;
            }
            else if(navMeshAgent.enabled && movePositionTransform != null)
            {
                navMeshAgent.destination = movePositionTransform.position;

            }
        }
        if (movePositionTransform == null || movePositionTransform.tag == "BubbledEnemy")
        {
            movePositionTransform = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Enemy"));
            if (movePositionTransform == null)
            {
                anim.speed = 0f;
                navMeshAgent.enabled = false;
            }
            else
            {
                anim.speed = initialspeed;
                navMeshAgent.enabled = true;
            }
        }
        if (transform.position.y > MaxHeight)
        {
            DestroyFriendly();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Terrain")
        {
            navMeshAgent.enabled = true;
            AS.clip = AC[0];
            AS.Play();
            AS.clip = AC[1];
            InvokeRepeating("StepSounds", 0.1f, 1 / 2f);
        }
    }
    void DestroyFriendly()
    {
        Destroy(gameObject);
    }
    public void PowerUpFinished()
    {
        Bubble.SetActive(true);
        AS.clip = AC[2];
        AS.Play();
        RB.useGravity = false;
        Destroy(navMeshAgent);
        Invoke("DestroyFriendly", 10f);
        anim.speed = 0f;
        GetComponent<BoxCollider>().enabled = false;
        CancelInvoke("StepSounds");
    }
    Transform GetClosestEnemy(GameObject[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = gameObject.transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            if (potentialTarget.transform.name != "Stomp")
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                bool Playable = false;
                if (potentialTarget.GetComponent<EnemyHandController>())
                {
                    if (potentialTarget.GetComponent<EnemyHandController>().PlayableArea)
                    {
                        Playable = true;
                    }
                }
                else if (potentialTarget.GetComponent<LitterController>())
                {
                    if (potentialTarget.GetComponent<LitterController>().PlayableArea)
                    {
                        Playable = true;
                    }
                }
                if (dSqrToTarget < closestDistanceSqr && Playable == true)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }
        }
        return bestTarget;
    }
    void StepSounds()
    {
        if (anim.speed > 0)
        {
            AS.pitch = Random.Range(1 - pitchdiff, 1 + pitchdiff);
            AS.volume = Random.Range(0.2f - Volumediff, 0.2f);

            AS.Play();
        }
    }
}
