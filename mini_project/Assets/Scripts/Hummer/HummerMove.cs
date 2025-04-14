using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HummerMove : MonoBehaviour
{
    public NavMeshAgent agent;
    Vector3 targetPosition;//Target point
    public GameObject wayPointObj;//Store different routes of enemies
    public List<Vector3> wayPoints = new List<Vector3>();//Store each patrol point of the patrol route
    private int index;//Route subscript value
    public float maxPlayerHealth;//Player life value
    private float playerHealth;//Player life value
    public bool playerIsDead;//Judge whether the player is dead



    public GameObject DeadToMenu;//Return to the main menu button after death
    public GameObject SuccessUI;//Successful interface UI

    public Text playerHealthUIText;
    public Slider playerHealthUIBar;
    public AudioSource audioSource;

    void Awake()
    {
        Application.targetFrameRate = 90;//Set the frame rate to 90
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        agent = GetComponent<NavMeshAgent>();
        playerHealth = maxPlayerHealth;
        playerHealthUIBar.maxValue = maxPlayerHealth;
        index = 0;
        LoadPath(wayPointObj);//Load path points
        DynamicGI.UpdateEnvironment();//Update the lighting environment to prevent it from getting dark when switching scenes
        //agent.updateRotation = false;//Turn off automatic rotation
        Time.timeScale = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(agent.velocity);
        if (playerIsDead)//Stop all movement behavior after the player dies
        {
            audioSource.Pause();
            return;
        }

        float distance = Vector3.Distance(transform.position, wayPoints[index]);//Calculate the distance between the Hummer and the navigation point
        if (distance <= 3f)//When the distance is very small, it means that it has reached the navigation point
        {
            index++;//Set the next navigation point
            index = Mathf.Clamp(index, 0, wayPoints.Count - 1);//Limit the size of index to prevent overflow
            if (Vector3.Distance(transform.position, wayPoints[wayPoints.Count - 1]) <= 0.5f)//Judge the distance between the enemy and the last navigation point on the patrol route. If the distance is very small, then the current route has been completed, reset the navigation point subscript, and let the enemy repeat the patrol route
            {
                index = 0;
            }
        }
        targetPosition = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);//Set the moving position and speed of the target point
        agent.destination = targetPosition;
        //transform.LookAt(wayPoints[index]);//Make the player always face the target point

        audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.25f;//Update the volume size

        if (index >= 10)
        {
            agent.speed = 100f;
        }

        if (index >= 26)
        {
            Time.timeScale = 0f;
            SuccessUI.SetActive(true);//Display the success interface
            Cursor.lockState = CursorLockMode.None;//Unlock the mouse
            if (PlayerPrefs.GetInt("Battle") <= 5) PlayerPrefs.SetInt("Battle", 5);//Set to complete battle5
        }
    }

    public void LoadPath(GameObject go)
    {
        wayPoints.Clear();//Clear the list before loading the route
        foreach (Transform T in go.transform)//Traverse all navigation point position information in the route prefab and add it to the list
        {
            wayPoints.Add(T.position);
        }
    }

    public void PlayerHealth(float damage)
    {
        playerHealth -= damage;
        playerHealth = Mathf.Round(playerHealth);//Round the blood volume
        playerHealthUIText.text = "Hardness: " + playerHealth;//Update Health Text
        playerHealthUIBar.value = maxPlayerHealth - playerHealth;//Update blood volume bar
        if (playerHealth <= 0)
        {
            playerIsDead = true;
            playerHealthUIText.text = "Player Dead";
            Time.timeScale = 0;//Game pause
            DeadToMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

        }
    }
}
