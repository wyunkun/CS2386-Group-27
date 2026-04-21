using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject newGameButton;
    public GameObject continueButton;
    public GameObject settingsButton;
    public GameObject exitButton;
    public TMP_Text title;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingButtonOnClick()
    {
        newGameButton.SetActive(false);
    }
}
