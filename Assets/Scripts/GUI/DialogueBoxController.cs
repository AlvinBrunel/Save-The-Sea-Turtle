using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxController : MonoBehaviour
{
    public string Text;
    [SerializeField] float TextSpeed;

    [SerializeField] char[] CharArr;

    AudioSource As;
    [SerializeField] float pitchdiff;
    [SerializeField] float Volumediff;

    [SerializeField] GameObject GameController;
    [SerializeField] GameObject Weapon;

    int CurrentPoint;
    int BubbleBulletSize;

    TMPro.TextMeshProUGUI TMP;
    // Start is called before the first frame update
    void Start()
    {
        TMP = GetComponent<TMPro.TextMeshProUGUI>();
        As = GetComponent<AudioSource>();
        ChangeText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (TMP.text != Text)
            {
                CancelInvoke("DisplayText");
                TypingSoundEffect();
                TMP.text = Text;
                transform.parent.GetChild(1).gameObject.SetActive(true);
                if (!GameController.GetComponent<GameController>().inCutscene)
                {
                    Invoke("CloseDialogueBox", 5f);
                }

            }
            else
            {
                if(GameController.GetComponent<GameController>().inCutscene)
                {
                    GameController.GetComponent<GameController>().CurrentScene++;
                    GameController.GetComponent<GameController>().ChangeCutScene(GameController.GetComponent<GameController>().CurrentScene);
                    ChangeText();
                }
                else 
                {
                    if (IsInvoking("CloseDialogueBox"))
                    {
                        CancelInvoke("CloseDialogueBox");
                    }
                    CloseDialogueBox();
                }

            }


        }
    }
    string SetupText()
    {
        CurrentPoint++;
        return new string(CharArr[0..CurrentPoint]);
    }
    void DisplayText()
    {
        TMP.text = SetupText();

        if(CurrentPoint < CharArr.Length)
        {
            Invoke("DisplayText", TextSpeed);
        }
        else
        {
            transform.parent.GetChild(1).gameObject.SetActive(true);
            TypingSoundEffect();
            if(!GameController.GetComponent<GameController>().inCutscene)
            {
                Invoke("CloseDialogueBox", 5f);
            }
        }

    }
    public void ChangeText()
    {
        //ToggleGUI(true);
        CharArr = Text.ToCharArray();
        CurrentPoint = 0;
        transform.parent.GetChild(1).gameObject.SetActive(false);
        DisplayText();
    }
    void TypingSoundEffect()
    {
        As.pitch = Random.Range(1-pitchdiff, 1+pitchdiff);
        As.volume = Random.Range(1 - Volumediff, 1);
        As.Play();
    }
    public void ToggleGUI(bool Toggle)
    {
        if (Toggle == true)
        {
            transform.parent.parent.GetChild(0).gameObject.SetActive(false);
            if (Weapon.GetComponent<WeaponController>().enabled)
            { 
                BubbleBulletSize = Weapon.GetComponent<WeaponController>().MaxClipSize;
                for (int i = 2; i < 2 + (BubbleBulletSize); i++)
                {
                    transform.parent.parent.GetChild(i).gameObject.SetActive(false);

                }
            }
        }
        else
        {
            transform.parent.parent.GetChild(0).gameObject.SetActive(true);
            if (Weapon.GetComponent<WeaponController>().enabled)
            {
                BubbleBulletSize = Weapon.GetComponent<WeaponController>().MaxClipSize;
                for (int i = 2; i < 2 + (BubbleBulletSize); i++)
                {
                    transform.parent.parent.GetChild(i).gameObject.SetActive(true);

                }
            }
            transform.parent.parent.GetChild(1).gameObject.SetActive(false);
        }
           
    }
    void CloseDialogueBox()
    {
        //Dialogue off sound effect???
        transform.parent.gameObject.SetActive(false);
    }
}
