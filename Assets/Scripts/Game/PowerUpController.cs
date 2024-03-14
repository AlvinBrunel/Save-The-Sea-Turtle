using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float Duration;
    [SerializeField] float PowerUpDuration;

    [SerializeField] float FallingSpeed;
    [SerializeField] GameObject Player;

    [SerializeField] Sprite PowerUpGUI;
    [SerializeField] GameObject GameController;
    [SerializeField] GameObject PowerUpGUIController;
    void Start()
    {
        Player = GameObject.Find("Player");
        GameController = GameObject.Find("Game");
        PowerUpGUIController = GameObject.Find("PowerUP GUI");

        Invoke("DestroyPowerUp", Duration);
    }

    // Update is called once per frame
    void Update()
    {
    }
    void DestroyPowerUp()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Terrain")
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            GetComponent<Collider>().isTrigger = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.transform.gameObject == Player || (other.transform.name == "Body")) && PowerUpGUIController.GetComponent<Image>().sprite == null)
        {

            GameController.GetComponent<GameController>().ChangePowerUpGUI(PowerUpGUI);
            GameController.GetComponent<GameController>().PowerUpTimer = PowerUpDuration;
            DestroyPowerUp();
        }

    }
}
