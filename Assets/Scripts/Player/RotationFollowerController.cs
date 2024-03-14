using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFollowerController : MonoBehaviour
{
    public Transform Target;
    [SerializeField] float RotationLerp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, RotationLerp*Time.deltaTime);
        //transform.rotation = Target.rotation;
    }
}
