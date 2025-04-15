using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public GameObject MenuUI;//Pause interface UI
    public GameObject SettingUI;//Setting UI
    public GameObject BattleModeUI;//BattleMode UI

    // Update is called once per frame
    private void Start()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }
    void Update()
    {
    }


    public void LoadSetting()//Load settings
    {
        SettingUI.SetActive(true);
        MenuUI.SetActive(false);
    }

    public void LoadTestScene()//Load BattleMode UI
    {
        BattleModeUI.SetActive(true);
        MenuUI.SetActive(false);
        //SceneManager.LoadScene("BattleMode", LoadSceneMode.Single);
    }

    public void QuitGame()//Quit game
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }


}
