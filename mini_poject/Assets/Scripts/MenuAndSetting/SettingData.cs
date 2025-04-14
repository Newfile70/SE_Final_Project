using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SettingData : MonoBehaviour
{
    [Header("Volume Control")]
    public Slider AudioSlider;//Volume control slider
    public Text AudioValueText;//Volume value Text

    [Header("Brightness Control")]
    public Slider LightSlider;//Brightness control slider
    public Text LightValueText;//Brightness value Text

    [Header("Mouse Sensitivity Control")]
    public Slider MouseSenstivitySlider;//Mouse sensitivity control slider
    public Text MouseSenstivityValueText;//Mouse sensitivity value Text

    [Header("Difficulty Control")]
    public Button EasyButton;//Difficulty Easy button
    public Button NormalButton;//Difficulty Normal button
    public Button HardButton;//Difficulty Hard button

    private Color EasyColor;
    private Color NormalColor;
    private Color HardColor;

    float TransparencyDefault = 0f;
    float TransparencySelected = 0.1725490196f;

    public GameObject Canvas;//Interface UI
    public GameObject pauseMenuUI;//Pause interface UI
    public GameObject SettingUI;//In-game setting UI


    void Start()
    {
        if (PlayerPrefs.GetString("Difficulties") == "")//When entering the game for the first time, set the difficulty to Normal
        {
            PlayerPrefs.SetString("Difficulties", "Normal");
        }

        Cursor.lockState = CursorLockMode.None;

        AudioSlider.value = Mathf.Round(PlayerPrefs.GetFloat("AudioValue", 60f));//Set the volume slider value
        AudioValueText.text = Mathf.Round(PlayerPrefs.GetFloat("AudioValue", 60f)).ToString();//Update volume size Text

        LightSlider.value = Mathf.Round(PlayerPrefs.GetFloat("LightValue", 60f));//Set the brightness slider value
        LightValueText.text = Mathf.Round(PlayerPrefs.GetFloat("LightValue", 60f)).ToString();//Update brightness size Text

        MouseSenstivitySlider.value = Mathf.Round(PlayerPrefs.GetFloat("mouseSenstivity", 400f));//Set the mouse sensitivity slider value
        MouseSenstivityValueText.text = Mathf.Round(PlayerPrefs.GetFloat("mouseSenstivity", 400f)).ToString();//Update mouse sensitivity size Text

        EasyColor = EasyButton.GetComponent<Button>().colors.normalColor;
        NormalColor = NormalButton.GetComponent<Button>().colors.normalColor;
        HardColor = HardButton.GetComponent<Button>().colors.normalColor;

        /* If the difficulty is easy, then EasyButton is highlighted, normal and difficult are the same */
        if (PlayerPrefs.GetString("Difficulties") == "Easy")
        {
            DifficultiesCtrlEasy();
        }
        else if (PlayerPrefs.GetString("Difficulties") == "Normal")
        {
            DifficultiesCtrlNormal();
        }
        else if (PlayerPrefs.GetString("Difficulties") == "Hard")
        {
            DifficultiesCtrlHard();
        }
    }



    public void AudioCtrl()//Volume control
    {
        PlayerPrefs.SetFloat("AudioValue", AudioSlider.value);//Save the volume size data to PlayerPrefs
        PlayerPrefs.Save();
        AudioValueText.text = Mathf.Round(PlayerPrefs.GetFloat("AudioValue")).ToString();//Update volume size Text
    }


    public void LightCtrl()//Brightness control
    {
        PlayerPrefs.SetFloat("LightValue", LightSlider.value);//Save the brightness size data to PlayerPrefs
        PlayerPrefs.Save();
        LightValueText.text = Mathf.Round(PlayerPrefs.GetFloat("LightValue")).ToString();//Update brightness size Text
    }

    public void MouseSenstivityCtrl()//Mouse sensitivity control
    {
        PlayerPrefs.SetFloat("mouseSenstivity", MouseSenstivitySlider.value);//Save the brightness size data to PlayerPrefs
        PlayerPrefs.Save();
        MouseSenstivityValueText.text = Mathf.Round(PlayerPrefs.GetFloat("mouseSenstivity")).ToString();//Update mouse sensitivity size Text
    }

    public void DifficultiesCtrlEasy()//Difficulty control, set to easy, EasyButton highlighted
    {
        TransparencyCtrl(EasyColor, EasyButton, TransparencySelected);
        TransparencyCtrl(NormalColor, NormalButton, TransparencyDefault);
        TransparencyCtrl(HardColor, HardButton, TransparencyDefault);
        PlayerPrefs.SetString("Difficulties", "Easy");
        PlayerPrefs.Save();
    }

    public void DifficultiesCtrlNormal()//Difficulty control, set to normal, NormalButton highlighted
    {
        TransparencyCtrl(EasyColor, EasyButton, TransparencyDefault);
        TransparencyCtrl(NormalColor, NormalButton, TransparencySelected);
        TransparencyCtrl(HardColor, HardButton, TransparencyDefault);
        PlayerPrefs.SetString("Difficulties", "Normal");
        PlayerPrefs.Save();
    }

    public void DifficultiesCtrlHard()//Difficulty control, set to hard, HardButton highlighted
    {
        TransparencyCtrl(EasyColor, EasyButton, TransparencyDefault);
        TransparencyCtrl(NormalColor, NormalButton, TransparencyDefault);
        TransparencyCtrl(HardColor, HardButton, TransparencySelected);
        PlayerPrefs.SetString("Difficulties", "Hard");
        PlayerPrefs.Save();
    }



    public void TransparencyCtrl(Color color, Button button, float transparency)//Change the transparency of the difficulty button
    {
        color.a = transparency;
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        button.colors = cb;
    }



    public void Return()
    {
        pauseMenuUI.SetActive(true);//Open the pause interface UI
        SettingUI.SetActive(false);//Close the in-game setting UI
    }


}

