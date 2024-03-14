using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public GameObject Camera;
    KeyCode ThrowKey = KeyCode.G;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0 && Input.GetKeyDown(ThrowKey))
        {
            transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
            transform.GetChild(0).gameObject.GetComponent<Collider>().enabled = true;
            transform.GetChild(0).gameObject.transform.tag = "MovableObject";
            transform.GetChild(0).transform.parent = null;
        }
    }
}
