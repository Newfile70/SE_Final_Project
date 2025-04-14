using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class HummerEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private AudioSource audioSource;
    Vector3 targetPosition;//Target point
    public GameObject wayPointObjWin;//Store enemy route (win)
    public GameObject wayPointObjFail;//Store enemy different routes (fail)
    public List<Vector3> wayPoints = new List<Vector3>();//Store each patrol point of the patrol route
    private int index;//Route subscript value

    [Tooltip("Enemy health")] public float enemyHealth;
    [Tooltip("Enemy health bar")] public Slider slider;
    [Tooltip("Enemy damage text UI")] public Text getDamageText;
    [Tooltip("Enemy death effect")] public GameObject deathEffectExplosion;
    public GameObject deathEffectFlameBall;
    public GameObject enemySoldier;
    public AudioClip deathSound;

    public GameObject player;

    public List<Transform> attackList = new List<Transform>();//Enemy's attack target, there are enemies (players) in the scene stored in the list
    [Tooltip("Attack interval")] public float attackRate;//The longer the time, the slower the attack frequency
    private float nextAttack = 0;//Next attack time
    [Tooltip("Normal attack distance")] public float attackRange;
    private bool isDead;//Judge whether it is dead

    [Tooltip("Soldier enemy gun attribute")] public Transform EnemyShootPoint;//Enemy bullet shooting point
    [Tooltip("Enemy collider")] public Collider collider;
    public GameObject gunMount;//Enemy turret

    public Transform BulletShootPoint;//Position where the bullet effect is shot
    private float EnemyShootSpreadFactor = 0.05f;//Enemy shooting bullet offset
    private float ShootRange = 300f;//Gun range
    public Transform bulletPrefab;//Bullet
    private float bulletForce = 100f;//The force of bullet firing
    public float MAX_Damage;//Maximum damage
    public float MIN_Damage;//Minimum damage
    public int DiscoveryDistance;//Discover player distance

    public AudioClip attackSound;
    public bool Hummer;
    private bool Explosion;
    private bool close;//Judge whether the player is close to Hummer
    private float distance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {

        isDead = false;//The enemy is not dead
        slider.minValue = 0;//The minimum value of the health bar UI is 0
        slider.maxValue = enemyHealth;//The maximum value of the health bar UI is enemy health
        slider.value = enemyHealth;//The value of the health bar UI is enemy health
        index = 0;
        LoadPath(wayPointObjWin);
        nextAttack = Time.time;//Initialize the next attack time

        player = GameObject.Find("Player");

        /* Set enemy attack power according to difficulty */
        if (PlayerPrefs.GetString("Difficulties") == "Easy")
        {
            MAX_Damage = MAX_Damage * 0.7f;
            MIN_Damage = MIN_Damage * 0.7f;
        }
        else if (PlayerPrefs.GetString("Difficulties") == "Normal")
        {
            MAX_Damage = MAX_Damage * 1f;
            MIN_Damage = MIN_Damage * 1f;
        }
        else if (PlayerPrefs.GetString("Difficulties") == "Hard")
        {
            MAX_Damage = MAX_Damage * 1.3f;
            MIN_Damage = MIN_Damage * 1.3f;
        }
    }


    // Update is called once per frame
    void Update()
    {
        audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.1f;//Update volume size
        distance = Vector3.Distance(gameObject.transform.position, player.transform.position);//Calculate the distance between the enemy and the player

        if (distance <= DiscoveryDistance)
        {
            close = true;
        }
        if (!close)//If the player does not approach Hummer, the following actions cannot be performed
        {
            return;
        }


        if (!Explosion)//Hummer cannot perform the following actions after the explosion
        {
            if (index > 2)
            {
                agent.speed = 100;
            }
            float distance = Vector3.Distance(transform.position, wayPoints[index]);//Calculate the distance between Hummer and the navigation point
            if (distance <= 3f)//When the distance is very small, it means that it has reached the navigation point
            {
                index++;//Set the next navigation point
                index = Mathf.Clamp(index, 0, wayPoints.Count - 1);//Limit the size of index to prevent overflow
                if (Vector3.Distance(transform.position, wayPoints[wayPoints.Count - 1]) <= 1f)//Judge the distance between the enemy and the last navigation point on the patrol route. If the distance is very small, it means that Hummer has exploded
                {
                    GameObject InstanceOfExplosion1 = Instantiate(deathEffectExplosion, gameObject.transform);//Instantiate explosion effect
                    GameObject InstanceOfExplosion2 = Instantiate(deathEffectFlameBall, gameObject.transform);//Instantiate flame effect
                    audioSource.clip = deathSound;//Play explosion sound effect
                    audioSource.Play();
                    Destroy(InstanceOfExplosion1, 2f);//Destroy the explosion effect after a delay of 2s
                    Destroy(InstanceOfExplosion2, 20f);//Destroy the flame effect after a delay of 2s
                    Destroy(gameObject, 20f);//Destroy the enemy 20 seconds after death

                    Explosion = true;
                }
            }
            targetPosition = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);//Set the moving position and speed of the target point
            agent.destination = targetPosition;

        }



        if (isDead)//The enemy does not attack the player after death
        {
            collider.enabled = false;
            return;
        }

        if (index == 14 && slider.value > 0)//If Hummer is not killed before the river, Hummer will change the route, greatly increase the attack power, close the collider, and let the player die
        {
            LoadPath(wayPointObjFail);
            index = 22;
            MAX_Damage = 220;
            MIN_Damage = 200;
            collider.enabled = false;
        }


        if (index > 0)//Hummer will shoot after going out for a while
        {
            TransToAttack();
        }


    }

    public void Health(float damage)
    {
        if (isDead)
        {
            return;
        }

        getDamageText.text = Mathf.Round(damage).ToString();//Set the UI text of the enemy's damage, float rounded
        enemyHealth -= Mathf.Round(damage);
        slider.value = enemyHealth;//Update health bar UI

        if (slider.value <= 0)//When the enemy's health is less than or equal to 0
        {
            isDead = true;
            enemySoldier.SetActive(false);
            Debug.Log("Dead");


        }
    }




    public void AttackAction()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)//Trigger attack animation when the player enters the enemy's attack range
        {
            if (Time.time > nextAttack)
            {
                PlayShootAttack();
                nextAttack = Time.time + attackRate;//Update the next attack time

            }
        }

    }


    public void PlayShootAttack()
    {
        RaycastHit hit;

        Vector3 shootDirection = EnemyShootPoint.forward;//Shoot forward
        shootDirection = shootDirection + EnemyShootPoint.TransformDirection(new Vector3(Random.Range(-EnemyShootSpreadFactor, EnemyShootSpreadFactor), Random.Range(-EnemyShootSpreadFactor, EnemyShootSpreadFactor)));//Add offset to the ray direction
        if (Physics.Raycast(EnemyShootPoint.position, shootDirection, out hit, ShootRange))//Ray detection (here the ray detection method is shot from the center of the screen)
        {
            Transform bullet;

            bullet = Instantiate(bulletPrefab, BulletShootPoint.transform.position, BulletShootPoint.transform.rotation);//Instantiate the bullet trail effect, including hit and bullet hole effects

            bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;//Give the bullet trail a forward speed force (plus the offset value of the ray shot out)
                                                                                                                  //Debug.Log("Launch");
        }

        //Debug.Log(hit.transform.gameObject.name + "hit");

        if (hit.transform.gameObject.transform.tag == "Player")//Judgment when hitting the player
        {
            hit.transform.gameObject.GetComponent<HummerMove>().PlayerHealth(Random.Range(MIN_Damage, MAX_Damage));//Cause damage to Hummer
                                                                                                                   //Debug.Log("Hummer");

        }

        audioSource.clip = attackSound;//Play the gun sound effect
        audioSource.Play();
    }

    public void TransToAttack()
    {

        //Debug.Log(distance);
        gunMount.transform.LookAt(player.transform.position);//Make the enemy always face the player
        AttackAction();


    }

    public void LoadPath(GameObject go)
    {
        wayPoints.Clear();//Clear the list before loading the route
        foreach (Transform T in go.transform)//Traverse all navigation point location information in the route prefab and add it to the list
        {
            wayPoints.Add(T.position);
        }
    }


}
