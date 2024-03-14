using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurtleController : MonoBehaviour
{
    [SerializeField] Vector3 movePositionTransform;
    [SerializeField] GameObject GameController;
    [SerializeField] bool Moving = true;

    [SerializeField] Motion AnimationMoving;
    public int Health;
    [SerializeField] float Speed;
    NavMeshAgent navMeshAgent;

    public GameObject GUIAlert;

    [SerializeField] Animator anim;
    private void Awake()
    {
        Invoke("IntervalMovement", 30/24f);
    }
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        GameController = GameObject.Find("Game");
        movePositionTransform = new Vector3(GameController.transform.position.x, transform.position.y, transform.position.z);
        navMeshAgent.speed = Speed;

        anim = transform.GetChild(0).GetComponent<Animator>();
        Physics.IgnoreCollision(GameObject.Find("Player").transform.GetChild(0).GetComponent<Collider>(), GetComponent<Collider>());

        foreach (GameObject Turtle in GameObject.FindGameObjectsWithTag("Animal"))
        {
            Physics.IgnoreCollision(Turtle.GetComponent<Collider>(), GetComponent<Collider>());
        }
            anim.Play("Turtle|Moving");

        Speed = (transform.localScale.x / 0.6f);
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
            anim.speed = 1;
            navMeshAgent.enabled = true;
            if (Moving == true)
            {
                navMeshAgent.destination = movePositionTransform;
            }
        }
        if(Vector3.Distance(transform.position,new Vector3(movePositionTransform.x,transform.position.y,movePositionTransform.z)) < 2f)
        {
            if(GameObject.FindGameObjectsWithTag("Animal").Length == 1)
            {
                GameController.GetComponent<GameController>().SpawnTurtle();
            }
            TurtleSaved();
        }
    }
    void IntervalMovement()
    {
        if(Moving == false)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Turtle|Moving"))
            {
                anim.Play("Turtle|Moving 0");
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Turtle|Moving 0"))
            {
                anim.Play("Turtle|Moving");
            }

            Moving = true;
            Invoke("IntervalMovement", 30/24f);
            navMeshAgent.speed = Speed;
        }
        else if (Moving == true)
        {
            Moving = false;
            Invoke("IntervalMovement", 40/24f);
            navMeshAgent.speed = 0;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Terrain")
        {
            navMeshAgent.enabled = true;
        }
    }
    void TurtleSaved()
    {
        Destroy(gameObject);
        GameController.GetComponent<GameController>().TurtlesSaved++;
    }
    public void GUIAlertOff()
    {
        GUIAlert.SetActive(false);
    }
}
