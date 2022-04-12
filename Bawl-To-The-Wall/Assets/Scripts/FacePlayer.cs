using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private GameObject player;
    private GameManager gameManager;
    public AudioClip orbPickedUp;
    private AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        //audioSource = gameObject.AddComponent<AudioSource>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.Log("player is null");

    }

    // Update is called once per frame
    void Update()
    {
        //temporary
        //time += Time.deltaTime;
        //OnTime();

        if (Vector3.Magnitude(player.transform.position - transform.position) < 50)
        {
            RotateToPlayer();
        }
    }

    void RotateToPlayer()
    {
        //Vector3 playerDirection = player.transform.position - transform.position;
        //Quaternion rotation = Quaternion.LookRotation(playerDirection);
        //Quaternion rotation = Quaternion.LookRotation(player.transform.position, Vector3.up);
        transform.LookAt(player.transform);
        //transform.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Controller"))
        {
            //audioSource.PlayOneShot(orbPickedUp);
            gameManager.orbDestroyed(gameObject);
            //Destroy(gameObject);
        }
    }





}
