using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOrbs : MonoBehaviour
{
    public GameObject Orb;
    private GameManager gameManager;

    private int xBound = 170; //(-170, 170)
    private int yLowerBound = 20; //(20,340)
    private int yUpperBound = 340;
    private int zBound = 170; //(-170,170)

    //// Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void newRound(int numOfSpawns)
    {
        for (int n = numOfSpawns; n > 0; n--)
        {
            Vector3 spawnPos = randomSpawnPos();

            Instantiate(Orb, spawnPos, Orb.transform.rotation);
        }
        //spawnCountCheck();
    }


    //public void spawnCountCheck()
    //{
    //    if(GameObject.FindGameObjectsWithTag("Orb").Length > gameManager.orbsPerRound)
    //    {
    //        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
    //        for (int index = 0; index < orbs.Length; index++) Destroy(orbs[index]);
    //        newRound(gameManager.orbsPerRound);
    //    }
    //}




    Vector3 randomSpawnPos()
    {
        float xPos = Random.Range(-xBound, xBound);
        float yPos = Random.Range(yLowerBound, yUpperBound);
        float zPos = Random.Range(-zBound, zBound);

        Vector3 spawnPos = new Vector3(xPos, yPos, zPos);

        return spawnPos;
    }


}
