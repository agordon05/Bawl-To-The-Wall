using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePillar : MonoBehaviour
{
    private GameManager gameManager;

    public bool isMoving;
    public bool isMovingForward;
    public bool isDone;

    public float speed = 10.0f;
    public float extendedWaitTime = 5.0f;
    private float timeAwake;



    Rigidbody pillarRb;


    //is given after instantiating from Spawn Pillar
    public GameObject spawnIndicator;

    /*
     * bound positions:
     * 0 = 300
     * 1 = 0
     * 2 = -150
     * 3 = 150
     * 4 = 150
     * 5 = -150
     */
    private float[] bounds = { 351, 0, 355, -355, 176.3f, -176.7f };

    public float frontBound;
    public float backBound;
    private float offset = 351 / 2;


    /*
     * initiates in start()
     * is gotten from spawnManager
     * 
     * 0 = ceiling wall
     * 1 = floor wall
     * 2 = front wall
     * 3 = back wall
     * 4 = left wall
     * 5 = right wall
     */
    public float startingWall;
    public Vector3 spawnPos;


    // Start is called before the first frame update
    void Start()
    {
        pillarRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //Debug.Log("pillar Spawned: true");
        getStartingWall();
        getBounds();

        isMoving = true;
        isMovingForward = true;
        spawnPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        timeAwake += Time.deltaTime;
        if (timeAwake > 10)
        {
            isMoving = true;
            isMovingForward = false;
            Debug.Log("pillar has been spawned for too long");
        }

        if (!gameManager.isGameOver)
        {
            //rbPosConstraint();
            if (isMoving)
            {
                //Debug.Log("isMoving is true");
                if (isMovingForward)
                {
                    //Debug.Log("isMovingforward is true");
                    moveForward();
                }
                else
                {
                    //Debug.Log("isMovingForward is false");
                    moveBackwards();
                }
            }
            //else Debug.Log("isMoving is false");
        }

    }




    void rbPosConstraint()
    {
        switch (startingWall)
        {
            //defualt is the ceiling
            default:
                {

                    Vector3 posCorrection = new Vector3(spawnPos.x - transform.position.x, 0, spawnPos.z - transform.position.z);
                    transform.Translate(posCorrection);

                    //Vector3 localVelocity = transform.InverseTransformDirection(pillarRb.velocity);
                    //localVelocity.x = 0;
                    //localVelocity.z = 0;

                    //pillarRb.velocity = transform.TransformDirection(localVelocity);
                }
                break;
            //floor
            case 1:
                {
                    Vector3 posCorrection = new Vector3(spawnPos.x - transform.position.x, 0, spawnPos.z - transform.position.z);
                    transform.Translate(posCorrection);

                    //Vector3 localVelocity = transform.InverseTransformDirection(pillarRb.velocity);
                    //localVelocity.x = 0;
                    //localVelocity.z = 0;

                    //pillarRb.velocity = transform.TransformDirection(localVelocity);
                }
                break;
            //front
            case 2:
                {
                    Vector3 posCorrection = new Vector3(spawnPos.x - transform.position.x, spawnPos.y - transform.position.y, 0);
                    transform.Translate(posCorrection);


                    //Vector3 localVelocity = transform.InverseTransformDirection(pillarRb.velocity);
                    //localVelocity.x = 0;
                    //localVelocity.y = 0;

                    //pillarRb.velocity = transform.TransformDirection(localVelocity);
                }
                break;
            //back
            case 3:
                {
                    Vector3 posCorrection = new Vector3(spawnPos.x - transform.position.x, spawnPos.y - transform.position.y, 0);
                    transform.Translate(posCorrection);

                    //Vector3 localVelocity = transform.InverseTransformDirection(pillarRb.velocity);
                    //localVelocity.x = 0;
                    //localVelocity.y = 0;

                    //pillarRb.velocity = transform.TransformDirection(localVelocity);
                }
                break;
            //left
            case 4:
                {

                    Vector3 posCorrection = new Vector3(0, -spawnPos.y + transform.position.y, -spawnPos.z + transform.position.z);
                    transform.Translate(posCorrection);

                    //Vector3 localVelocity = transform.InverseTransformDirection(pillarRb.velocity);
                    //localVelocity.y = 0;
                    //localVelocity.z = 0;

                    //pillarRb.velocity = transform.TransformDirection(localVelocity);
                }
                break;
            //right
            case 5:
                {

                    Vector3 posCorrection = new Vector3(0, -spawnPos.y + transform.position.y, -spawnPos.z + transform.position.z);
                    transform.Translate(posCorrection);

                    //Vector3 localVelocity = transform.InverseTransformDirection(pillarRb.velocity);
                    //localVelocity.y = 0;
                    //localVelocity.z = 0;

                    //pillarRb.velocity = transform.TransformDirection(localVelocity);
                }
                break;
        }
    }




    //stops pillar from moving and waits for extendedWaitTime Seconds
    IEnumerator waitTime()
    {
        isMoving = false;
        isMovingForward = false;
        //rbPosConstraint();
        yield return new WaitForSeconds(extendedWaitTime);
        isMoving = true;
    }









    //gets the front and back bounds to figure out when the pillar will stop and when it will be destroyed
    void getBounds()
    {
        switch (startingWall)
        {
            //defualt is the ceiling
            default:
                {
                    //Debug.Log("starting wall is ceiling");
                    backBound = bounds[0];
                    frontBound = bounds[1];
                }
                break;
            //floor
            case 1:
                {
                    //Debug.Log("starting wall is floor");
                    backBound = bounds[1];
                    frontBound = bounds[0];
                }
                break;
            //front X
            case 2:
                {
                    //Debug.Log("starting wall is front");
                    backBound = bounds[2];
                    frontBound = bounds[1];
                }
                break;
            //back X
            case 3:
                {
                    //Debug.Log("starting wall is back");
                    backBound = bounds[3];
                    frontBound = bounds[1];
                }
                break;
            //left
            case 4:
                {
                    //Debug.Log("starting wall is left");
                    backBound = bounds[4];
                    frontBound = bounds[1];
                }
                break;
            //right
            case 5:
                {
                    //Debug.Log("starting wall is right");
                    backBound = bounds[5];
                    frontBound = bounds[1];
                }
                break;
        }
    }









    /*
     * initiates in start()
     * is gotten from spawnManager
     * 
     * 0 = ceiling wall
     * 1 = floor wall
     * 2 = front wall
     * 3 = back wall
     * 4 = left wall
     * 5 = right wall
     */
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
            startingWall = 0;
        //started on floor X
        else if (xRotation == 0 && yRotation == 180 && zRotation == 0)
            startingWall = 1;
        //started on front X
        else if (xRotation == 0 && yRotation == 180 && zRotation == 270)
            startingWall = 2;
        //started on back X
        else if (xRotation == 0 && yRotation == 0 && zRotation == 270)
            startingWall = 3;
        //started on left X
        else if (xRotation == 0 && yRotation == 90 && zRotation == 270)
            startingWall = 4;
        //started on right X
        else if (xRotation == 0 && yRotation == 270 && zRotation == 270)
            startingWall = 5;
    }







    void moveForward()
    {
        //Debug.Log("Pillar moves");


        switch (startingWall)
        {

            //defualt is the ceiling X
            default:
                {
                    if ((transform.position.y - offset) > frontBound)
                    {
                        //transform.Translate(Vector3.down * speed * Time.deltaTime);
                        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //stops movement for extendedWaitTime
                        StartCoroutine(waitTime());
                    }
                }
                break;
            //floor X
            case 1:
                {
                    if ((transform.position.y + offset) < frontBound)
                    {
                        //Debug.Log("pillar moves up");
                        //transform.Translate(Vector3.up * speed * Time.deltaTime);
                        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //stops movement for extendedWaitTime
                        StartCoroutine(waitTime());
                    }
                }
                break;
            //front X
            case 2:
                {
                    if ((transform.position.x) > frontBound)
                    {
                        //transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //stops movement for extendedWaitTime
                        StartCoroutine(waitTime());
                    }
                }
                break;
            //back X
            case 3:
                {
                    if ((transform.position.x) < frontBound)
                    {
                        //transform.Translate(Vector3.back * speed * Time.deltaTime);

                        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //stops movement for extendedWaitTime
                        StartCoroutine(waitTime());
                    }
                }
                break;
            //left
            case 4:
                {
                    if ((transform.position.z) > frontBound)
                    {
                        //transform.Translate(Vector3.right * speed * Time.deltaTime);

                        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //stops movement for extendedWaitTime
                        StartCoroutine(waitTime());
                    }
                }
                break;
            //right X
            case 5:
                {
                    if ((transform.position.z) < frontBound)
                    {
                        //transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

                        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //stops movement for extendedWaitTime
                        StartCoroutine(waitTime());
                    }
                }
                break;


        }
    }

    void moveBackwards()
    {
        switch (startingWall)
        {
            //defualt is the ceiling X
            default:
                {
                    if ((transform.position.y - offset) < backBound)
                    {
                        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //destroys gameObject
                        OnDestroy();
                    }
                }
                break;
            //floor X
            case 1:
                {
                    if ((transform.position.y + offset) > backBound)
                    {
                        //Debug.Log("pillar moves down");
                        //transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
                        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //destroys gameObject
                        OnDestroy();
                    }
                }
                break;
            //front X
            case 2:
                {
                    if ((transform.position.x - offset) < backBound)
                    {
                        //transform.Translate(Vector3.back * speed * Time.deltaTime);
                        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //destroys gameObject
                        OnDestroy();
                    }
                }
                break;
            //back X
            case 3:
                {
                    if ((transform.position.x + offset) > backBound)
                    {
                        //transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //destroys gameObject
                        OnDestroy();
                    }
                }
                break;
            //left X
            case 4:
                {
                    if ((transform.position.z - offset) < backBound)
                    {
                        //transform.Translate(Vector3.left * speed * Time.deltaTime);
                        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //destroys gameObject
                        OnDestroy();
                    }
                }
                break;
            //right X
            case 5:
                {
                    if ((transform.position.z + offset) > backBound)
                    {
                        //transform.Translate(Vector3.right * speed * Time.deltaTime);
                        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);
                    }
                    else
                    {
                        //destroys gameObject
                        OnDestroy();
                    }
                }
                break;
        }
    }




    public void SpawnIndicator(GameObject indicator)
    {
        spawnIndicator = indicator;
        spawnIndicator.GetComponent<IndicatorBehavior>().isActive = true;
        //Debug.Log("Spawn Indicator has been switched to true");
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public void setWaitTime(float waitTime)
    {
        extendedWaitTime = waitTime;
    }



    private void OnDestroy()
    {
        if (spawnIndicator != null)
        {
            //Debug.Log("Spawn Indicator has switched to false");
            spawnIndicator.GetComponent<IndicatorBehavior>().isActive = false;
        }
        Destroy(gameObject);
    }



    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Inside on trigger enter");
        //Debug.Log(other.tag);
        //Debug.Log(other.gameObject.tag);
        //Debug.Log(tag);
        //Debug.Log(gameObject.tag);
        //if (CompareTag("Pillar"))
        //{
        Debug.Log(gameObject.name + " triggered: " + other.name);
        if (other.CompareTag("Pillar"))
        {

            //Debug.Log("hit Pillar");
            MovePillar otherPillarScript = other.gameObject.GetComponent<MovePillar>();
            if (otherPillarScript == null) Debug.Log("pillar does not have a script");
            if (otherPillarScript.isMovingForward == true)
            {
                StartCoroutine(otherPillarScript.waitTime());
            }
            if (isMovingForward == true)
            {
                StartCoroutine(waitTime());
            }
        }
        //}

        else if (other.CompareTag("Player") && isMovingForward)
        {
            Debug.Log("Player was hit");
            //StartCoroutine(waitTime());
            gameManager.gameOver();

        }
        else if (other.CompareTag("Chicken"))
        {
            Debug.Log("Chicken was hit");
            
        }

    }


}
