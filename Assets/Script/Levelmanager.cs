using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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
    public Slider sensitivitySlider;
    public float defaultSensitivity = 2f;
    public TMP_Text sensitivityText;
    public TMP_Text playCountText;
    public Slider volumeSlider;
    public TMP_Text volumeText;
    public GameObject settingsPanel;

    [Header("InGame Settings Panel")]
    public GameObject inGameSettingsPanel;
    public Slider inGameSensitivitySlider;
    public TMP_Text inGameSensitivityText;
    public Slider inGameVolumeSlider;
    public TMP_Text inGameVolumeText;

    private AudioSource audioSource;
    private int count;
    private int playCount;

    void Awake()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1f);
        bool isMainMenu = SceneManager.GetActiveScene().buildIndex == 0;
        IsPlaying = !isMainMenu;
        count = PlayerPrefs.GetInt("LevelIndex", 0);

        playCount = PlayerPrefs.GetInt("PlayCount", 0) + 1;
        PlayerPrefs.SetInt("PlayCount", playCount);
        PlayerPrefs.Save(); 

        audioSource = GetComponent<AudioSource>();
        Debug.Log("LevelManager Awake executed");

    }

    void Start()
    {
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(ShowSettingMenu);
            Debug.Log("Settings button listener added");
        }
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        //if (settingsPanel != null) settingsPanel.SetActive(false);
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            UpdateVolumeText(volumeSlider.value);
        }
        if (playCountText != null)
            playCountText.text = "You have played this game: " + playCount;
        //if (continueButton != null && !PlayerPrefs.HasKey("LevelIndex"))
            //continueButton.interactable = false;
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            UpdateSensitivityText(sensitivitySlider.value);
        }
        if (inGameSettingsPanel != null) inGameSettingsPanel.SetActive(false);

        if (inGameSensitivitySlider != null)
        {
            inGameSensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);
            inGameSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            UpdateInGameSensitivityText(inGameSensitivitySlider.value);
        }

        if (inGameVolumeSlider != null)
        {
            inGameVolumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
            inGameVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            UpdateInGameVolumeText(inGameVolumeSlider.value);
        }
    }

    void Update()
    {
        if (IsPlaying && !PlayerHealth.isAlive)
        {
            IsPlaying = false;
            LevelLost();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowInGameSettings();
        }
    }
    void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
            sensitivityText.text = value.ToString();
    }

    public void ShowSettingMenu()
    {   
        Debug.Log("ShowSettingMenu called");
        settingsPanel.SetActive(true);
        newGameButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }

    public void HideSettingMenu()
    {
        settingsPanel.SetActive(false);
        newGameButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        PlayerPrefs.Save();
        UpdateSensitivityText(value);
        UpdateInGameSensitivityText(value);

    
        if (sensitivitySlider != null && sensitivitySlider.value != value)
            sensitivitySlider.value = value;
        if (inGameSensitivitySlider != null && inGameSensitivitySlider.value != value)
            inGameSensitivitySlider.value = value;
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
        UpdateVolumeText(value);
        UpdateInGameVolumeText(value);

        if (volumeSlider != null && volumeSlider.value != value)
            volumeSlider.value = value;
        if (inGameVolumeSlider != null && inGameVolumeSlider.value != value)
            inGameVolumeSlider.value = value;
    }

    void UpdateInGameSensitivityText(float value)
    {
        if (inGameSensitivityText != null)
            inGameSensitivityText.text = value.ToString("F1");
    }

    void UpdateInGameVolumeText(float value)
    {
        if (inGameVolumeText != null)
        inGameVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }
    void UpdateVolumeText(float value)
    {
        if (volumeText != null)
            volumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void ShowInGameSettings()
    {
        inGameSettingsPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) 
            pc.enabled = false;
    }

    public void HideInGameSettings()
    {   
        Debug.Log("HideInGameSettings called");
        inGameSettingsPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null)
            pc.enabled = true;
    }
}