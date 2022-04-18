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

    public float repeatSpawnTime = 5.0f;
    public float indicatorWaitTime = 3;

    private int[] weights = { 0, 0, 0, 0, 0, 0, 0 };
    private int[] maxWeightRatio = { 150, 70, 143, 57, 57, 0, 93 };

    /****TEMPORARY****/
    private bool[] finishedSpawnTypes = { true, true, true, true, true, false, true };


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




    /***TEMPORARY***/
    //int BEHAVIOR = 4;
    //bool played = false;

    void SpawnPillar()
    {
        if (gameManager.isGameActive && !gameManager.isGameOver && !gameManager.isBetweenRound)
        {
            int behavior = GetRandBehavior();

            //if(behavior != 0)
            //    behavior = 3;

            /****TEMPORARY****/
            while (!finishedSpawnTypes[behavior])
            {
                behavior--;
                if (behavior < 0)
                {
                    behavior = 0;
                    break;
                }
            }

            //int behavior = BEHAVIOR;

            switch (behavior)
            {
                default:
                    {
                        behavior = 0;
                        defaultSpawn(behavior, 1);
                    }
                    break;
                case 1:
                    {
                        RowSpawn(behavior);
                    }
                    break;
                case 2:
                    {
                        defaultSpawn(behavior, 0.5f);
                    }
                    break;
                case 3:
                    {
                        PointSpawn(behavior);
                    }
                    break;
                case 4:
                    {
                        MassSpawn(behavior);
                    }
                    break;
                case 5:
                    {
                        AdjacentSpawn(behavior);
                    }
                    break;
                case 6:
                    {
                        defaultSpawn(behavior, 2);
                    }
                    break;

            }
        }

    }

    public void SpawnPillar(float timeAwake, GameObject indicator, int startingWall, int behavior)
    {
        StartCoroutine(IndicatorWaitTime(timeAwake, indicator, startingWall, behavior));
    }


    //behaviors 0,2,6
    //spawns a single pillar
    void defaultSpawn(int behavior, float multiplier)
    {

        //if (played) return;
        //played = true;

        //Selects Random Wall
        int wallIndex = Random.Range(0, Walls.Length); ;

        //int wallIndex = 5;

        //gets indicators that indicate where pillar will spawn at
        GameObject wall = Walls[wallIndex];
        List<GameObject> indicatorList = GetIndicators(wall);

        //finds select Positions close to player position
        //within about 35 for center Pillar (the pillar on that wall thats closest to players position
        //other select positions must be within about 53?
        indicatorList = GetIndicatorSelection(indicatorList, wallIndex);

        //selects the indicator that the pillar will spawn from
        GameObject indicator = SelectedIndicator(indicatorList);

        if (indicator == null) return;

        indicator.GetComponent<IndicatorBehavior>().setTimeAwake(indicatorWaitTime * multiplier);

        float waitTime = indicator.GetComponent<IndicatorBehavior>().timeAwake;
        if (!gameManager.isBetweenRound)
            StartCoroutine(IndicatorWaitTime(waitTime, indicator, wallIndex, behavior));

    }


    //behavior = 1
    //spawns an entire row of pillars
    void RowSpawn(int behavior)
    {
        //if (played) return;
        //played = true;

        if (behavior != 1) return;

        //selects wall for spawn
        int wallIndex = (int)Random.Range(0, Walls.Length);
        GameObject wall = Walls[wallIndex];


        //gets all indicators involved
        List<GameObject> indicatorList = GetIndicators(wall);
        GameObject wallIndicators = null;

        //gets empty object inside of wall with the wall indicator tag
        for (int index = 0; index < wall.transform.childCount; index++)
        {
            if (wall.transform.GetChild(index).CompareTag("Wall Indicator"))
            {
                //Debug.Log("child is Wall Indicator");
                wallIndicators = wall.transform.GetChild(index).gameObject;
            }
        }


        if (wallIndicators == null)
        {
            Debug.Log("no object had Wall Indicator tag");
            return;
        }

        indicatorList = GetIndicatorRows(wallIndicators);
        indicatorList = GetRowsInRange(indicatorList, wallIndex);

        int randomIndex = (int)Random.Range(0, indicatorList.Count);
        Debug.Log("rows in Range Count: " + indicatorList.Count);
        indicatorList = GetGameObjectList(indicatorList[randomIndex], "Indicator");


        //spawns pillars


        for (int index = 0; index < indicatorList.Count; index++)
        {
            IndicatorBehavior indicatorScript = indicatorList[index].GetComponent<IndicatorBehavior>();
            indicatorScript.setTimeAwake(indicatorWaitTime);
            float waitTime = indicatorScript.timeAwake;
            if (!gameManager.isBetweenRound && !indicatorScript.isActive)
                StartCoroutine(IndicatorWaitTime(waitTime, indicatorList[index], wallIndex, behavior));

        }
    }


    List<GameObject> GetRowsInRange(List<GameObject> rows, int wallIndex)
    {
        List<GameObject> validRows = new List<GameObject>();

        //player's position
        float playerPosX = player.transform.position.x;
        float playerPosY = player.transform.position.y;
        //float playerPosZ = player.transform.position.z;


        for (int index = 0; index < rows.Count; index++)
        {
            //finds the indicator's distance from player
            float xDistFromPlayer = Mathf.Abs(playerPosX - rows[index].transform.position.x);
            float yDistFromPlayer = Mathf.Abs(playerPosY - rows[index].transform.position.y);
            //float zDistFromPlayer = Mathf.Abs(playerPosZ - rows[index].transform.position.z);


            //plane axis': X & Z (Floor & Ceiling)
            if (wallIndex == 0 || wallIndex == 1)
            {


                //if the indicator is within range of the player i.e. less than the pillarOffset
                if (xDistFromPlayer < PillarOffset /*&& zDistFromPlayer < PillarOffset*/)
                {
                    //Debug.Log("Pillar is within range");
                    validRows.Add(rows[index]);
                }
            }
            //plane axis': Y & Z (Front & Back)
            else if (wallIndex == 2 || wallIndex == 3)
            {


                //if the indicator is within range of the player i.e. less than the pillarOffset
                if (yDistFromPlayer < PillarOffset /*&& zDistFromPlayer < PillarOffset*/)
                {
                    //Debug.Log("Pillar is within range");
                    validRows.Add(rows[index]);
                }
            }
            //plane axis': X & Y (Left & Right)
            else
            {


                //if the indicator is within range of the player i.e. less than the pillarOffset
                if (/*xDistFromPlayer < PillarOffset && */ yDistFromPlayer < PillarOffset)
                {
                    //Debug.Log("Pillar is within range");
                    validRows.Add(rows[index]);
                }
            }
        }

        return validRows;
    }



    //behavior = 3
    //spawns all pillars within range from every wall that converge to a single point
    void PointSpawn(int behaviour)
    {
        //if (played) return;
        //played = true;

        for (int wallIndex = 0; wallIndex < Walls.Length; wallIndex++)
        {
            GameObject wall = Walls[wallIndex];

            List<GameObject> indicatorList = GetIndicators(wall);

            indicatorList = GetIndicatorSelection(indicatorList, wallIndex);

            for (int index = 0; index < indicatorList.Count; index++)
            {
                indicatorList[index].GetComponent<IndicatorBehavior>().setTimeAwake(indicatorWaitTime);

                float waitTime = indicatorList[index].GetComponent<IndicatorBehavior>().timeAwake;
                if (!gameManager.isBetweenRound)
                    StartCoroutine(IndicatorWaitTime(waitTime, indicatorList[index], wallIndex, behaviour));
                else
                    break;

            }

            if (gameManager.isBetweenRound) break;


        }
    }

    //behavior = 4
    //spawns a bunch of pillars ignoring whether or not they're within range from a single wall and recalls all pillars in game
    void MassSpawn(int behavior)
    {

        //if (played) return;
        //played = true;

        //Selects Random Wall
        int wallIndex = Random.Range(0, Walls.Length);
        //int wallIndex = 5;


        //gets indicators that indicate where pillar will spawn at
        GameObject wall = Walls[wallIndex];
        List<GameObject> indicatorList = GetIndicators(wall);

        //finds select Positions close to player position
        //within about 35 for center Pillar (the pillar on that wall thats closest to players position
        //other select positions must be within about 53?
        //indicatorList = GetIndicatorSelection(indicatorList, wallIndex);



        //recalls all pillars in game
        GameObject[] pillarsInGame = GameObject.FindGameObjectsWithTag("Pillar");
        for (int index = 0; index < pillarsInGame.Length; index++) pillarsInGame[index].GetComponent<MovePillar>().recall();


        //gets rid of half the indicators at random
        for (int count = 0, length = indicatorList.Count; count < length / 2; count++)
        {
            int index = Random.Range(0, indicatorList.Count);
            indicatorList.RemoveAt(index);
        }

        for (int index = 0; index < indicatorList.Count; index++)
        {
            indicatorList[index].GetComponent<IndicatorBehavior>().setTimeAwake(indicatorWaitTime);

            float waitTime = indicatorList[index].GetComponent<IndicatorBehavior>().timeAwake;
            if (!gameManager.isBetweenRound)
                StartCoroutine(IndicatorWaitTime(waitTime, indicatorList[index], wallIndex, behavior));
            else
                break;

        }
    }



    //behavior = 5
    //spawns two pillars from a connecting wall
    void AdjacentSpawn(int behavior)
    {

    }




    IEnumerator IndicatorWaitTime(float waitTime, GameObject SelectedIndicator, int wallIndex, int behavior)
    {
        if (gameManager.isBetweenRound) yield return new WaitForSeconds(0);
        SelectedIndicator.GetComponent<IndicatorBehavior>().Activate(behavior);

        //Selects random Pillar position from selection
        Vector3 indicatorPos = SelectedIndicator.transform.position;

        //gathers neccesary information in order to spawn pillar
        Vector3 rotation = GetPillarRotation(wallIndex);
        //Debug.Log("Wall Index: " + wallIndex);
        yield return new WaitForSeconds(waitTime);

        if (gameManager.isGameActive && !gameManager.isGameOver && !gameManager.isBetweenRound)
        {
            //spawns pillar
            GameObject Pillar = Instantiate(pillar, GetSpawnPos(indicatorPos, wallIndex), Quaternion.Euler(rotation));
            MovePillar pillarScript = Pillar.GetComponent<MovePillar>();

            pillarScript.SpawnIndicator(SelectedIndicator);
            //if spawn type is quick
            if (behavior == 2) pillarScript.setSpeed(pillarScript.speed * 2);
            else if (behavior == 6) pillarScript.setSpeed(pillarScript.speed / 2);

        }

    }


    public Vector3 GetSpawnPos(Vector3 indicatorPos, float wallIndex)
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
        //int[] summations = { summation(weights, 0), summation(weights, 1), summation(weights, 2) , summation(weights, 3) , summation(weights, 4) , summation(weights, 5) };
        int result = 0;

        if (randomInt < summation(weights, 0))
        {
            Debug.Log("inside 0");

            result = 0;
        }
        else if (randomInt < summation(weights, 1))
        {
            Debug.Log("inside 1");

            result = 1;
        }
        else if (randomInt < summation(weights, 2))
        {
            Debug.Log("inside 2");

            result = 2;
        }
        else if (randomInt < summation(weights, 3))
        {
            Debug.Log("inside 3");
            result = 3;
        }
        else if (randomInt < summation(weights, 4))
        {
            Debug.Log("inside 4");

            result = 4;
        }
        else if (randomInt < summation(weights, 5))
        {
            Debug.Log("inside 5");

            result = 5;
        }
        else if (randomInt < summation(weights, 6))
        {
            Debug.Log("inside 6");

            result = 6;
        }
        //else result = 0;
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
        Debug.Log("Summation of " + maxRange + " is " + sum);
        return sum;
    }





    void adjustWeights()
    {

        //int difficulty = 0;

        //    public int[] weights = { 0, 0, 0, 0, 0, 0, 100 };
        //default, row, quick, point, Mass, adjacent, slow

        //if game is set to easy
        switch (difficulty)
        {
            //easy: used default, quick, slow spawn types
            default:
                {
                    if (gameManager.roundNumber < 5)
                    {
                        weights[0] += 10;
                        //weights[6] -= 5;
                    }
                    else if (gameManager.roundNumber < 10)
                    {
                        //weights[0] -= 5;
                        weights[2] += 5;
                        //weights[6] -= 2;
                    }
                    else
                    {
                        weights[0] += 2;
                        //weights[1] += 1;
                        weights[2] += 1;
                        //weights[3] += 1;
                        //weights[4] += 1;
                        //weights[5] += 1;
                        //weights[6] += 1;
                    }
                }
                break;
            //medium: uses default, quick, point, slow spawn types
            case 2:
                {
                    if (gameManager.roundNumber < 5)
                    {
                        weights[0] += 5;
                        weights[2] += 2;
                        //weights[3] += 1;
                        //weights[6] -= 5;
                    }
                    else if (gameManager.roundNumber < 10)
                    {
                        //weights[0] -= 2;
                        weights[1] += 1;
                        weights[2] += 5;
                        //weights[3] += 1;
                        //weights[6] -= 5;
                    }
                    else
                    {
                        weights[0] += 1;
                        weights[1] += 1;
                        weights[2] += 1;
                        //weights[3] += 1;
                        //weights[4] += 1;
                        //weights[5] += 1;
                        weights[6] += 1;
                    }
                }
                break;
            //hard: uses default, row, quick, point, Mass, adjacent, slow spawn types
            case 3:
                {
                    if (gameManager.roundNumber < 5)
                    {
                        weights[0] += 2;
                        //weights[1] += 1;
                        weights[2] += 3;
                        //weights[3] += 2;
                        //weights[5] += 1;
                        weights[6] += 1;
                    }
                    else if (gameManager.roundNumber < 10)
                    {
                        weights[0] += 1;
                        weights[1] += 5;
                        weights[2] += 5;
                        //weights[3] += 3;
                        //weights[4] += 5;
                        //weights[5] += 5;
                        weights[6] += 5;
                    }
                    else
                    {
                        weights[0] += 1;
                        weights[1] += 1;
                        weights[2] += 1;
                        weights[3] += 3;
                        weights[4] += 3;
                        //weights[5] += 1;
                        weights[6] += 1;
                    }
                }
                break;
        }
        weightsCheck();
    }


    void weightsCheck()
    {

        for (int index = 0; index < weights.Length; index++)
        {
            if (weights[index] > maxWeightRatio[index]) weights[index] = maxWeightRatio[index];
        }
    }





    /*
     * spawn types
     * 0 = default
     * 1 = row
     * 2 = quick
     * 3 = point
     * 4 = mass
     * 5 = adjacent
     * 6 = slow
     * 
     * 
     * 
     * 
     */


    void setWeights()
    {
        switch (difficulty)
        {
            default:
                {
                    weights[6] = 100;

                }
                break;
            case 2:
                {
                    weights[0] = 100;
                    weights[6] = 50;

                }
                break;
            case 3:
                {
                    weights[0] = 100;

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


        if (difficulty == 1)
        {
            repeatSpawnTime = 3.5f;
            indicatorWaitTime = 4;
        }
        if (difficulty == 2)
        {
            repeatSpawnTime = 3;
            indicatorWaitTime = 3.5f;

        }
        if (difficulty == 3)
        {
            repeatSpawnTime = 2.5f;
            indicatorWaitTime = 3;

        }


        setWeights();
    }

    public void newRound()
    {
        stopRound();
        adjustWeights();
    }





}
