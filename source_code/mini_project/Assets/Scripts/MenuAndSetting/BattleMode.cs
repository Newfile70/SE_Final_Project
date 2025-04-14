using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleMode : MonoBehaviour
{
    public GameObject BattleModeUI;
    public GameObject MenuUI;
    public List<GameObject> battleImages = new List<GameObject>();//Scene preview image
    public List<GameObject> coverImages = new List<GameObject>();//Gray cover image
    public List<GameObject> chooseBattleButton = new List<GameObject>();//Select button
    public List<GameObject> unlockText = new List<GameObject>();//Unlocked text
    private int battle;


    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.SetInt("Battle", 1);
        Debug.Log(PlayerPrefs.GetInt("Battle"));
        battle = PlayerPrefs.GetInt("Battle");
        for (int i = 0; i < battle + 1; i++)
        {
            battleImages[i].SetActive(true);
            coverImages[i].SetActive(false);
            chooseBattleButton[i].SetActive(true);
            unlockText[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void Return()
    {
        MenuUI.SetActive(true);
        BattleModeUI.SetActive(false);
    }

}
