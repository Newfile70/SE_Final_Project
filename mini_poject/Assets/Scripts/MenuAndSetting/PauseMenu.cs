using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;//Determine whether the game is paused

    public GameObject pauseMenuUI;//Pause interface UI
    public GameObject SettingUI;//Setting UI
    public GameObject SuccessUI;
    public GameObject DeadUI;
    public GameObject FailedUI;
    public GameObject WeaponBackpackUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !SuccessUI.activeSelf && !DeadUI.activeSelf && !FailedUI.activeSelf && !WeaponBackpackUI.activeSelf)//Press P to pause or resume the game (cannot be used when UI is in Success, Dead, Failed and Weapon Library)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    void Pause()//Pause the game
    {
        pauseMenuUI.SetActive(true);//Open the pause interface UI
        Time.timeScale = 0f;//Pause the game
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()//Resume the game
    {
        pauseMenuUI.SetActive(false);//Close the pause interface UI
        GameIsPaused = false;
        Time.timeScale = 1f;//Resume the game
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadMenu()//Load the menu interface
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void LoadSetting()//Load the menu interface
    {
        Time.timeScale = 0f;//Pause the game
        SettingUI.SetActive(true);//Open the setting UI in the game
        pauseMenuUI.SetActive(false);//Close the pause interface UI
    }

    public void QuitGame()//Quit the game
    {
        //Debug.Log("Quitting game...");
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;

    }

    public void Button1()
    {
        SceneManager.LoadScene("TestScene 1", LoadSceneMode.Single);
    }

    public void Button2()
    {
        SceneManager.LoadScene("TestScene 2", LoadSceneMode.Single);
    }

    public void Button3()
    {
        SceneManager.LoadScene("TestScene 3", LoadSceneMode.Single);
    }

    public void Button4()
    {
        SceneManager.LoadScene("TestScene 4", LoadSceneMode.Single);
    }

    public void Button5()
    {
        SceneManager.LoadScene("TestScene 5", LoadSceneMode.Single);
    }


}
