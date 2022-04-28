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
    public bool isBetweenRound;


    private int roundNumber = 0;
    private int difficulty;

    public Canvas MainCanvas;
    public Canvas playerCanvas;

    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject settingsMenu;

    public GameObject gameOverText;
    public GameObject roundCountText;
    public GameObject youSurvivedText;
    public GameObject returnToMenu;
    public GameObject newRoundText;

    public GameObject spawnManager;


    private float volume = 1.0f;
    private float musicVolume = 1.0f;

    private int orbCount = 0;
    private int orbsPerRound = 0;
    private int maxOrbsPerRound = 30;


    public GameObject orbCounterText;


    private SpawnPillars pillarSpawnScript;
    private SpawnOrbs spawnOrbsScript;

    public AudioClip buttonHoverSound;
    public AudioClip buttonPressedSound;
    public AudioClip backButtonPressedSound;
    public AudioClip roundCompleteSound;
    public AudioClip newRoundBeginsSound;
    public AudioClip orbPickedUp;
    public AudioClip gameOverSound;
    

    public GameObject musicManager;
    private MusicPlayer musicPlayer;


    private AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        

        pillarSpawnScript = spawnManager.GetComponent<SpawnPillars>();
        spawnOrbsScript = spawnManager.GetComponent<SpawnOrbs>();
        musicPlayer = musicManager.GetComponent<MusicPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();

        //sets music volume at start
        if (musicPlayer != null)
        {
            musicPlayer.setVolume(musicVolume);
        }


        //Debug.Log("Round: " + roundNumber);
        isGameActive = false;
        isGameOver = false;
        isBetweenRound = false;
        //Debug.Log("Game Over: " + "false");


        MainCanvas.gameObject.SetActive(true);
        playerCanvas.gameObject.SetActive(false);

        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);


        roundCountText.SetActive(false);
        youSurvivedText.SetActive(false);
        gameOverText.SetActive(false);
        returnToMenu.SetActive(false);
        orbCounterText.SetActive(false);
        newRoundText.SetActive(false);

        //startGame(2);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Orbs in play: " + GameObject.FindGameObjectsWithTag("Orb").Length);
        if (orbCount != GameObject.FindGameObjectsWithTag("Orb").Length) updateOrbCounter();

        GameObject[] particleList = GameObject.FindGameObjectsWithTag("Particles");
        for (int index = 0; index < particleList.Length; index++)
        {
            ParticleSystem particle = particleList[index].GetComponent<ParticleSystem>();
            if (particle.particleCount == 0 && !particle.isEmitting) Destroy(particleList[index]);
        }

        //causes massively fun bug
        //updateOrbCounter();
    }








    public void playButtonPressed()
    {
        audioSource.PlayOneShot(buttonPressedSound);
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void SettingsButtonPressed()
    {
        audioSource.PlayOneShot(buttonPressedSound);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void BackButtonPressed()
    {
        audioSource.PlayOneShot(backButtonPressedSound);
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
        audioSource.volume = volume;
    }
    public void setMusicVolume(float volumeLevel)
    {
        Debug.Log("Music Volume: " + volumeLevel);
        if (volumeLevel < 0) volumeLevel = 0;
        else if (volumeLevel > 1) volumeLevel = 1;
        musicVolume = volumeLevel;
        musicPlayer.setVolume(musicVolume);
    }
    public float getMusicVolume()
    {
        return musicVolume;
    }
    public float getVolume()
    {
        return volume;
    }









    public void gameOver()
    {
        roundCountText.SetActive(true);
        youSurvivedText.SetActive(true);
        gameOverText.SetActive(true);
        returnToMenu.SetActive(true);
        orbCounterText.SetActive(false);
        newRoundText.SetActive(false);

        audioSource.PlayOneShot(gameOverSound);

        //Debug.Log("Game Over: " + "true");
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
        audioSource.PlayOneShot(buttonPressedSound);


        //switches to necessary canvases and texts
        MainCanvas.gameObject.SetActive(false);
        playerCanvas.gameObject.SetActive(true);

        roundCountText.SetActive(true);
        youSurvivedText.SetActive(false);
        gameOverText.SetActive(false);
        returnToMenu.SetActive(false);
        orbCounterText.SetActive(true);
        newRoundText.SetActive(false);


        //adjusts values depending on difficulty
        if (difficulty == 1)
        {
            //Debug.Log("Difficulty: Easy");
            this.difficulty = 1;
            orbsPerRound = 1;
        }
        else if (difficulty == 2)
        {
            //Debug.Log("Difficulty: Medium");
            this.difficulty = 2;

            orbsPerRound = 3;
        }
        else
        {
            //Debug.Log("Difficulty: Hard");
            this.difficulty = 3;
            orbsPerRound = 5;
        }





        updateRoundCount();
        isGameActive = true;

        Debug.Log("isGameActive: true");

        //begins new round
        spawnOrbsScript.newRound(orbsPerRound);
        pillarSpawnScript.setDifficulty(difficulty);
        pillarSpawnScript.startRound();
        if (GameObject.FindGameObjectsWithTag("Orb").Length == 0) Debug.Log("Orbs have either not spawned or findgameobjects does not find objects set unactive");

        updateOrbCounter();


    }


    public void exitGame()
    {
        Application.Quit(0);
    }











    public void newRound()
    {
        DestroyWarningPillars();
        if (isGameOver == false)
        {
            audioSource.PlayOneShot(roundCompleteSound);
            orbCounterText.SetActive(false);
            pillarSpawnScript.newRound();
            if (orbsPerRound < maxOrbsPerRound) orbsPerRound++;
            StartCoroutine(betweenRounds());
        }

    }

    private IEnumerator betweenRounds()
    {
        isBetweenRound = true;

        updateRoundCount();

        newRoundText.SetActive(true);

        yield return new WaitForSeconds(5);

        newRoundText.SetActive(false);

        yield return new WaitForSeconds(5);




        spawnOrbsScript.newRound(orbsPerRound);

        updateOrbCounter();
        orbCounterText.SetActive(true);
        pillarSpawnScript.startRound();
        isBetweenRound = false;
        audioSource.PlayOneShot(newRoundBeginsSound);
    }


    private IEnumerator waitToRestart()
    {
        DestroyWarningPillars();
        float time = 5;
        TextMeshProUGUI textBox = returnToMenu.GetComponent<TMPro.TextMeshProUGUI>();
        if (textBox == null) Debug.Log("textBox is null");
        textBox.SetText("Returning to Main Menu:\n\n" + time);
        //Debug.Log("textBox change: after");

        while (time > 0)
        {
            //Debug.Log("Return To Menu: " + time);
            yield return new WaitForSeconds(1);
            time--;
            textBox.SetText("Returning to Main Menu:\n\n" + time);
        }
        restartGame();
    }









    public void orbDestroyed(GameObject orbObject)
    {
        Destroy(orbObject);
        audioSource.PlayOneShot(orbPickedUp);

        updateOrbCounter();

    }

    private void updateRoundCount()
    {
        roundNumber++;
        TextMeshProUGUI roundCounter = roundCountText.GetComponent<TMPro.TextMeshProUGUI>();
        if (roundCounter == null) Debug.Log("round Counter does not exist");
        roundCounter.SetText("" + roundNumber);

        TextMeshProUGUI newRoundTextBox = newRoundText.GetComponent<TMPro.TextMeshProUGUI>();
        if (newRoundTextBox == null) Debug.Log("round Counter does not exist");
        newRoundTextBox.SetText("Round " + roundNumber);
    }



    private void updateOrbCounter()
    {
        GameObject[] currentOrbs = GameObject.FindGameObjectsWithTag("Orb");
        orbCount = currentOrbs.Length;

        orbCounterText.GetComponent<TextMeshProUGUI>().SetText("Orbs Left: " + orbCount + "/" + orbsPerRound);
        //StartCoroutine(orbCounterCheck(orbCount));
        if (orbCount > orbsPerRound)
        {
            spawnOrbsScript.newRound(orbsPerRound);
            for (int index = 0; index < currentOrbs.Length; index++) Destroy(currentOrbs[index]);
            updateOrbCounter();
        }
        else if (orbCount <= 0)
        {
            newRound();
        }



    }

    private void DestroyWarningPillars()
    {
        GameObject[] warningPillars = GameObject.FindGameObjectsWithTag("Warning Pillar");

        for(int index = 0; index < warningPillars.Length; index++)
        {
            Destroy(warningPillars[index]);
        }
    }



    public int getDifficulty()
    {
        return difficulty;
    }


    public int getRoundNumber()
    {
        return roundNumber;
    }

}
