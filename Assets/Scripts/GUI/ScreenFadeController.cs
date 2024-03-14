using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFadeController : MonoBehaviour
{
    [SerializeField] bool isStartGame;

    [SerializeField] GameObject GameController;
    [SerializeField] GameObject DialogueBox;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeAnimation(0.01f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator FadeAnimation(float intervals)
    {
        for (float i = 1; i > 0; i -= intervals)
        {
            GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return new WaitForSecondsRealtime(Time.deltaTime * 2);
        }
        if (isStartGame)
        {
            DialogueBox.SetActive(true);
            GameController.GetComponent<GameController>().ChangeCutScene(GameController.GetComponent<GameController>().CurrentScene);
        }
    }
    public IEnumerator FadeAnimationOut(float intervals)
    {
        for (float i = 0; i < 1; i += intervals)
        {
            GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return new WaitForSecondsRealtime(Time.deltaTime * 2);
        }
        SceneManager.LoadScene("Bedroom");
    }
}

