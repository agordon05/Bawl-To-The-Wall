using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;
    private GameManager gameManager;
    //private float Volume;



    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = 1.0f;
        if (slider.CompareTag("Volume Slider"))
        {
            slider.onValueChanged.AddListener(volumeChanged);
        }
        else if (slider.CompareTag("Music Slider"))
        {
            slider.onValueChanged.AddListener(musicVolumeChanged);
        }

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Volume = slider.value;
    }

    void volumeChanged(float Volume)
    {
        gameManager.setVolume(Volume);
    }

    void musicVolumeChanged(float Volume)
    {
        gameManager.setMusicVolume(Volume);
    }

    //public void setVolume(float Volume)
    //{
    //    slider.value = Volume;
    //}


}
