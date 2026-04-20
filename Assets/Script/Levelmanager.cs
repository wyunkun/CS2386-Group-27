using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LevelManager : MonoBehaviour
{
    public static bool IsPlaying { get; private set; }

    public GameObject winPanel;
    public GameObject losePanel;
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
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
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
        if (winSFX != null) audioSource.PlayOneShot(winSFX);
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(LoadNextLevel());
    }

    public void LevelLost()
    {
        IsPlaying = false;
        if (loseSFX != null) audioSource.PlayOneShot(loseSFX);
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(ReloadLevel());
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(0);
    }

    IEnumerator ReloadLevel()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}