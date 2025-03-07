using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{

    private Button button;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(exitGame);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

    }

    void exitGame()
    {
        gameManager.exitGame();
    }




}
