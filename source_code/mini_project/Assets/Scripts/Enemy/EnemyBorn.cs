using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBorn : MonoBehaviour
{
    [SerializeField]
    public GameObject[] Enemy;//Enemy prefab
                              //public GameObject Enemy;//Enemy prefab
    private GameObject[] BornedEnemy;//Instantiated enemy
    public Transform[] EnemyBornPoint;//Enemy spawn point
    private float bornTime;//Next enemy spawn time, determine whether to spawn enemy
    public float bornInterval;//Enemy spawn time interval
    public int battle;//Current scene's battle number
    public float enemyNumber;//Total number of enemies spawned
    private int currentEnemyNumber;//Number of enemies already spawned
    public GameObject WeaponBackpackUI;//Weapon library UI
    public GameObject SuccessUI;//Success interface UI
    private Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        /* Set enemy spawn interval based on difficulty */
        if (PlayerPrefs.GetString("Difficulties") == "Easy")
        {
            bornInterval = bornInterval * 1.5f;
            enemyNumber = enemyNumber * 0.67f;
        }
        else if (PlayerPrefs.GetString("Difficulties") == "Normal")
        {
            bornInterval = bornInterval;
            enemyNumber = enemyNumber;

        }
        else if (PlayerPrefs.GetString("Difficulties") == "Hard")
        {
            bornInterval = bornInterval * 0.67f;
            enemyNumber = enemyNumber * 1.5f;

        }
        enemy = GetComponent<Enemy>();
        if (battle == 2) BornedEnemy = new GameObject[1];
        if (battle == 3) BornedEnemy = new GameObject[(int)enemyNumber + 1];
        if (battle == 4) BornedEnemy = new GameObject[EnemyBornPoint.Length];
    }

    // Update is called once per frame
    void Update()
    {

        /*  if (Time.time > bornTime)//Enemy spawn time interval
            {
                BornedEnemy = Instantiate(Enemy) as GameObject;
                BornedEnemy.transform.position = EnemyBornPoint.position;
                bornTime = Time.time + 10f;

            }*/

        /*        if (BornedEnemy == null)
                {
                    BornedEnemy = Instantiate(Enemy) as GameObject;
                    BornedEnemy = Instantiate(Enemy) as GameObject;
                    BornedEnemy = Instantiate(Enemy) as GameObject;
                    BornedEnemy.transform.position = new Vector3(0, 3.18000007f, 41.2299995f);
                }*/
        if (battle == 2) battle2();
        if (battle == 3) battle3();
        if (battle == 4) battle4();



    }



    /// <summary>
    /// Enemy spawn method and success judgment for battle2
    /// </summary>
    public void battle2()
    {
        if (currentEnemyNumber <= enemyNumber)//Spawn enemy when the number of enemies has not reached the maximum number of enemies
        {
            if (Time.time > bornTime)//Enemy spawn time interval
            {
                int dropIndex = Random.Range(0, Enemy.Length);
                BornedEnemy[0] = Instantiate(Enemy[dropIndex]) as GameObject;
                //Debug.Log(dropIndex);
                bornTime = Time.time + bornInterval;
                currentEnemyNumber += 1;//Increase the number of enemies already spawned by one

            }

            if (SuccessUI.activeSelf)
            {
                if (PlayerPrefs.GetInt("Battle") <= 2)
                {
                    Debug.Log(PlayerPrefs.GetInt("Battle"));
                    PlayerPrefs.SetInt("Battle", 2);//Set to have completed battle2
                }
            }
        }
    }



    /// <summary>
    /// Enemy spawn method and success judgment for battle3
    /// </summary>
    public void battle3()
    {
        if (currentEnemyNumber <= enemyNumber)//Spawn enemy when the number of enemies has not reached the maximum number of enemies
        {
            if (Time.time > bornTime)//Enemy spawn time interval
            {
                BornedEnemy[currentEnemyNumber] = Instantiate(Enemy[0]) as GameObject;
                BornedEnemy[currentEnemyNumber].transform.position = EnemyBornPoint[0].position;
                bornTime = Time.time + bornInterval;
                currentEnemyNumber += 1;//Increase the number of enemies already spawned by one
            }
        }

        if (currentEnemyNumber > (enemyNumber - 0.5f))//All enemies have been spawned
        {
            int DeadEnemy = 0;
            for (int i = 0; i < BornedEnemy.Length; i++)//Traverse the BornedEnemy list, if any element is null, it means this enemy is dead
            {
                if (BornedEnemy[i] == null)
                {
                    DeadEnemy += 1;
                }
            }

            if (DeadEnemy == currentEnemyNumber)//When the number of dead enemies equals the number of spawned enemies, it means all enemies have been cleared
            {
                Time.timeScale = 0f;
                WeaponBackpackUI.SetActive(false);
                SuccessUI.SetActive(true);//Display success interface
                Cursor.lockState = CursorLockMode.None;//Unlock the mouse
                if (PlayerPrefs.GetInt("Battle") <= 3) PlayerPrefs.SetInt("Battle", 3);//Set to have completed battle3

            }

        }
    }

    /// <summary>
    /// Enemy spawn method and success judgment for battle4
    /// </summary>
    public void battle4()
    {

        if (currentEnemyNumber == 0)//Spawn all enemies at once
        {
            for (int i = 0; i < EnemyBornPoint.Length; i++)
            {
                BornedEnemy[i] = Instantiate(Enemy[0]);
                BornedEnemy[i].transform.parent = EnemyBornPoint[i];
                BornedEnemy[i].transform.localPosition = new Vector3(0, 0, 0);
                //BornedEnemy[i].transform.position = EnemyBornPoint[i].position;
                //BornedEnemy[i].transform.position = gameObject.transform.position;
                //Debug.Log(i + ": " + BornedEnemy[i].transform.position);
                //Debug.Log(gameObject.transform.position);
                currentEnemyNumber += 1;
            }

        }

        int DeadEnemy = 0;
        for (int i = 0; i < BornedEnemy.Length; i++)//Traverse the BornedEnemy list, if any element is null, it means this enemy is dead
        {
            if (BornedEnemy[i] == null)
            {
                DeadEnemy += 1;
            }
        }

        if (DeadEnemy == currentEnemyNumber)//When the number of dead enemies equals the number of spawned enemies, it means all enemies have been cleared
        {
            Time.timeScale = 0f;
            WeaponBackpackUI.SetActive(false);
            SuccessUI.SetActive(true);//Display success interface
            Cursor.lockState = CursorLockMode.None;//Unlock the mouse
            if (PlayerPrefs.GetInt("Battle") <= 4) PlayerPrefs.SetInt("Battle", 4);//Set to have completed battle4
        }

    }


}

