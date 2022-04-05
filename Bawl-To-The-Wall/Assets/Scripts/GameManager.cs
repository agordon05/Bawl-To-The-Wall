using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool isGameActive;
    public bool isGameOver;
    public int roundNumber = 0;

    public Canvas MainCanvas;
    public Canvas playerCanvas;

    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject settingsMenu;

    public GameObject gameOverText;
    public GameObject roundCountText;
    public GameObject youSurvivedText;
    public GameObject returnToMenu;

    public GameObject spawnManager;


    public float volume = 1.0f;
    public float musicVolume = 1.0f;








    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Round: " + roundNumber);
        isGameActive = false;
        isGameOver = false;
        Debug.Log("Game Over: " + "false");


        MainCanvas.gameObject.SetActive(true);
        playerCanvas.gameObject.SetActive(false);

        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);


        roundCountText.SetActive(false);
        youSurvivedText.SetActive(false);
        gameOverText.SetActive(false);
        returnToMenu.SetActive(false);

        startGame(1);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void playButtonPressed()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }
    public void SettingsButtonPressed()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    public void BackButtonPressed()
    {
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }





    public void setVolume(float volumeLevel)
    {
        Debug.Log("Volume: " + volumeLevel);
        if (volumeLevel < 0) volumeLevel = 0;
        else if (volumeLevel > 1) volumeLevel = 1;
        volume = volumeLevel;
    }
    public void setMusicVolume(float volumeLevel)
    {
        Debug.Log("Music Volume: " + volumeLevel);
        if (volumeLevel < 0) volumeLevel = 0;
        else if (volumeLevel > 1) volumeLevel = 1;
        musicVolume = volumeLevel;
    }




    public void gameOver()
    {


        roundCountText.SetActive(true);
        youSurvivedText.SetActive(true);
        gameOverText.SetActive(true);
        returnToMenu.SetActive(true);

        Debug.Log("Game Over: " + "true");
        isGameActive = false;
        isGameOver = true;
        youSurvivedText.GetComponent<TextMeshProUGUI>().text = "You Survived " + roundNumber + " Rounds";

        StartCoroutine(waitToRestart());

    }



    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }


    public void startGame(int difficulty)
    {
        //MainCanvas.gameObject.SetActive(false);
        //playerCanvas.gameObject.SetActive(true);
        //gameOverText.SetActive(false);
        //youSurvivedText.SetActive(false);
        //roundCountText.SetActive(true);
        //roundCountText.GetComponent<TextMeshPro>().text = "" + roundNumber;
        //Debug.Log("start game called");

        MainCanvas.gameObject.SetActive(false);
        playerCanvas.gameObject.SetActive(true);

        //Debug.Log("canvas switched");
        //mainMenu.SetActive(false);
        //playMenu.SetActive(false);
        //settingsMenu.SetActive(false);

        roundCountText.SetActive(true);
        youSurvivedText.SetActive(false);
        gameOverText.SetActive(false);
        returnToMenu.SetActive(false);


        //Debug.Log("texts have successfully switched");




        if (difficulty == 1)
            Debug.Log("Difficulty: Easy");
        else if (difficulty == 2)
            Debug.Log("Difficulty: Medium");
        else
            Debug.Log("Difficulty: Hard");



        isGameActive = true;
        Debug.Log("isGameActive: true");
        SpawnPillars pillarSpawnScript = spawnManager.GetComponent<SpawnPillars>();

        pillarSpawnScript.setDifficulty(difficulty);

        pillarSpawnScript.startRound();


    }


    public void exitGame()
    {
        Application.Quit(0);
    }


    public void newRound()
    {
        roundNumber++;
        roundCountText.GetComponent<TextMeshPro>().text = "" + roundNumber;
        //call spawnPillar adjustWeights
        //call indicators to adjust wait times
        //adust round text
        //start new round

    }




    IEnumerator waitToRestart()
    {
        Debug.Log("inside waitToRestart");
        float time = 5;
        TextMeshProUGUI textBox = returnToMenu.GetComponent<TMPro.TextMeshProUGUI>();
        Debug.Log("textBox change: before");
        if (textBox == null) Debug.Log("textBox is null");
        textBox.SetText("Returning to Main Menu:\n\n" + time);
        //returnToMenu.SetActive(true);
        Debug.Log("textBox change: after");


        while (time > 0)
        {
            Debug.Log("Return To Menu: " + time);
            yield return new WaitForSeconds(1);
            time--;
            textBox.SetText("Returning to Main Menu:\n\n" + time);
        }
        restartGame();
    }







}
