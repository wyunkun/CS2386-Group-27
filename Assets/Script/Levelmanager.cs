using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LevelManager : MonoBehaviour
{
    public static bool IsPlaying { get; private set; }

    [Header("UI")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Audio")]
    public AudioClip winSFX;
    public AudioClip loseSFX;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        IsPlaying = true;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    void Update()
    {
        if (IsPlaying && !PlayerHealth.isAlive)
        {
            IsPlaying = false;
            LevelLost();
        }
    }

    public void LevelBeat()
    {
        IsPlaying = false;
        PlaySoundClip(winSFX);

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
        StartCoroutine(ReloadAfterDelay());
    }

    public void LevelLost()
    {
        IsPlaying = false;
        PlaySoundClip(loseSFX);

        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;
        StartCoroutine(ReloadAfterDelay());
    }

    IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void PlaySoundClip(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}