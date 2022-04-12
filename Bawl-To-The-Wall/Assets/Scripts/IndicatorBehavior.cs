using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IndicatorBehavior : MonoBehaviour
{

    private bool isMainMenu;
    public float timeAwake = 3;
    public float volumeLevel;
    public bool isActive;
    public int wall;

    public float MainMenuIconMinTime = 3;
    public float MainMenuIconMaxTime = 5;

    /*
     * list: (icon (sound))
     * 
     * 0 = Red Eye (Hit1)
     * 1 = All Seeing Eye (Hit2)
     * 2 = Quick (Hit3)
     * 3 = Target (Hit4)
     * 4 = Eye Bleed (Hit5)
     * 5 = Diamond (Hit6)
     * 6 = Tear Drop (Jump1)
     */
    public GameObject[] icons;

    //public AudioSource[] sounds;
    public AudioClip[] soundList;
    public AudioSource sound;

    public GameObject spawnManager;

    private GameManager gameManager;



    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        isMainMenu = false;
        isActive = false;
        sound = GetComponent<AudioSource>();
        getStartingWall();
        if (sound == null) gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        isBetweenRound();
    }

    IEnumerator indicatorTimeAwake(int behavior)
    {

        //Debug.Log("inside Activate,     ");

        if (!gameManager.isBetweenRound)
        {
            isActive = true;
            icons[behavior].SetActive(true);
            playSound(behavior);
        }
        //indicatorTimeAwake();

        yield return new WaitForSeconds(timeAwake);


        //Debug.Log("deactivating icon");
        icons[behavior].SetActive(false);
    }


    //value corresponds to the type of indicator and sound that should be used
    public void Activate(int behavior)
    {
        if (behavior < 0) return;
        if (behavior < icons.Length && behavior < soundList.Length)
        {
            StartCoroutine(indicatorTimeAwake(behavior));
        }
    }

    IEnumerator IconWait(float randTime, int randInt)
    {
        yield return new WaitForSeconds(randTime);
        icons[randInt].SetActive(false);
    }


    void MainMenuBehavior()
    {
        while (isMainMenu)
        {
            int randInt = (int)Random.Range(0, icons.Length);
            float randTime = Random.Range(MainMenuIconMinTime, MainMenuIconMaxTime);
            icons[randInt].SetActive(true);
            StartCoroutine(IconWait(randTime, randInt));

        }
    }



    void getStartingWall()
    {
        int xRotation = (int)transform.eulerAngles.x;
        int yRotation = (int)transform.eulerAngles.y;
        int zRotation = (int)transform.eulerAngles.z;




        //Debug.Log("x rotation: " + xRotation + ", y rotation: " + yRotation + ", z rotation: " + zRotation);
        //Debug.Log("xRotation is " + xRotation);
        //Debug.Log("zRoation is " + zRotation);

        //based off the euler wall rotation
        //started on ceiling
        if (xRotation == 0 && yRotation == 0 && zRotation == 180)
            wall = 0;
        //started on floor X
        else if (xRotation == 0 && yRotation == 180 && zRotation == 0)
            wall = 1;
        //started on front X
        else if (xRotation == 0 && yRotation == 180 && zRotation == 270)
            wall = 2;
        //started on back X
        else if (xRotation == 0 && yRotation == 0 && zRotation == 270)
            wall = 3;
        //started on left X
        else if (xRotation == 0 && yRotation == 90 && zRotation == 270)
            wall = 4;
        //started on right X
        else if (xRotation == 0 && yRotation == 270 && zRotation == 270)
            wall = 5;
    }




    void setMainMenu(bool Value)
    {
        isMainMenu = Value;
        MainMenuBehavior();
    }


    void playSound(int behavior)
    {
        sound.volume = gameManager.volume;
        sound.PlayOneShot(soundList[behavior]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnPillars spawnPillarScript = spawnManager.GetComponent<SpawnPillars>();
            spawnPillarScript.SpawnPillar(timeAwake, gameObject, wall, 0);



        }
    }

    void isBetweenRound()
    {
        if (gameManager.isBetweenRound)
        {
            isActive = false;
            for (int n = 0; n < icons.Length; n++)
            {
                icons[n].SetActive(false);
            }
        }
    }


    public void setTimeAwake(float time)
    {
        if (time < 1) time = 1;
        timeAwake = time;
    }



}
