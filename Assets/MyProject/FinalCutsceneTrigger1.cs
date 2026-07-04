using UnityEngine;
using UnityEngine.Playables;

public class FinalCutsceneTrigger1 : MonoBehaviour
{
    public PlayableDirector director;
    private bool hasPlayed;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered final trigger: " + other.gameObject.name);

        if (hasPlayed)
            return;

        if (other.GetComponentInParent<Gamekit3D.PlayerController>() == null)
            return;

        if (director == null)
        {
            Debug.LogError("FinalCutscene Director is not assigned");
            return;
        }

        hasPlayed = true;
        Debug.Log("Final cutscene started");
        director.Play();
    }
}