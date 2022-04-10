using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public List<AudioClip> songList;
    private AudioSource audioSource;

    private List<bool> songsAllowed;

    private float volume;

    private List<AudioClip> playlist;
    private int playlistIndex = 0;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        songsAllowed = new List<bool>();
        for (int index = 0; index < songList.Count; index++) songsAllowed.Add(false);

        allSongs(true);
        createPlaylist();
        startPlaylist();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource == null) return;
        if (!audioSource.isPlaying)
        {
            nextSong();
        }
    }

    private void nextSong()
    {
        if (playlist == null) return;
        playlistIndex++;
        if (playlistIndex >= playlist.Count)
        {
            createPlaylist();
            startPlaylist();
            return;
        }
        if (isSongAllowed(playlist[playlistIndex].name))
        {
            playSong(playlist[playlistIndex].name);
            Debug.Log("song playing: " + playlist[playlistIndex].name);
        }
        else
        {
            nextSong();
        }
    }


    private bool isSongAllowed(string name)
    {
        if (songsAllowed == null) return false;
        for (int index = 0; index < songsAllowed.Count; index++)
        {
            if (songList[index].name == name) return songsAllowed[index];
        }
        return false;
    }



    private AudioClip getSong(string song)
    {
        for (int index = 0; index < songList.Count; index++)
        {
            if (songList[index].name == song) return songList[index];
        }
        return null;
    }



    //creates a randomized playlist of the songs given
    private void createPlaylist()
    {
        if (playlist == null) playlist = new List<AudioClip>();
        playlistIndex = 0;

        //all are initialized as false
        List<bool> songsPicked = new List<bool>();
        for (int index = 0; index < songList.Count; index++) songsPicked.Add(false);

        //continues until all songs are picked for playlist
        for (int index = 0; index < songsPicked.Count;)
        {
            //incase there is only one song left, to make it slightly more efficient
            if (oneSongLeft(songsPicked))
            {
                //finds the song thats left and adds it to the playlist
                for (int song = 0; song < songsPicked.Count; song++)
                {
                    if (songsPicked[song] == false)
                    {
                        playlist.Add(songList[song]);
                        return;
                    }
                }
            }
            //else Debug.Log("more than one song left");


            int random = Random.Range(0, songsPicked.Count);
            if (songsPicked[random] == false)
            {
                playlist.Add(songList[random]);
                songsPicked[random] = true;
                index++;
            }
        }
    }



    //checks to see if there is only one song left from the given boolean list
    private bool oneSongLeft(List<bool> songsPicked)
    {
        int count = 0;
        for (int index = 0; index < songsPicked.Count; index++)
        {
            if (songsPicked[index] == false) count++;
            if (count > 1) return false;
        }
        if (count == 1) return true;
        return false;
    }



    //starts playing the songs in the playlist
    private void startPlaylist()
    {
        if (playlist != null)
            audioSource.PlayOneShot(playlist[playlistIndex++]);
        else Debug.Log("startPlaylist() \t playlist is null");
        Debug.Log("song playing: " + playlist[playlistIndex - 1].name);
    }

    public void playSong(string songName)
    {
        AudioClip song = getSong(songName);
        if (song != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(song);
        }
    }

    //decides whether a song will play
    public bool getSongBool(string song)
    {
        for (int index = 0; index < songsAllowed.Count; index++)
        {
            if (songList[index].name == song) return songsAllowed[index];
        }

        return false;
    }
    public void setSongBool(string song, bool boolean)
    {
        for (int index = 0; index < songsAllowed.Count; index++)
        {
            if (songList[index].name == song) songsAllowed[index] = boolean;
        }
    }


    public void allSongs(bool boolean)
    {

        for (int index = 0; index < songsAllowed.Count; index++)
        {
            songsAllowed[index] = boolean;
        }
    }

    public List<string> getAllSongNames()
    {
        List<string> songNames = new List<string>();

        for (int index = 0; index < songList.Count; index++)
        {
            songNames.Add(songList[index].name);
        }

        return songNames;
    }
    public List<bool> getAllSongsBool()
    {
        return songsAllowed;
    }

    public float getVolume()
    {
        return volume;
    }

    public void setVolume(float vol)
    {
        if (vol < 0) vol = 0;
        else if (vol > 1) vol = 1;
        volume = vol;
        audioSource.volume = volume;

    }




}