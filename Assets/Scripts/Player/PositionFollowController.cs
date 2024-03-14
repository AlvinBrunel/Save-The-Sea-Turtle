using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFollowController : MonoBehaviour
{
    public Transform TargetTransform;
    public Vector3 Offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = TargetTransform.position + Offset;
    }
}
