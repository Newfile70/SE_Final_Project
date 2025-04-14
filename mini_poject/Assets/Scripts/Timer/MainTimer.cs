using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTimer : MonoBehaviour
{
    public Text timerText;//Timer Text
    public float leftTime;//Remaining total time (s)
    private int leftMinutes;//Minutes part of the remaining time
    private int leftSeconds;//Seconds part of the remaining time
    private string leftMinutesText;
    private string leftSecondText;
    public GameObject FailedUI;
    public GameObject WeaponBackpackUI;
    public GameObject timerTextGameObject;

    // Start is called before the first frame update
    void Start()
    {
        //leftTime = 30f;//Total limited time is 180s
    }

    // Update is called once per frame
    void Update()
    {
        leftTime -= Time.deltaTime;
        leftMinutes = (int)Mathf.Round(leftTime) / 60;//Calculate the minutes part of the remaining time
        leftSeconds = (int)Mathf.Round(leftTime) % 60;//Calculate the seconds part of the remaining time
        leftMinutesText = leftMinutes.ToString();//Convert to string type
        leftSecondText = leftSeconds.ToString();

        if (leftMinutes < 10f)//Solve the format problem when the clock and seconds are single digits
        {
            leftMinutesText = "0" + leftMinutesText;
        }
        if (leftSeconds < 10f)
        {
            leftSecondText = "0" + leftSecondText;
        }


        if (leftTime <= 20f)//The timer turns red when there is 20s left
        {
            timerText.color = Color.red;
        }

        /* The timer starts to flash when there is 10s left, flashing once every second */
        if ((leftTime <= 10f && leftTime > 9.5f) || (leftTime <= 9f && leftTime > 8.5f) || (leftTime <= 8f && leftTime > 7.5f)
            || (leftTime <= 7f && leftTime > 6.5f) || (leftTime <= 6f && leftTime > 5.5f) || (leftTime <= 5f && leftTime > 4.5f)
            || (leftTime <= 4f && leftTime > 3.5f) || (leftTime <= 3f && leftTime > 2.5f) || (leftTime <= 2f && leftTime > 1.5f)
            || (leftTime <= 1f && leftTime > 0.5f) || leftTime <= 0f)
        {
            timerTextGameObject.SetActive(true);
        }
        if ((leftTime <= 9.5f && leftTime > 9f) || (leftTime <= 8.5f && leftTime > 8f) || (leftTime <= 7.5f && leftTime > 7f)
             || (leftTime <= 6.5f && leftTime > 6f) || (leftTime <= 5.5f && leftTime > 5f) || (leftTime <= 4.5f && leftTime > 4f)
             || (leftTime <= 3.5f && leftTime > 3f) || (leftTime <= 2.5f && leftTime > 2f) || (leftTime <= 1.5f && leftTime > 1f)
             || (leftTime <= 0.5f && leftTime > 0f))
        {
            timerTextGameObject.SetActive(false);
        }


        if (leftTime >= 0f)//Update timer Text when there is remaining time
        {
            timerText.text = leftMinutesText + ":" + leftSecondText;
        }

        if (leftTime <= -5f)//End the game 5 seconds after the total time limit
        {
            FailedUI.SetActive(true);
            WeaponBackpackUI.SetActive(false);
            Time.timeScale = 0f;//Pause the game
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
