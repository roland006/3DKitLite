using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_play : MonoBehaviour
{
    AudioSource aud;

    void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    public void play_sound()
    {
        aud.Play();
    }
}
