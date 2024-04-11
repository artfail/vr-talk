using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public AudioClip[] songs;
    private AudioSource aud;
    void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    public void PlaySong(int songNum)
    {
        aud.Stop();
        aud.clip = songs[songNum];
        aud.Play();
    }
}
