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
        if (sound == null) gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator indicatorTimeAwake(int value)
    {

        //Debug.Log("inside Activate,     ");
        isActive = true;
        icons[value].SetActive(true);
        playSound(value);
        //indicatorTimeAwake();

        yield return new WaitForSeconds(timeAwake);


        //Debug.Log("deactivating icon");
        icons[value].SetActive(false);
    }


    //value corresponds to the type of indicator and sound that should be used
    public void Activate(int value)
    {
        if (value < 0) return;
        if (value < icons.Length && value < soundList.Length)
        {
            StartCoroutine(indicatorTimeAwake(value));
        }
    }

    //void setVolumeSetttings(float value)
    //{
    //    if (value < 0) value = 0;
    //    if (value > 1) value = 1;

    //    volumeLevel = value;

    //    sound.volume = value;

    //}

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


    void playSound(int value)
    {
        sound.volume = gameManager.volume;
        sound.PlayOneShot(soundList[value]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnPillars spawnPillarScript = spawnManager.GetComponent<SpawnPillars>();
            spawnPillarScript.SpawnPillar(timeAwake, gameObject, wall, 0);



        }
    }







}
