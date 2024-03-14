using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] KeyCode KeyEndGame = KeyCode.E;
    [SerializeField] GameObject FadeController;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyEndGame))
        {
            FadeController.GetComponent<ScreenFadeController>().StartCoroutine(FadeController.GetComponent<ScreenFadeController>().FadeAnimationOut(0.01f));
        }
    }
}
