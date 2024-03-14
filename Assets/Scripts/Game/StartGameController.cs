using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour
{
    bool isReady = false;
    bool isSwitching = false;
    [SerializeField] GameObject Sensitivity;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeAnimationIn(0.01f));
        if (PlayerPrefs.HasKey("Sens"))
        {
            Sensitivity.GetComponent<TextMeshProUGUI>().text =  PlayerPrefs.GetFloat("Sens").ToString();
        }
        else
        {
            PlayerPrefs.SetFloat("Sens", 1f);
            Sensitivity.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetFloat("Sens").ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isReady)
        {
            StartCoroutine(FadeAnimationOut(0.01f));
        }

        if (Input.GetKey(KeyCode.A) && isReady && ((float)Convert.ToDouble(Sensitivity.GetComponent<TextMeshProUGUI>().text) > 0f) && isSwitching == false)
        {
            PlayerPrefs.SetFloat("Sens", (float)Math.Round(PlayerPrefs.GetFloat("Sens") - 0.05f, 2));
            Sensitivity.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetFloat("Sens").ToString();
            isSwitching = true;
            Invoke("EndSwitching", 0.15f);
        }
        if (Input.GetKey(KeyCode.D) && isReady && ((float)Convert.ToDouble(Sensitivity.GetComponent<TextMeshProUGUI>().text) < 5f) && isSwitching == false)
        {
            PlayerPrefs.SetFloat("Sens", (float) Math.Round(PlayerPrefs.GetFloat("Sens") + 0.05f,2));
            Sensitivity.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetFloat("Sens").ToString();
            isSwitching = true;
            Invoke("EndSwitching", 0.15f);
        }


    }
    private IEnumerator FadeAnimationOut(float intervals)
    {
        for (float i = 0; i < 1; i+= intervals)
        {
            transform.GetChild(2).GetComponent<Image>().color = new Color(0,0,0,i);
            yield return new WaitForSecondsRealtime(Time.deltaTime*2);
        }
        SceneManager.LoadScene("MainGame");
    }
    private IEnumerator FadeAnimationIn(float intervals)
    {
        for (float i = 1; i > 0; i -= intervals)
        {
            transform.GetChild(2).GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return new WaitForSecondsRealtime(Time.deltaTime * 2);
        }
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        isReady = true;
    }
    void EndSwitching()
    {
        isSwitching = false;
    }
}
