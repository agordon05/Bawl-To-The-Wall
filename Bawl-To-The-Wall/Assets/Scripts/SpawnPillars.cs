using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPillars : MonoBehaviour
{
    public GameObject pillar;
    public GameObject player;

    private GameManager gameManager;
    private int difficulty;

    /*
     * Walls array:
     * 0 = Ceiling  (plane axis': X & Z)
     * 1 = Floor    (plane axis': X & Z)
     * 2 = Front    (plane axis': Y & Z)
     * 3 = back     (plane axis': Y & Z)
     * 4 = left     (plane axis': X & Y)
     * 5 = right    (plane axis': X & Y)
     * 
     * (Pillar Spawn Information)
     * Paths:
     * walls (tag: "Arena")
     *      wall (Rotation, Global Y Position) (tag: "wall")
     *          fake Pillars (tag: "Wall Indicator")
     *                  rows # (Local Z Position) (tag: "Indicator Rows")
     *                      cube # (Local X Position) (tag: "Indicator")
     * 
     * 
     */
    public GameObject[] Walls;

    //range from player that a pillar is allowed to instantiate from in a certain wall
    //half the length of the pillar
    public float PillarOffset;

    //transform of opposite wall - where the pillar transform is when fully extended
    public float spawnOffset = 150;

    public float repeatSpawnTime = 10.0f;

    private int[] weights = { 0, 0, 0, 0, 0, 0, 0 };





    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        spawnOffset = (pillar.GetComponent<Transform>().localScale.y * pillar.GetComponent<BoxCollider>().size.y) / 2;
        //Debug.Log("PillarOffset: " + PillarOffset);




        //InvokeRepeating("SpawnPillar", delayedSpawnTime, repeatSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {

    }






    void SpawnPillar()
    {
        if (gameManager.isGameActive && !gameManager.isGameOver && !gameManager.isBetweenRound)
        {
            int behavior = GetRandBehavior();

            behavior = 0;

            switch (behavior)
            {
                default:
                    {
                        defaultSpawn(0);
                    }
                    break;
                case 1:
                    {
                        RowSpawn(behavior);
                    }
                    break;
                case 2:
                    {
                        QuickPillarSpawn(behavior);
                    }
                    break;
                case 3:
                    {
                        PointSpawn(behavior);
                    }
                    break;
                case 4:
                    {
                        MassPoint(behavior);
                    }
                    break;
                case 5:
                    {
                        AdjacentSpawn(behavior);
                    }
                    break;
                case 6:
                    {
                        SlowSpawn(behavior);
                    }
                    break;

            }
        }

    }

    public void SpawnPillar(float timeAwake, GameObject indicator, int startingWall, int behavior)
    {
        StartCoroutine(IndicatorWaitTime(timeAwake, indicator, startingWall, behavior));
    }



    //spawns a single pillar
    void defaultSpawn(int behavior)
    {
        //gets player Position
        Vector3 playerPos = player.transform.position;

        GameObject selectedIndicator = null;
        //bool hasIndicator = false;

        int wallIndex = 0;



        //stops pillars spawning in an indicator slot that is already being used
        //while (!hasIndicator)
        //{
        //Selects Random Wall
        wallIndex = (int)Random.Range(0, Walls.Length);

        //gets indicators that indicate where pillar will spawn at
        GameObject wall = Walls[wallIndex];
        List<GameObject> indicators = GetIndicators(wall);

        //finds select Positions close to player position
        //within about 35 for center Pillar (the pillar on that wall thats closest to players position
        //other select positions must be within about 53?

        List<GameObject> indicatorList = GetIndicatorSelection(indicators, wallIndex);
        //Debug.Log("indicator count = " + indicatorList.Count);
        //selects the indicator that the pillar will spawn from
        selectedIndicator = SelectedIndicator(indicatorList);
        //if (selectedIndicator != null) hasIndicator = true;

        //}



        if (selectedIndicator == null) return;


        float waitTime = selectedIndicator.GetComponent<IndicatorBehavior>().timeAwake;
        if (!gameManager.isBetweenRound)
            StartCoroutine(IndicatorWaitTime(waitTime, selectedIndicator, wallIndex, behavior));

    }


    //behavior = 1
    //spawns an entire row of pillars
    void RowSpawn(int behavior)
    {

    }


    //behavior = 2
    //short indicator wait time and pillar extends quicker
    void QuickPillarSpawn(int behavior)
    {

    }


    //behavior = 3
    //spawns a  pillars every wall that converge to a single point
    void PointSpawn(int behaviour)
    {

    }

    //behavior = 4
    //spawns a bunch of pillars from a single wall
    void MassPoint(int behavior)
    {

    }



    //behavior = 5
    //spawns two pillars from a connecting wall
    void AdjacentSpawn(int behavior)
    {

    }


    //behavior = 6
    //spawns a pillar that extends slowly
    void SlowSpawn(int behavior)
    {

    }







    IEnumerator IndicatorWaitTime(float waitTime, GameObject SelectedIndicator, int wallIndex, int behavior)
    {
        if (gameManager.isBetweenRound) yield return new WaitForSeconds(0);
        SelectedIndicator.GetComponent<IndicatorBehavior>().Activate(behavior);

        //Selects random Pillar position from selection
        Vector3 randPillarPos = SelectedIndicator.transform.position;

        //gathers neccesary information in order to spawn pillar
        Vector3 rotation = GetPillarRotation(wallIndex);
        //Debug.Log("Wall Index: " + wallIndex);
        yield return new WaitForSeconds(waitTime);

        if (gameManager.isGameActive && !gameManager.isGameOver && !gameManager.isBetweenRound)
        {
            //spawns pillar
            GameObject Pillar = Instantiate(pillar, GetSpawnPos(randPillarPos, wallIndex), Quaternion.Euler(rotation));

            pillar.GetComponent<MovePillar>().SpawnIndicator(SelectedIndicator);
        }

    }


    Vector3 GetSpawnPos(Vector3 indicatorPos, float wallIndex)
    {



        /*
         * yxz
        *Walls array:
        *0 = Ceiling(plane axis': X & Z)
        * 1 = Floor(plane axis': X & Z)
        * 2 = Front(plane axis': Y & Z)
        * 3 = back(plane axis': Y & Z)
        * 4 = left(plane axis': X & Y)
        * 5 = right(plane axis': X & Y)
        */
        switch (wallIndex)
        {
            //Ceiling
            case 0:
                {
                    indicatorPos.y += spawnOffset;
                }
                break;
            //floor
            case 1:
                {
                    indicatorPos.y -= spawnOffset;
                }
                break;
            //front
            case 2:
                {
                    indicatorPos.x += spawnOffset;
                }
                break;
            //back
            case 3:
                {
                    indicatorPos.x -= spawnOffset;
                }
                break;
            //left
            case 4:
                {
                    indicatorPos.z += spawnOffset;
                }
                break;
            //right
            case 5:
                {
                    indicatorPos.z -= spawnOffset;
                }
                break;
        }

        return indicatorPos;
    }


    List<GameObject> GetIndicatorSelection(List<GameObject> indicators, int wallIndex)
    {
        List<GameObject> newIndicatorList = new List<GameObject>();

        //player's position
        float playerPosX = player.transform.position.x;
        float playerPosY = player.transform.position.y;
        float playerPosZ = player.transform.position.z;


        for (int index = 0; index < indicators.Count; index++)
        {
            //finds the indicator's distance from player
            float xDistFromPlayer = Mathf.Abs(playerPosX - indicators[index].transform.position.x);
            float yDistFromPlayer = Mathf.Abs(playerPosY - indicators[index].transform.position.y);
            float zDistFromPlayer = Mathf.Abs(playerPosZ - indicators[index].transform.position.z);


            //plane axis': X & Z (Floor & Ceiling)
            if (wallIndex == 0 || wallIndex == 1)
            {


                //if the indicator is within range of the player i.e. less than the pillarOffset
                if (xDistFromPlayer < PillarOffset && zDistFromPlayer < PillarOffset)
                {
                    //Debug.Log("Pillar is within range");
                    newIndicatorList.Add(indicators[index]);
                }
            }
            //plane axis': Y & Z (Front & Back)
            else if (wallIndex == 2 || wallIndex == 3)
            {


                //if the indicator is within range of the player i.e. less than the pillarOffset
                if (yDistFromPlayer < PillarOffset && zDistFromPlayer < PillarOffset)
                {
                    //Debug.Log("Pillar is within range");
                    newIndicatorList.Add(indicators[index]);
                }
            }
            //plane axis': X & Y (Left & Right)
            else
            {


                //if the indicator is within range of the player i.e. less than the pillarOffset
                if (xDistFromPlayer < PillarOffset && yDistFromPlayer < PillarOffset)
                {
                    //Debug.Log("Pillar is within range");
                    newIndicatorList.Add(indicators[index]);
                }
            }
        }

        return newIndicatorList;
    }


    //gathers all indicators inside of wall
    List<GameObject> GetIndicators(GameObject wall)
    {


        GameObject wallIndicators = null;

        for (int index = 0; index < wall.transform.childCount; index++)
        {
            if (wall.transform.GetChild(index).CompareTag("Wall Indicator"))
            {
                //Debug.Log("child is Wall Indicator");
                wallIndicators = wall.transform.GetChild(index).gameObject;
            }

        }

        List<GameObject> indicatorRows = GetIndicatorRows(wallIndicators);

        return GetIndicatorList(indicatorRows);
    }


    //gathers all the indicator rows inside of wallIndicators
    List<GameObject> GetIndicatorRows(GameObject wallIndicators)
    {
        //Debug.Log("inside getIndicatorRows");
        List<GameObject> rows = GetGameObjectList(wallIndicators, "Indicator Row");

        //Debug.Log("wall indicator successfully get components in children: " + rows[0].tag);

        return rows;
    }

    //creates a list of all the indicators inside of wall
    List<GameObject> GetIndicatorList(List<GameObject> indicatorRows)
    {
        List<GameObject> indicators = new List<GameObject>();
        for (int index = 0; index < indicatorRows.Count; index++)
        {
            List<GameObject> temp = GetGameObjectList(indicatorRows[index], "Indicator");

            for (int i = 0; i < temp.Count; i++)
            {
                indicators.Add(temp[i]);
            }
        }
        return indicators;
    }



    //selects an indicator from the list provided
    GameObject SelectedIndicator(List<GameObject> list)
    {
        //makes sure list is not empty
        if (list.Count == 0)
        {
            return null;
        }



        int randomIndex = -1;

        //makes sure there is an indicator available
        if (!AllIndicatorsActive(list))
        {


            randomIndex = Random.Range(0, list.Count);

            GameObject indicator = list[randomIndex];
            IndicatorBehavior script = indicator.GetComponent<IndicatorBehavior>();
            if (script == null || script.isActive == true)
            {
                return null;
            }

            return list[randomIndex];
        }


        return null;
    }


    //makes sure not all indicators are being used
    bool AllIndicatorsActive(List<GameObject> list)
    {

        for (int index = 0; index < list.Count; index++)
        {
            if (!list[index].GetComponent<IndicatorBehavior>().isActive) return false;
        }

        return true;
    }




    Vector3 GetPillarRotation(int selectWall)
    {
        GameObject wall = Walls[selectWall];

        float xRotation = wall.transform.eulerAngles.x;
        float yRotation = wall.transform.eulerAngles.y;
        float zRotation = wall.transform.eulerAngles.z;

        //Debug.Log("euler Wall rotation: " + xRotation + ", " + yRotation + ", " + zRotation);

        return new Vector3(xRotation, yRotation, zRotation);
    }



    List<GameObject> GetGameObjectList(GameObject o, string tag)
    {
        List<GameObject> list = new List<GameObject>();
        for (int index = 0; index < o.transform.childCount; index++)
        {
            if (o.transform.GetChild(index).tag == tag)
            {
                //Debug.Log("object has child with " + tag + " tag");
                list.Add(o.transform.GetChild(index).gameObject);
            }
        }
        return list;
    }

    int GetRandBehavior()
    {
        int weightSum = calcWeightSum();
        int randomInt = Random.Range(0, weightSum);

        int result = -1;

        if (randomInt < weights[0])
        {
            result = 0;
        }
        else if (randomInt < summation(weights, 1))
        {
            result = 1;
        }
        else if (randomInt < summation(weights, 2))
        {
            result = 2;
        }
        else if (randomInt < summation(weights, 3))
        {
            result = 3;
        }
        else if (randomInt < summation(weights, 4))
        {
            result = 4;
        }
        else if (randomInt < summation(weights, 5))
        {
            result = 5;
        }
        else
        {
            result = 6;
        }
        return result;
    }

    int calcWeightSum()
    {
        int sum = 0;

        for (int index = 0; index < weights.Length; index++)
        {
            sum += weights[index];
        }

        return sum;
    }

    int summation(int[] values, int maxRange)
    {
        if (maxRange > values.Length) return -1;
        int sum = 0;
        for (int index = 0; index <= maxRange; index++)
        {
            sum += values[index];
        }

        return 0;
    }





    void adjustWeights()
    {

        int difficulty = 0;

        //    public int[] weights = { 0, 0, 0, 0, 0, 0, 100 };
        //default, row, quick, point, Mass, adjacent, slow

        //if game is set to easy
        switch (difficulty)
        {
            //easy: used default, quick, slow spawn types
            case 1:
                {
                    if (gameManager.roundNumber < 10)
                    {
                        weights[0] += 5;
                        weights[6] -= 5;
                    }
                    else if (gameManager.roundNumber < 20)
                    {
                        weights[0] -= 5;
                        weights[2] += 5;
                        weights[6] -= 2;
                    }
                    else
                    {
                        weights[0] += 1;
                        weights[1] += 1;
                        weights[2] += 1;
                        weights[3] += 1;
                        weights[4] += 1;
                        weights[5] += 1;
                        weights[6] += 1;
                    }
                    weightsCheck();

                }
                break;
            //medium: uses default, quick, point, slow spawn types
            case 2:
                {
                    if (gameManager.roundNumber < 10)
                    {
                        weights[0] += 5;
                        weights[2] += 2;
                        weights[3] += 1;
                        weights[6] -= 5;
                    }
                    else if (gameManager.roundNumber < 20)
                    {
                        weights[0] -= 2;
                        weights[2] += 5;
                        weights[3] += 1;
                        weights[6] -= 5;
                    }
                    else
                    {
                        weights[0] += 1;
                        weights[1] += 1;
                        weights[2] += 1;
                        weights[3] += 1;
                        weights[4] += 1;
                        weights[5] += 1;
                        weights[6] += 1;
                    }
                    weightsCheck();
                }
                break;
            //hard: uses default, row, quick, point, Mass, adjacent, slow spawn types
            case 3:
                {
                    if (gameManager.roundNumber < 10)
                    {
                        weights[0] += 2;
                        weights[1] += 1;
                        weights[2] += 2;
                        weights[3] += 5;
                        weights[5] += 1;
                        weights[6] += 1;
                    }
                    else if (gameManager.roundNumber < 20)
                    {
                        weights[0] += 1;
                        weights[1] += 1;
                        weights[2] += 2;
                        weights[3] -= 3;
                        weights[4] += 1;
                        weights[5] += 1;
                    }
                    else
                    {
                        weights[0] -= 1;
                        weights[1] += 1;
                        weights[2] += 1;
                        weights[3] += 1;
                        weights[4] += 1;
                        weights[5] += 1;
                    }
                    weightsCheck();
                }
                break;
        }
    }


    void weightsCheck()
    {

        for (int index = 0; index < weights.Length; index++)
        {
            if (weights[index] < 0) weights[index] = 0;
        }
    }








    void setWeights()
    {
        switch (difficulty)
        {
            case 1:
                {
                    weights[6] = 100;
                }
                break;
            case 2:
                {
                    weights[0] = 100;
                    weights[6] = 100;
                }
                break;
            case 3:
                {
                    weights[0] = 100;
                    weights[2] = 100;
                    weights[6] = 100;
                }
                break;
        }

        weightsCheck();
    }






    public void startRound()
    {
        InvokeRepeating("SpawnPillar", 0.0f, repeatSpawnTime);
    }

    public void stopRound()
    {
        CancelInvoke();
    }

    public void setDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
        setWeights();
    }

    public void newRound()
    {
        stopRound();
        adjustWeights();
    }





}
