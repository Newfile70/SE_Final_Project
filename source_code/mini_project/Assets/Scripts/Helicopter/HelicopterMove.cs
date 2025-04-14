using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HelicopterMove : MonoBehaviour
{
    public NavMeshAgent agent;
    Vector3 targetPosition;//Target point
    public GameObject wayPointObj;//Store the player's town patrol route
    public GameObject wayPointObjLanding1;//Store the player's landing route 1
    public GameObject wayPointObjLanding2;//Store the player's landing route 2
    public GameObject wayPointObjLanding3;//Store the player's landing route 3
    public GameObject WayPointObjToTown_Player;//Store the player's town entry route
    public List<Vector3> wayPoints = new List<Vector3>();//Store each patrol point of the patrol route
    private int index;//Route index value
    public float maxPlayerHealth;//Player's life value
    private float playerHealth;//Player's life value
    public bool playerIsDead;//Judge whether the player is dead
    private bool toLanding;//Judge whether to perform the landing operation
    public bool canLanding;//Judge whether the landing operation can be performed
    public GameObject helicopter;//Helicopter
    public GameObject body;//Player on the helicopter
    public GameObject player;//Player off the helicopter
    public float rateOfDecline;//Helicopter's rate of decline
    public float rateOfVelocity;//Helicopter's rate of velocity change
    private float helicopterHeight;//Height of the helicopter
    public Collider collider;//Helicopter's collider
    public bool playerExit;//Whether the player exits the plane
    public bool toTown;//Whether the player enters the town
    public GameObject enemyHelicopter;//Enemy helicopter
    private HelicopterEnemy enemyHelicopterAttribute;//Enemy helicopter


    public GameObject DeadToMenu;//Button to return to the main menu after death
    public GameObject SuccessUI;//Success UI

    public Text playerHealthUIText;
    public Slider playerHealthUIBar;
    public GameObject exitText;//Exit helicopter Text
    public Text task;//Task
    public Text tip;//Tip
    private bool AuxiliaryExitText;
    private bool taskText;
    public AudioSource audioSource;

    void Awake()
    {
        Application.targetFrameRate = 60;//Set the frame rate to 90
    }

    void Start()
    {
        task.text = "Task: Waiting to go to town";//Display the initial task
        tip.text = "";
        Cursor.lockState = CursorLockMode.Locked;
        agent = GetComponent<NavMeshAgent>();
        playerHealth = maxPlayerHealth;
        playerHealthUIBar.maxValue = maxPlayerHealth;
        index = 0;
        LoadPath(WayPointObjToTown_Player);//Load the town entry path points
        DynamicGI.UpdateEnvironment();//Update the lighting environment to prevent it from darkening when switching scenes
        enemyHelicopterAttribute = enemyHelicopter.GetComponent<HelicopterEnemy>();
        //agent.updateRotation = false;//Turn off automatic rotation
        Time.timeScale = 1f;
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.timeScale);
        Application.targetFrameRate = 60;//Set the frame rate to 90

        //Debug.Log(agent.velocity);
        if (playerIsDead)//After the player dies, stop all moving behaviors
        {
            audioSource.Pause();
            return;
        }

        if (!toTown)//Still flying on the town valley route
        {
            float distance = Vector3.Distance(transform.position, wayPoints[index]);//Calculate the distance between the helicopter and the navigation point
            if (distance <= 3f)//When the distance is very small, it means that it has reached the navigation point
            {
                index++;//Set the next navigation point
                index = Mathf.Clamp(index, 0, wayPoints.Count - 1);//Limit the size of index to prevent overflow
                if (Vector3.Distance(transform.position, wayPoints[wayPoints.Count - 1]) <= 0.5f)//Judge the distance between the enemy and the last navigation point on the patrol route. If the distance is very small, then the current route has been completed, reset the navigation point index, and let the enemy repeat the patrol route
                {
                    index = 0;
                }
            }
            targetPosition = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);//Set the moving position and speed of the target point
            agent.destination = targetPosition;

            if (index == 2)//When encountering an enemy helicopter, display clear enemy helicopter and tip
            {
                task.text = "Task: Destroy enemy helicopter";
                tip.text = "Tip: Shoot the tail rotor";
            }
            ChangeVelocity(4, 33.5f, rateOfVelocity, true);//When passing through the valley and the speed is higher than the normal value
            ChangeHeight(5, 280f, rateOfDecline, true);//When approaching the valley and the height is lower than the maximum value
            ChangeVelocity(5, 25, rateOfVelocity, false);//When passing through the valley and the speed is higher than the normal value
            ChangeHeight(20, 75f, rateOfDecline, false);//When passing through the valley and the height is higher than the normal value
            if (!taskText)//Only call task.text once
            {
                if (enemyHelicopterAttribute.Explosion)
                {
                    task.text = "Task: Waiting to go to town";
                    taskText = true;
                    tip.text = "";
                }
            }
            if (index == 20) task.text = "Task: kill all enemies";//When you are about to reach the town, display clear all enemies

        }



        else if (!toLanding && toTown)//Still flying in the town sky
        {
            float distance = Vector3.Distance(transform.position, wayPoints[index]);//Calculate the distance between the helicopter and the navigation point
            if (distance <= 3f)//When the distance is very small, it means that it has reached the navigation point
            {
                index++;//Set the next navigation point
                index = Mathf.Clamp(index, 0, wayPoints.Count - 1);//Limit the size of index to prevent overflow
                if (Vector3.Distance(transform.position, wayPoints[wayPoints.Count - 1]) <= 0.5f)//Judge the distance between the enemy and the last navigation point on the patrol route. If the distance is very small, then the current route has been completed, reset the navigation point index, and let the enemy repeat the patrol route
                {
                    index = 0;
                }
            }
            targetPosition = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);//Set the moving position and speed of the target point
            agent.destination = targetPosition;
            //transform.LookAt(wayPoints[index]);//Make the player always face the target point

            ChangeHeight(4, 125f, rateOfDecline / 2f, true);//When approaching the tower and the height is lower than the maximum value
            ChangeHeight(9, 75f, rateOfDecline / 3f, false);//When passing the tower and the height is higher than the normal value

            /* helicopterHeight = helicopter.transform.localPosition.y;//Get the current height of the helicopter
             if (index == 4 && helicopterHeight <= 125f)//When approaching the tower and the height is lower than the maximum value
             {
                 helicopterHeight = helicopter.transform.localPosition.y + (rateOfDecline / 2f * Time.deltaTime);//The helicopter rises to the maximum height
                 helicopter.transform.localPosition = new Vector3(helicopter.transform.localPosition.x, helicopterHeight, helicopter.transform.localPosition.z);
             }
             else if (index == 9 && helicopterHeight >= 75f)//When passing the tower and the height is higher than the normal value
             {
                 helicopterHeight = helicopter.transform.localPosition.y - (rateOfDecline / 3f * Time.deltaTime);//The helicopter descends to the normal height
                 helicopter.transform.localPosition = new Vector3(helicopter.transform.localPosition.x, helicopterHeight, helicopter.transform.localPosition.z);
             }*/

        }



        else//Perform landing operation
        {
            float distance = Vector3.Distance(transform.position, wayPoints[index]);//Calculate the distance between the helicopter and the navigation point
            if (distance <= 3f)//When the distance is very small, it means that it has reached the navigation point
            {
                index++;//Set the next navigation point
                index = Mathf.Clamp(index, 0, wayPoints.Count - 1);//Limit the size of index to prevent overflow
            }
            targetPosition = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);//Set the moving position and speed of the target point
            agent.destination = targetPosition;

            //helicopterHeight = helicopter.transform.localPosition.y;//Get the current height of the helicopter
            ChangeHeight(wayPoints.Count - 1, 10f, rateOfDecline / 1.5f, false);//When approaching the end point and the height is above the ground
            /*if (index == wayPoints.Count - 1 && helicopterHeight >= 10f)//When approaching the end point and the height is above the ground
            {
                helicopterHeight = helicopter.transform.localPosition.y - (rateOfDecline /1.5f * Time.deltaTime);//The helicopter descends to the ground
                helicopter.transform.localPosition = new Vector3(helicopter.transform.localPosition.x, helicopterHeight, helicopter.transform.localPosition.z);
            }*/
        }

        audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.25f;//Update the volume size

        if (helicopterHeight <= 10 && !AuxiliaryExitText)//When the helicopter lands, only show the landing prompt once
        {
            exitText.SetActive(true);
        }

        if (!AuxiliaryExitText && canLanding)//Show the task when landing
        {
            task.text = "Task: waiting for the helicopter to land";
        }

        if (helicopterHeight <= 10 && Input.GetKeyDown(KeyCode.F))//After landing, press the F key to get off the helicopter, switch from the helicopter lens to the down machine lens
        {
            body.SetActive(false);//Turn off the helicopter lens
            player.SetActive(true);//Turn on the down machine lens
            playerHealthUIBar.maxValue = 100f;
            exitText.SetActive(false);//Close the landing prompt
            AuxiliaryExitText = true;
            task.text = "Task: kill all remaining enemies";
            helicopter.name = "Helicopter";//Change the name of the helicopter to prevent it from being the same as the player
            helicopter.tag = "Helicopter";//Change the tag of the helicopter to prevent it from being attacked by the enemy and deducting blood
            playerExit = true;

            /* Show sniper rifle *//*
            PlayerController Player = GetComponentInChildren<PlayerController>();
            GameObject weaponModel = GameObject.Find("HPlayer/Helicopter/Player/Assult_Rifle_Arm/Inventory/4").gameObject;//Find and get each weapon object under the Inventory object
            Player.PickUpWeapon(4, weaponModel);//Call the method, add and display the weapon */
        }


        if (!toLanding && canLanding) SwitchToLanding();
        if (!toTown) SwitchToEnterTown();


    }


    /// <summary>
    /// The helicopter changes height to the target point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="targetHeight"></param>
    /// <param name="rateOfDecline"></param>
    /// <param name="up"></param>
    public void ChangeHeight(int index, float targetHeight, float rateOfDecline, bool up)
    {
        helicopterHeight = helicopter.transform.localPosition.y;//Get the current height of the helicopter
        if (up == true)
        {
            if (this.index == index && helicopterHeight <= targetHeight)//When reaching the target point and the height is lower than the target height
            {
                helicopterHeight = helicopter.transform.localPosition.y + (rateOfDecline * Time.deltaTime);//The helicopter rises to the target height
                helicopter.transform.localPosition = new Vector3(helicopter.transform.localPosition.x, helicopterHeight, helicopter.transform.localPosition.z);
            }
        }
        else
        {
            if (this.index == index && helicopterHeight >= targetHeight)//When reaching the target point and the height is higher than the target height
            {
                helicopterHeight = helicopter.transform.localPosition.y - (rateOfDecline * Time.deltaTime);//The helicopter descends to the target height
                helicopter.transform.localPosition = new Vector3(helicopter.transform.localPosition.x, helicopterHeight, helicopter.transform.localPosition.z);

            }
        }
    }


    /// <summary>
    /// The helicopter changes speed to the target point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="targetSpeed"></param>
    /// <param name="rateOfVelocity"></param>
    /// <param name="speedUp"></param>
    public void ChangeVelocity(int index, float targetSpeed, float rateOfVelocity, bool speedUp)
    {
        if (speedUp)//If it is acceleration
        {
            if (this.index == index && agent.speed <= targetSpeed)//When the speed has not accelerated to the target speed
            {
                agent.speed += rateOfVelocity * Time.deltaTime;
            }
        }
        else//Deceleration
        {
            if (this.index == index && agent.speed >= targetSpeed)//When the speed has not decreased to the target speed
            {
                agent.speed -= rateOfVelocity * Time.deltaTime;
            }
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

    public void SwitchToEnterTown()//Helicopter enters the city
    {
        if (index == 22)
        {
            LoadPath(wayPointObj);
            toTown = true;
            index = 0;
        }
    }




    public void SwitchToLanding()//The helicopter switches to landing mode
    {
        if (index == 3)//The first landing point
        {
            LoadPath(wayPointObjLanding1);
            toLanding = true;
            index = 0;
        }

        else if (index == 12)//The second landing point
        {
            LoadPath(wayPointObjLanding2);
            toLanding = true;
            index = 0;
        }

        else if (index == 22)//The third landing point
        {
            LoadPath(wayPointObjLanding3);
            toLanding = true;
            index = 0;
        }
    }
}
