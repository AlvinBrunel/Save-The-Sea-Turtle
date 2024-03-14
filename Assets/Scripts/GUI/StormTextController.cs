using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormTextController : MonoBehaviour
{
    [SerializeField] float SizeChange;
    [SerializeField] float time;
    AudioSource AS;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(TextAnimation(30));
        AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator TextAnimation(int intervals)
    {
        int Count = 0;
        while (Count <= 5)
        {
            for (int i = 0; i < intervals; i++)
            {
                yield return new WaitForSeconds(time);
                transform.GetComponent<RectTransform>().localScale += (Vector3.one*SizeChange)/intervals;
            }
            AS.Play();
            for (int i = 0; i < intervals; i++)
            {
                yield return new WaitForSeconds(time);
                transform.GetComponent<RectTransform>().localScale -= (Vector3.one * SizeChange)/intervals;
            }
            Count++;
        }
        EndText();
    }
    public void EndText()
    {
        StopCoroutine("TextAnimation");
        GetComponent<StormTextController>().enabled = false;
        this.gameObject.SetActive(false);
    }
}
