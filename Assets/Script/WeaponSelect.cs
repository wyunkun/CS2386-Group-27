using UnityEngine;
using UnityEngine.UI;

public class WeaponSelect : MonoBehaviour
{
    public GameObject buttons;
    public MonoBehaviour controlScript;
    public GameObject SWA;
    public GameObject SMG;
    public GameObject LMG;
    public GameObject RLA;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowWheel();
    }

    void ShowWheel()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            buttons.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            controlScript.enabled = false;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            buttons.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            controlScript.enabled = true;
        }
    }

    public void ShowSWA()
    {
        SWA.SetActive(true);
        SMG.SetActive(false);
        LMG.SetActive(false);
        RLA.SetActive(false);
    }

    public void ShowSMG()
    {
        SWA.SetActive(false);
        SMG.SetActive(true);
        LMG.SetActive(false);
        RLA.SetActive(false);
    }

    public void ShowLMG()
    {
        SWA.SetActive(false);
        SMG.SetActive(false);
        LMG.SetActive(true);
        RLA.SetActive(false);
    }

    public void ShowRLA()
    {
        SWA.SetActive(false);
        SMG.SetActive(false);
        LMG.SetActive(false);
        RLA.SetActive(true);
    }
}
