using System.Collections;
using TMPro;
using UnityEngine;

public class EndingCutscene : MonoBehaviour
{
    [Header("Затемнение и текст")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private TMP_Text endingText;

    [TextArea(2, 5)]
    [SerializeField]
    private string endingMessage =
        "Продолжение следует...";

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float textDelay = 1f;

    [Header("Музыка")]
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private AudioSource cutsceneMusic;

    [Header("Управление игроком")]
    [SerializeField] private MonoBehaviour playerController;

    private bool cutsceneStarted;

    private void Start()
    {
        blackScreen.alpha = 0f;
        blackScreen.blocksRaycasts = false;
        endingText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cutsceneStarted || !other.CompareTag("Player"))
            return;

        cutsceneStarted = true;
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        if (playerController != null)
            playerController.enabled = false;

        if (mainMusic != null)
            mainMusic.Stop();

        if (cutsceneMusic != null)
            cutsceneMusic.Play();

        blackScreen.blocksRaycasts = true;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            blackScreen.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        blackScreen.alpha = 1f;

        yield return new WaitForSeconds(textDelay);

        endingText.text = endingMessage;
        endingText.gameObject.SetActive(true);
    }
}