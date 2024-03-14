using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PowerUpGUIController : MonoBehaviour
{
    public float TimeLeft;
    [SerializeField] GameObject GameController;

    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("Game");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            TimeLeft = 20f;
            SetPowerUpTimer();
        }
    }
    public void SetPowerUpTimer()
    {
        TimeLeft--;
        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((int)TimeLeft).ToString();
        if (TimeLeft > 0)
        {
            Invoke("SetPowerUpTimer", 1f);
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            RemovePowerUp();
            GameController.GetComponent<GameController>().ChangePowerUpGUI(null);
        }
    }
    void RemovePowerUp()
    {
        if (transform.GetChild(0).GetComponent<Image>().sprite == GameController.GetComponent<GameController>().PowerUpGUIList[1])
        {
            GameObject.Find("Bubblegun").GetComponent<WeaponController>().InfiniteAmmo = false;
        }
        if (transform.GetChild(0).GetComponent<Image>().sprite == GameController.GetComponent<GameController>().PowerUpGUIList[2])
        {
            GameObject.Find("Friendly Hand").GetComponent<FriendlyHandController>().PowerUpFinished();
        }
    }
}
