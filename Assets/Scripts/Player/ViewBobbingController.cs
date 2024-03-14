using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBobbingController : MonoBehaviour
{
    public float EffectIntensity;
    public float EffectIntensityX;
    public float EffectSpeed;

    PositionFollowController FollowerInstance;
    Vector3 OriginalOffset;
    float SinTime;

    // Start is called before the first frame update
    void Start()
    {
        FollowerInstance = GetComponent<PositionFollowController>();
        OriginalOffset = FollowerInstance.Offset;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 InputVector = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));

        if(InputVector.magnitude > 0f)
        {
            SinTime += Time.deltaTime * EffectSpeed;

        }
        else
        {
            SinTime = 0f;
        }

        float SinAmountY = -Mathf.Abs(EffectIntensity * Mathf.Sin(SinTime));
        Vector3 SinAmountX = FollowerInstance.transform.right * EffectIntensity * Mathf.Cos(SinTime) * EffectIntensityX;

        FollowerInstance.Offset = new Vector3(OriginalOffset.x, OriginalOffset.y + SinAmountY, OriginalOffset.z);
        FollowerInstance.Offset += SinAmountX;
    }
}
