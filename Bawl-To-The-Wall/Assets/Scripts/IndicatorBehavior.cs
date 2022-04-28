using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IndicatorBehavior : MonoBehaviour
{

    private float timeAwake = 3;
    private bool isActive;
    private int wall;

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

    public AudioClip[] soundList;
    private AudioSource sound;

    public GameObject warningPillar;
    private GameObject instantiatedWarningPillar;


    private GameManager gameManager;
    private GameObject spawnManager;



    // Start is called before the first frame update
    void Start()
    {
        
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnManager = GameObject.FindGameObjectWithTag("Spawn Manager");
        //isMainMenu = false;
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

    private IEnumerator indicatorTimeAwake(int behavior)
    {

        if (warningPillar != null && gameManager.getDifficulty() == 1)
        {
            SpawnPillars spawnPillarsScript = spawnManager.GetComponent<SpawnPillars>();
            instantiatedWarningPillar = Instantiate(warningPillar, spawnPillarsScript.GetSpawnPos(transform.position, wall) + Offset(wall), gameObject.transform.rotation);
        }


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


    private Vector3 Offset(int wallIndex)
    {
        float spawnOffset = spawnManager.GetComponent<SpawnPillars>().getSpawnOffset();
        Vector3 offset;
        int offsetMultiplier = 2;
        switch (wallIndex)
        {
            //ceiling
            default:
                {
                    offset = new Vector3(0, -offsetMultiplier * spawnOffset, 0);
                }
                break;
                //floor
            case 1:
                {
                    offset = new Vector3(0, offsetMultiplier * spawnOffset, 0);

                }
                break;
                //front
            case 2:
                {
                    offset = new Vector3(-offsetMultiplier * spawnOffset, 0, 0);

                }
                break;
                //back
            case 3:
                {
                    offset = new Vector3(offsetMultiplier * spawnOffset, 0, 0);

                }
                break;
                //left
            case 4:
                {
                    offset = new Vector3(0, 0, -offsetMultiplier * spawnOffset);

                }
                break;
                //right
            case 5:
                {
                    offset = new Vector3(0, 0, offsetMultiplier * spawnOffset);

                }
                break;


        }





        return offset;
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


    private void getStartingWall()
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






    private void playSound(int behavior)
    {
        sound.volume = gameManager.getVolume();
        sound.PlayOneShot(soundList[behavior]);
    }


    private void isBetweenRound()
    {
        if (gameManager.isBetweenRound)
        {
            if (instantiatedWarningPillar != null) Destroy(instantiatedWarningPillar);

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

    public GameObject getWarningPillar()
    {
        return instantiatedWarningPillar;
    }

    public bool getIsActive()
    {
        return isActive;
    }
    public void setActive(bool active)
    {
        isActive = active;
    }
    public float getTimeAwake()
    {
        return timeAwake;
    }




}
