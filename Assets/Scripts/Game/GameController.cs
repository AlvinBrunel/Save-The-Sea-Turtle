using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;

public class GameController : MonoBehaviour
{
    public int Round = 1;
    public int ActualRound = 1;

    public int EnemyWave;
    public int EnemiesLeft;
    int EnemiesLeftToSpawn;
    public int EnemiesExpelled;

    [SerializeField] int EnemyHorde;

    public int TurtlesSaved;


    public float StormStrength;
    [SerializeField] float initialStormStrength;


    [SerializeField] GameObject[] Enemies;
    [SerializeField] GameObject[] Litter;
    [SerializeField] GameObject Turtle;
    [SerializeField] Vector3 TurtleSpawnPoint;

    [SerializeField] GameObject MainCamera;

    [SerializeField] GameObject CutSceneCamera;
    [SerializeField] GameObject TextController;
    [SerializeField] GameObject DialogueBox;
    [SerializeField] GameObject Crosshair;
    [SerializeField] GameObject PowerUpGUI;
    [SerializeField] GameObject EndscreenGUI;
    [SerializeField] GameObject StormAlertGUI;

    [SerializeField] GameObject BubbleGun;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject FriendlyHand;
    [SerializeField] GameObject StormParticles;
    [SerializeField] GameObject[] UIElements;
    [SerializeField] GameObject RoundText;

    [SerializeField] Vector3[] CutSceneCameraPosition;
    [SerializeField] Quaternion[] CutSceneCameraQuaternion;


    [SerializeField] string[] EndOfRoundText;
    public string[] PollutionText;

    public GameObject[] PowerUpList;

    public Sprite[] PowerUpGUIList;
    public float PowerUpTimer = 0;
    [SerializeField] GameObject PowerUpController;

    public bool inCutscene = false;
    public int CurrentScene = 0;
    bool firstcutscene = true;

    [SerializeField] string[] CutSceneText;

    float PointX1;
    float PointX2;

    float PointZ1;
    float PointZ2;

    // Start is called before the first frame update
    void Start()
    {
        ChangeCutSceneCamera(CutSceneCameraPosition[0], CutSceneCameraQuaternion[0], CutSceneCamera);

        PointX1 = transform.GetChild(0).position.x;
        PointX2 = transform.GetChild(1).position.x;

        PointZ1 = transform.GetChild(2).position.z;
        PointZ2 = transform.GetChild(3).position.z;


        EnemyHorde = (int) (4f + Mathf.Pow(1.03f, Round) * (Mathf.Log(Round, 20f) * 10f) / 8f);
        EnemyWave = (int) (6f + (Mathf.Pow(Round,(0.4f * Mathf.Log(Round,10f)))) * Round * 1.88f); //= (int) ((Round+5)*(Round+5)*0.2f);

        EnemiesLeft = EnemyWave;
        EnemiesLeftToSpawn = EnemiesLeft;

        inCutscene = true;

        if (!PlayerPrefs.HasKey("Turtles Saved"))
        {
            PlayerPrefs.SetInt("Turtles Saved", 0);
        }
        if (!PlayerPrefs.HasKey("Enemies Expelled"))
        {
            PlayerPrefs.SetInt("Enemies Expelled", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && PowerUpController.GetComponent<Image>().sprite != null && PowerUpController.transform.parent.GetComponent<PowerUpGUIController>().TimeLeft == 0)
        {
            ActivatePowerUp();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        { 
            CameraShaker.Instance.ShakeOnce(5f, 10, 0, 1.5f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Enemies Left to Spawn: " + EnemiesLeftToSpawn);
            Debug.Log("Enemies Left: " + EnemiesLeft);
            Debug.Log("Actual Round: " + ActualRound);
        }

    }
    public void ChangeRound()
    {
        CancelInvoke("SpawnEnemy");
        InGameDialogue(EndOfRoundText[Random.Range(0,EndOfRoundText.Length-1)]);

        EnemyHorde = (int)(4f + Mathf.Pow(1.03f, Round) * (Mathf.Log(Round, 20f) * 10f) / 8f);
        EnemyWave = (int)(6f + (Mathf.Pow(Round, (0.4f * Mathf.Log(Round, 10f)))) * Round * 1.88f);

        Debug.Log(EnemyHorde);
        Debug.Log(EnemyWave);

        EnemiesLeft = EnemyWave;
        EnemiesLeftToSpawn = EnemiesLeft;

        if (StormStrength != 0)
        {
            EndStorm();
        }
        Round++;
        ActualRound++;
        RoundText.GetComponent<TextMeshProUGUI>().text = ActualRound.ToString();
        if (Round > 20)
        {
            Round = 20;
        }
        if (ActualRound % 4 == 0)
        {
            Invoke("StartStorm",1f);
        }
        else
        {
            InvokeRepeating("SpawnEnemy", 3f, 14f);
        }

    }
    void SpawnEnemy()
    {
        int loop;
        int RandEnemy;
        if (StormStrength>0)
        {
            loop = 1;
        }
        else
        {
            loop = EnemyHorde;
        }
        for (int i =0;i<loop;i++)
        {
            EnemiesLeftToSpawn--;
            if (ActualRound <= 2)
            {
                SpawnBasicHandEnemy();
            }
            else
            {
                RandEnemy = Random.Range(1, Round * 10);
                if ((RandEnemy > (Round * 10) - Round / 4) && StormStrength == 0)
                {
                    SpawnSpitterEnemy();
                }
                else if (RandEnemy >= (int) (Round * 10f) * (7f / 10f))
                {

                    SpawnBasicHandEnemy();
                }
                else
                {

                    SpawnFastHandEnemy();
                }
            }
            if (EnemiesLeftToSpawn <= 0)
            {
                CancelInvoke("SpawnEnemy");
                break;
            }
        }
    }
    void StartStorm()
    {
        CancelInvoke("SpawnEnemy");
        StormParticles.SetActive(true);
        StormStrength = initialStormStrength;
        StormAlertGUI.SetActive(true);
        StormAlertGUI.GetComponent<StormTextController>().enabled = true;
        StormAlertGUI.GetComponent<StormTextController>().StartCoroutine(StormAlertGUI.GetComponent<StormTextController>().TextAnimation(30));
        InvokeRepeating("SpawnEnemy", 10f, 13f); ;
        InvokeRepeating("SpawnLitter", 1f, 5f);
    }

    void EndStorm()
    {
        StormParticles.SetActive(false);
        CancelInvoke("Storm");
        CancelInvoke("SpawnLitter");
        CancelInvoke("SpawnEnemy");
        StormStrength = 0f;
    }
    void SpawnBasicHandEnemy()
    {
        Vector3 SpawnPoint = new Vector3(Random.Range(PointX1, PointX2), 105f, Random.Range(PointZ1, PointZ2));
        GameObject InsEnemy = Instantiate(Enemies[0], SpawnPoint, transform.rotation);
        InsEnemy.transform.name = "Hand Fast ";
    }
    void SpawnFastHandEnemy()
    {
        Vector3 SpawnPoint = new Vector3(Random.Range(PointX1, PointX2), 105f, Random.Range(PointZ1, PointZ2));
        GameObject InsEnemy = Instantiate(Enemies[1], SpawnPoint, transform.rotation);
        InsEnemy.transform.name = "Hand Strong";
    }
    void SpawnSpitterEnemy()
    {
        Vector3 SpawnPoint = new Vector3(Random.Range(PointX1, PointX2), 105f, Random.Range(PointZ1, PointZ2));
        GameObject InsEnemy = Instantiate(Enemies[2], SpawnPoint, transform.rotation);
        InsEnemy.transform.name = "Stomp";
    }
    void SpawnLitter()
    {
        Vector3 SpawnPoint = new Vector3(Random.Range(PointX1, PointX2), 105f, Random.Range(PointZ1, PointZ2));
        GameObject InsEnemy = Instantiate(Litter[Random.Range(0,Litter.Length-1)], SpawnPoint, transform.rotation);
        InsEnemy.transform.name = "Litter";
    }


    void ChangeCutSceneCamera(Vector3 CamPos, Quaternion CamRot, GameObject Cam)
    {
        Cam.transform.position = CamPos;
        Cam.transform.rotation = CamRot;
    }
    public void ChangeCutScene(int Instance)
    {
        if (Instance == CutSceneCameraPosition.Length - 1)
        {
            StormParticles.SetActive(true);
        }

        if (Instance < CutSceneCameraPosition.Length)
        {
            ChangeCutSceneCamera(CutSceneCameraPosition[Instance], CutSceneCameraQuaternion[Instance], CutSceneCamera);
            TextController.GetComponent<DialogueBoxController>().Text = CutSceneText[Instance];
            inCutscene = true;
        }
        else
        {
            RoundText.SetActive(true);
            Player.SetActive(true);
            StormParticles.SetActive(false);
            inCutscene = false;
            BubbleGun.GetComponent<WeaponController>().enabled = true;
            MainCamera.GetComponent<CameraController>().enabled = true;
            MainCamera.SetActive(true);
            PowerUpGUI.SetActive(true);
            CutSceneCamera.SetActive(false);
            Player.SetActive(true);
            Crosshair.SetActive(true);
            DialogueBox.SetActive(false);

            if (firstcutscene == true)
            {
                Destroy(GameObject.Find("Rettips"));
                Destroy(GameObject.Find("Hand"));
                Destroy(GameObject.Find("PowerUpBubble"));

                firstcutscene = false;

                InvokeRepeating("SpawnEnemy", 3f,14f);
                //Invoke("FriendlyHandPowerUp", 1f);
                Invoke("SpawnTurtle", Random.Range(100f, 110f));
            }

        }
    }
    void ActivatePowerUp()
    {
        if(PowerUpTimer != 0)
        {
            PowerUpController.transform.parent.GetComponent<PowerUpGUIController>().TimeLeft = PowerUpTimer;
            PowerUpController.transform.parent.GetComponent<PowerUpGUIController>().SetPowerUpTimer();
            PowerUpTimer = 0;
        }
        if(PowerUpController.GetComponent<Image>().sprite == PowerUpGUIList[0])
        {
            PowerUpController.GetComponent<Image>().sprite = null;
            ChangePowerUpGUI(null);
            BubblePowerUp();
        }
        if (PowerUpController.GetComponent<Image>().sprite == PowerUpGUIList[1])
        {
            InfiniteClipPowerUp();
        }
        if (PowerUpController.GetComponent<Image>().sprite == PowerUpGUIList[2])
        {
            FriendlyHandPowerUp();
        }

    }
    public void InGameDialogue(string text)
    {
        TextController.transform.parent.gameObject.SetActive(isActiveAndEnabled); 
        TextController.GetComponent<DialogueBoxController>().Text = text;
        TextController.GetComponent<DialogueBoxController>().ChangeText();
    }
    void BubblePowerUp()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject Ens in Enemies)
        {
            Ens.transform.GetChild(0).gameObject.SetActive(true);
            Ens.transform.GetComponent<Rigidbody>().useGravity = false;
            Destroy(Ens.transform.GetComponent<NavMeshAgent>());
            if (Ens.gameObject.GetComponent<EnemyHandController>())
            {
                Ens.gameObject.GetComponent<EnemyHandController>().Invoke("DestroyEnemy", 10f);
            }
            else if (Ens.gameObject.GetComponent<SpitterController>())
            {
                Ens.gameObject.GetComponent<SpitterController>().Invoke("DestroyEnemy", 10f);
            }
            else if (Ens.gameObject.GetComponent<LitterController>())
            {
                Ens.gameObject.GetComponent<LitterController>().Invoke("DestroyLitter", 10f);
            }
            Ens.transform.GetComponent<BoxCollider>().enabled = false;
            Ens.transform.tag = "BubbledEnemy";
            Destroy(Ens.transform.GetChild(1).GetComponent<Animator>());
            Destroy(Ens.transform.GetComponent<NavMeshAgent>());
            GetComponent<GameController>().EnemiesLeft--;
            GetComponent<GameController>().EnemiesExpelled++;
            if (GetComponent<GameController>().EnemiesLeft <= 0 || GetComponent<GameController>().EnemiesLeftToSpawn <= 0)
            {
                GetComponent<GameController>().ChangeRound();
            }
        }
    }
    void InfiniteClipPowerUp()
    {
        BubbleGun.GetComponent<WeaponController>().InfiniteAmmo = true;
    }
    void FriendlyHandPowerUp()
    {
        GameObject InsFriendlyHand = Instantiate(FriendlyHand,new Vector3(Player.transform.position.x, 125f, Player.transform.position.z),transform.rotation);
        InsFriendlyHand.transform.name = "Friendly Hand";
    }
    public void ChangePowerUpGUI(Sprite Img)
    {
        if(Img == null)
        {
            PowerUpGUI.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            PowerUpController.GetComponent<Image>().enabled = false;
        }
        else
        {
            PowerUpGUI.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            PowerUpController.GetComponent<Image>().enabled = true;
        }
        PowerUpController.GetComponent<Image>().sprite = Img;
    }
    public void SpawnTurtle()
    {
        GameObject InsTurtle = Instantiate(Turtle,TurtleSpawnPoint,transform.rotation);
        InsTurtle.transform.name = "Turtle";

        Invoke("SpawnTurtle", Random.Range(40f, 55f));
    }
    public void GameOver(string Reason)
    {
        CancelInvoke("SpawnEnemies");
        CancelInvoke("SpawnTurtle");
        foreach(GameObject GUIbubs in BubbleGun.GetComponent<WeaponController>().GUIBubbles)
        {
            GUIbubs.SetActive(false);
        }
        foreach (GameObject UI in UIElements)
        {
            UI.SetActive(false);
        }
        EndscreenGUI.SetActive(true);
        foreach (GameObject Enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(Enemy);
        }
        foreach (GameObject BubbledEnemy in GameObject.FindGameObjectsWithTag("BubbledEnemy"))
        {
            Destroy(BubbledEnemy);
        }
        foreach (GameObject Animal in GameObject.FindGameObjectsWithTag("Animal"))
        {
            Destroy(Animal);
        }
        foreach (GameObject Friendly in GameObject.FindGameObjectsWithTag("Friendly"))
        {
            Destroy(Friendly);
        }

        Destroy(GameObject.Find("Player"));
        Destroy(MainCamera.transform.parent.gameObject);

        for (int i = 0; i <= BubbleGun.GetComponent<WeaponController>().MaxClipSize - 1; i++)
        {
            BubbleGun.GetComponent<WeaponController>().GUIBubbles[i].SetActive(false);
        }
        RoundText.SetActive(false);
        CutSceneCamera.SetActive(true);
        ChangeCutSceneCamera(new Vector3(-9.2f, 162.62f, 1.436f), Quaternion.Euler(-26.707f, 337.746f, 0f), CutSceneCamera);

       MainCamera.SetActive(false);

        EndscreenGUI.SetActive(true);

        PlayerPrefs.SetInt("Enemies Expelled", PlayerPrefs.GetInt("Enemies Expelled")+EnemiesExpelled);
        PlayerPrefs.SetInt("Turtles Saved", PlayerPrefs.GetInt("Turtles Saved") + TurtlesSaved);


        EndscreenGUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Reason;
        if (TurtlesSaved == 1)
        {
            EndscreenGUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "You Saved " + TurtlesSaved + " Turtle and " + PlayerPrefs.GetInt("Turtles Saved") + " in total";
        }
        else
        {
            EndscreenGUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "You Saved " + TurtlesSaved + " Turtles and " + PlayerPrefs.GetInt("Turtles Saved") + " in total";
        }
        if (EnemiesExpelled == 1)
        {
            EndscreenGUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You Expelled " + EnemiesExpelled + " Enemy and " + PlayerPrefs.GetInt("Enemies Expelled") + " in total";
        }
        else
        {
            EndscreenGUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You Expelled " + EnemiesExpelled + " Enemies and " + PlayerPrefs.GetInt("Enemies Expelled") + " in total";
        }

        gameObject.GetComponent<GameOverController>().enabled = true;
        Destroy(gameObject.GetComponent<GameController>());
    }

}
