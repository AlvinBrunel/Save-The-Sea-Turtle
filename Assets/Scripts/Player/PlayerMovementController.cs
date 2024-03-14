using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovementController : MonoBehaviour
{
    public float moveSpeed;
    public float groundDrag;

    public float walkSpeed;


    public float playerHeight;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody RB;
    [SerializeField] GameObject GameController;
    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;

        RB.drag = groundDrag;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 flatVel = new Vector3(RB.velocity.x, 0f, RB.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            RB.velocity = new Vector3(limitedVel.x, RB.velocity.y, limitedVel.z);
        }
        if(transform.position.y < 94f)
        {
            GameController.GetComponent<GameController>().GameOver("You have drowned");
        }
    }

    private void FixedUpdate()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        RB.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }
}