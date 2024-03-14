using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TMP;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public IEnumerator FadeAnimation(float intervals)
    {
        while (true)
        {
            for (float i = 0; i <=1; i+=intervals)
            {

                yield return new WaitForSecondsRealtime(0.018f);
                TMP.alpha = i;
            }
            yield return new WaitForSecondsRealtime(3f);
            for (float i = 1; i > 0; i -= intervals)
            {

                yield return new WaitForSecondsRealtime(0.018f);
                TMP.alpha = i;
            }
            gameObject.SetActive(false);
        }
    }
}
