using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip SoundClip;
    public bool IsMusic;
    public bool IsOnce;
    bool WasStarted;
    void OnTriggerEnter(Collider other)
    {

        if (!WasStarted || !IsOnce)
        {
            WasStarted = true;

            if (IsMusic)
            {
                AudioSource.clip = SoundClip;
                AudioSource.loop = true;
                AudioSource.Play();
            }
            else
            {
                AudioSource.PlayOneShot(SoundClip);
            }
        }




    }


}
