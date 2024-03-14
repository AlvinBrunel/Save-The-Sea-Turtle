using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeaController : MonoBehaviour
{
    public int PolutionLevel;
    [SerializeField] Color[] SeaColour;
    [SerializeField] Color[] ShallowSeaColour;

    [SerializeField] GameObject AlertGUI;
    AudioSource AS;

    GameObject GameController;
    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("Game");
        AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SeaPollution()
    {
        PolutionLevel++;

        GetComponent<MeshRenderer>().material.SetColor("_Shallow_Colour", ShallowSeaColour[PolutionLevel]);
        GetComponent<MeshRenderer>().material.SetColor("_Deep_Colour", SeaColour[PolutionLevel]);
        if(PolutionLevel >= 2)
        {
            GameController.GetComponent<GameController>().GameOver("The Sea has been polluted");
        }
        else
        {
            GameController.GetComponent<GameController>().InGameDialogue(GameController.GetComponent<GameController>().PollutionText[Random.Range(0, GameController.GetComponent<GameController>().PollutionText.Length - 1)]);
            if (AlertGUI.transform.parent.GetChild(1).gameObject.activeSelf)
            {
                AlertGUI.transform.parent.GetChild(1).gameObject.SetActive(false);
            }
            AlertGUI.SetActive(true);
            AlertGUI.GetComponent<FadeController>().StartCoroutine(AlertGUI.GetComponent<FadeController>().FadeAnimation(0.05f));
            AS.Play();
        }
    }
}
