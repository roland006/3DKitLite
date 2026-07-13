using UnityEngine;
using UnityEngine.Playables;

public class FinalCutsceneStarter : MonoBehaviour
{
    public PlayableDirector director;   // перетащи сюда Director финальной катсцены
    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;
            director.Play();
            // Опционально отключаем коллайдер, чтобы не сработал повторно
            GetComponent<Collider>().enabled = false;
        }
    }
}