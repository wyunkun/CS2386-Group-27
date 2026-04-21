using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class LevelManager : MonoBehaviour
{
    public static bool IsPlaying { get; private set; }

    public GameObject winPanel;
    public GameObject losePanel;
    public AudioClip winSFX;
    public AudioClip loseSFX;

    [Header("Main Menu Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("InGame Buttons")]
    public Button resumeButton;
    public Button inGameSettingsButton;
    public Button backToMainButton;

    [Header("Settings Menu")]
    private AudioSource audioSource;
    private int count;
    private int playCount;

    void Awake()
    {
        bool isMainMenu = SceneManager.GetActiveScene().buildIndex == 0;
        IsPlaying = !isMainMenu;
        count = PlayerPrefs.GetInt("LevelIndex", 0);

        playCount = PlayerPrefs.GetInt("PlayCount", 0) + 1;
        PlayerPrefs.SetInt("PlayCount", playCount);
        PlayerPrefs.Save(); 

        audioSource = GetComponent<AudioSource>();

    }

    void Start()
    {
        
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        if (!PlayerPrefs.HasKey("LevelIndex"))
            continueButton.interactable = false;
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
        {
            PlayerPrefs.SetInt("LevelIndex", next);
            PlayerPrefs.Save();
            SceneManager.LoadScene(next);
        }
        else
        {
            PlayerPrefs.DeleteKey("LevelIndex");
            PlayerPrefs.Save();
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator ReloadLevel()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowSettingMenu()
    {
        
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("LevelIndex", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1); 
    }

    public void ContinueGame()
    {
        int savedLevel = PlayerPrefs.GetInt("LevelIndex", 1);
        SceneManager.LoadScene(savedLevel);
    }

    public void ExitGame()
    {
        
    }
}