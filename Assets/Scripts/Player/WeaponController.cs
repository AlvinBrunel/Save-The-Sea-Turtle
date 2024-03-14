using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;
public class WeaponController : MonoBehaviour
{
    [SerializeField] GameObject Bullet;
    [SerializeField] float Firingrate;

    [SerializeField] Transform KickBackPosition;
    [SerializeField] Transform InitialPos;

    [SerializeField] Quaternion InitialRotationPos;

    [SerializeField] float KickBackAmount;

    [SerializeField] float TransitionSpeed;

    bool firing = false;
    bool Reloading = false;
    public bool InfiniteAmmo = false;

    [SerializeField] float ReloadTime;

    int CurrentClipSize;
    public int MaxClipSize;

    [SerializeField] int GUIGap;

    [SerializeField] GameObject Canvas;
    [SerializeField] GameObject GUIBubble;
    [SerializeField] GameObject GameController;
    [SerializeField] int GUIBubbleSize;

    public GameObject[] GUIBubbles;
    [SerializeField] Vector2 GUIPos;
    RectTransform Rect;

    AudioSource AS;
    [SerializeField] AudioClip[] AC;

    // Start is called before the first frame update
    void Start()
    {
        GUIGap = GUIBubbleSize * MaxClipSize + 710;
        CurrentClipSize = MaxClipSize;
        GUIBubbles = new GameObject[MaxClipSize];
        for(int i = 0;i<=MaxClipSize-1;i++)
        {
            GameObject InsBubble = Instantiate(GUIBubble);
            GUIBubbles[i] = InsBubble;
            InsBubble.transform.SetParent(Canvas.transform);
            InsBubble.transform.name = "Bubble"+(i+1);
            Rect = InsBubble.GetComponent<RectTransform>();
            Rect.sizeDelta = new Vector2(GUIBubbleSize, GUIBubbleSize);
            Rect.position = new Vector2(GUIPos.x-((i+1)*GUIBubbleSize) + GUIGap,GUIPos.y);
            InsBubble.transform.SetSiblingIndex(0);
        }

        AS = GetComponent<AudioSource>();
        InitialRotationPos = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GetComponent<GameController>().inCutscene == false)
        {
            if (CurrentClipSize <= 0 && Input.GetMouseButtonUp(0) && AS.isPlaying == false)
            {
                AS.clip = AC[1];
                AS.Play();
            }
            if (Input.GetMouseButtonUp(0) && firing == false && CurrentClipSize > 0 && Reloading == false)
            {
                if (InfiniteAmmo == false)
                {
                    CurrentClipSize--;
                    GUIBubbles[CurrentClipSize].SetActive(false);
                }
                GameObject InsBullet = Instantiate(Bullet, transform.GetChild(0).position, transform.parent.rotation);
                InsBullet.transform.name = Bullet.name;
                Invoke("CoolingDown", Firingrate);
                firing = true;
                StartCoroutine(KickBackController(8));
                CameraShaker.Instance.ShakeOnce(0.4f, 10, 0, 1.5f);

                AS.clip = AC[0];
                AS.Play();
            }
            if ((CurrentClipSize <= 0 && Reloading == false) || (Input.GetKeyDown(KeyCode.R) && Reloading == false && CurrentClipSize != MaxClipSize))
            {
                StartCoroutine(ReloadAnimation(60));
                Reloading = true;
            }
            if (Reloading == false && transform.rotation != InitialRotationPos)
            {
                //transform.localRotation = InitialRotationPos;
            }
        }
    }
    void CoolingDown()
    {
        firing = false;
    }
    private IEnumerator KickBackController(int intervals)
    {
        bool Finished = false;
        while (Finished == false)
        {
            for (int i = 0; i < intervals; i++)
            {
                yield return new WaitForSeconds(0.02f);
                transform.position -= ((transform.position - KickBackPosition.position) / intervals);
            }
            for (int i = 0; i < intervals; i++)
            {
                yield return new WaitForSeconds(0.02f);
                transform.position += ((transform.position - KickBackPosition.position) / intervals);
            }
            transform.position = InitialPos.position;
            Finished = true;
        }

    }
    private IEnumerator ReloadAnimation(int intervals)
    {
        bool Finished = false;
        while (Finished == false)
        {
            for (int i = 0; i < intervals; i++)
            {
                yield return new WaitForSecondsRealtime((ReloadTime / 4) / (intervals));
                transform.rotation *= (Quaternion.Euler(360 / intervals,0,0));
            }

            for (int i = 0; i < intervals/4; i++)
            {
                yield return new WaitForSecondsRealtime((ReloadTime/4)/(intervals/2));
                transform.position -= ((transform.position - KickBackPosition.position) / (intervals/4));
            }
            for (int i = 0; i < intervals/4; i++)
            {
                yield return new WaitForSecondsRealtime((ReloadTime/6)/(intervals/2));
                transform.position += ((transform.position - KickBackPosition.position) / (intervals / 4));
            }
            transform.position = InitialPos.position;
            transform.localRotation = InitialRotationPos;

            CurrentClipSize = MaxClipSize;
            for (int i = 0; i <= MaxClipSize - 1; i++)
            {
                GUIBubbles[i].SetActive(true);
            }

            Reloading = false;
            Finished = true;
        }
    }
}
