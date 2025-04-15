using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HelicopterEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private AudioSource audioSource;
    Vector3 targetPosition;//Target point
    public GameObject WayPointObjToTown_HelicopterEnemy;//Store enemy route
    public List<Vector3> wayPoints = new List<Vector3>();//Store each patrol point of the patrol route
    private int index;//Route subscript value
    public GameObject helicopterEnemyObject;//Helicopter entity
    public GameObject lookAtPoint;//Auxiliary direction point (normal flight)
    private Quaternion targetRotation;//Player's target point
    public float spinSpeed;  //Enemy rotation speed
    private bool spinToPlayer;
    private bool fristTimeToShoot;//Determine whether it is the first shot to initialize the time

    [Tooltip("Enemy health")] public float enemyHealth;
    [Tooltip("Enemy health bar")] public Slider slider;
    public GameObject canvas;
    [Tooltip("Enemy damage text UI")] public Text getDamageText;
    [Tooltip("Enemy death special effect")] public GameObject deathEffectExplosion;
    public GameObject deathEffectFlameBall;
    public GameObject flameStream;
    public GameObject tailRotor;//Tail rotor
    public AudioClip deathSound;
    public float rateOfDecline;//Helicopter descent rate
    public float rateOfVelocity;//Helicopter speed change rate
    private float helicopterHeight;//Height of the helicopter
    public float rateOfFallingSpin;//Helicopter's falling rotation speed


    public GameObject player;


    public List<Transform> attackList = new List<Transform>();//The enemy's attack target, there are enemies (players) in the scene, stored in a list
    [Tooltip("Attack interval")] public float attackRate;//The longer the time, the slower the attack frequency
    private float nextAttack = 0;//Next attack time
    private bool isDead;//Determine if it is dead

    [Tooltip("Soldier enemy gun attributes")] public Transform EnemyShootPointLeft;//Enemy missile right shooting point
    public Transform EnemyShootPointRight;//Enemy missile right shooting point
    public Transform BulletShootPointLeft;//Bullet special effect left shot position
    public Transform BulletShootPointRight;//Bullet special effect right shot position
    private bool shootAttackLeft;//Assist the left side to only fire one missile

    [Tooltip("Enemy collider")] public Collider collider;

    private float EnemyShootSpreadFactor = 0.025f;//Enemy shooting bullet offset
    private float ShootRange = 300f;//Gun range
    public Transform bulletPrefab;//Bullet
    private float bulletForce = 100f;//Bullet launch force
    public float MAX_Damage;//Maximum damage
    public float MIN_Damage;//Minimum damage

    public AudioClip attackSound;
    public bool Explosion;//Helicopter crash explosion
    private bool Flame;//Tail rotor damaged and caught fire

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {

        isDead = false;//The enemy is not dead
        slider.minValue = 0;//The minimum value of the blood volume bar UI is 0
        slider.maxValue = enemyHealth;//The maximum value of the blood volume bar UI is the enemy's blood volume
        slider.value = enemyHealth;//The value of the blood volume bar UI is the enemy's blood volume
        index = 0;
        LoadPath(WayPointObjToTown_HelicopterEnemy);
        fristTimeToShoot = true;




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
        audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.2f;//Update volume size
        canvas.transform.LookAt(player.transform.position);//Let the health bar and other UI always face the player

        helicopterHeight = helicopterEnemyObject.transform.localPosition.y;//Get the current height of the helicopter

        if (!Explosion)//The helicopter cannot perform the following actions after the explosion
        {
            /*        if (index > 2)
                    {
                        agent.speed = 100;
                    }*/
            float distance = Vector3.Distance(transform.position, wayPoints[index]);//Calculate the distance between the Hummer and the navigation point
            if (distance <= 3f)//When the distance is very small, it means that it has reached the navigation point
            {
                index++;//Set the next navigation point
                index = Mathf.Clamp(index, 0, wayPoints.Count - 1);//Limit the size of index not to overflow
                /*            if (Vector3.Distance(transform.position, wayPoints[wayPoints.Count - 1]) <= 1f)//Judge the distance between the enemy and the last navigation point on the patrol route. If the distance is very small, it means that the Hummer has exploded
                            {
                                GameObject InstanceOfExplosion1 = Instantiate(deathEffectExplosion, gameObject.transform);//Instantiate explosion effect
                                GameObject InstanceOfExplosion2 = Instantiate(deathEffectFlameBall, gameObject.transform);//Instantiate flame effect
                                audioSource.clip = deathSound;//Play explosion sound effect
                                audioSource.Play();
                                Destroy(InstanceOfExplosion1, 2f);//Destroy the explosion effect after a delay of 2s
                                Destroy(InstanceOfExplosion2, 20f);//Destroy the flame effect after a delay of 2s
                                Destroy(gameObject, 20f);//Destroy the enemy 20 seconds after death

                                Explosion = true;
                            }*/
            }
            targetPosition = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);//Set the moving position and speed of the target point
            agent.destination = targetPosition;

        }

        if (!Flame && enemyHealth < (slider.maxValue / 3))//When the helicopter's blood volume is less than one third of the full blood, the tail rotor catches fire
        {
            GameObject InstanceOfExplosion1 = Instantiate(flameStream, tailRotor.gameObject.transform);//Instantiate explosion effect
            Flame = true;
        }

        if (isDead)//The enemy does not attack the player after death
        {
            if (helicopterHeight >= -3.7)//The helicopter falls after death
            {
                helicopterHeight = helicopterEnemyObject.transform.localPosition.y - (rateOfDecline * Time.deltaTime);//The helicopter descends to the ground
                helicopterEnemyObject.transform.localPosition = new Vector3(helicopterEnemyObject.transform.localPosition.x, helicopterHeight, helicopterEnemyObject.transform.localPosition.z);
                helicopterEnemyObject.transform.Rotate(0, rateOfFallingSpin * Time.deltaTime, 0);


            }
            if (helicopterHeight <= -3.7f && !Explosion)
            {
                GameObject InstanceOfExplosion1 = Instantiate(deathEffectExplosion, gameObject.transform);//Instantiate explosion effect
                GameObject InstanceOfExplosion2 = Instantiate(deathEffectFlameBall, gameObject.transform);//Instantiate flame effect
                audioSource.clip = deathSound;//Play explosion sound effect
                audioSource.Play();
                Destroy(tailRotor);
                Destroy(InstanceOfExplosion1, 2f);//Destroy the explosion effect after a delay of 2s
                Destroy(InstanceOfExplosion2, 40f);//Destroy the flame effect after a delay of 2s
                Destroy(gameObject, 50f);//Destroy the enemy 50 seconds after death

                Explosion = true;
                collider.enabled = false;
            }

            return;
        }

        if (index > 2)//The helicopter shoots again after going out for a while
        {
            if (fristTimeToShoot)
            {
                nextAttack = Time.time;//Initialize the next attack time
                fristTimeToShoot = false;
            }
            TransToAttack();
        }



        ChangeHeight(2, 170f, rateOfDecline * 0.8f, false);//Lower the height after discovering the player
        ChangeVelocity(2, 31, rateOfVelocity, false);//Slow down when discovering the player
        ChangeVelocity(3, 24, rateOfVelocity * 0.3f, false);//Change speed during attack
        ChangeVelocity(4, 43, rateOfVelocity * 0.3f, true);//Change speed during attack

        if (index == 3) collider.enabled = true;

        if (index >= 5)
        {
            MAX_Damage = 8000f;
            MIN_Damage = 7000f;
        }

    }



    /// <summary>
    /// Change the height of the helicopter to the target point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="targetHeight"></param>
    /// <param name="rateOfDecline"></param>
    /// <param name="up"></param>
    public void ChangeHeight(int index, float targetHeight, float rateOfDecline, bool up)
    {
        helicopterHeight = helicopterEnemyObject.transform.localPosition.y;//Get the current height of the helicopter
        if (up == true)
        {
            if (this.index == index && helicopterHeight <= targetHeight)//When reaching the target point and the height is lower than the target height
            {
                helicopterHeight = helicopterEnemyObject.transform.localPosition.y + (rateOfDecline * Time.deltaTime);//The helicopter rises to the target height
                helicopterEnemyObject.transform.localPosition = new Vector3(helicopterEnemyObject.transform.localPosition.x, helicopterHeight, helicopterEnemyObject.transform.localPosition.z);
            }
        }
        else
        {
            if (this.index == index && helicopterHeight >= targetHeight)//When reaching the target point and the height is higher than the target height
            {
                helicopterHeight = helicopterEnemyObject.transform.localPosition.y - (rateOfDecline * Time.deltaTime);//The helicopter descends to the target height
                helicopterEnemyObject.transform.localPosition = new Vector3(helicopterEnemyObject.transform.localPosition.x, helicopterHeight, helicopterEnemyObject.transform.localPosition.z);

            }
        }
    }


    /// <summary>
    /// Change the speed of the helicopter to the target point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="targetSpeed"></param>
    /// <param name="rateOfVelocity"></param>
    /// <param name="speedUp"></param>
    public void ChangeVelocity(int index, float targetSpeed, float rateOfVelocity, bool speedUp)
    {
        if (speedUp)//If it is accelerating
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

    public void Health(float damage)
    {
        if (isDead)
        {
            return;
        }

        getDamageText.text = Mathf.Round(damage).ToString();//Set the UI text of the enemy's damage, float rounded
        enemyHealth -= Mathf.Round(damage);
        slider.value = enemyHealth;//Update blood volume bar UI

        if (slider.value <= 0)//When the enemy's blood volume is less than or equal to 0
        {
            isDead = true;
            Debug.Log("Dead");


        }
    }



    public void AttackAction()
    {
        if (!shootAttackLeft)//Only fire one left missile for one attack
        {
            PlayShootAttackLeft();
            shootAttackLeft = true;
        }
        if (Time.time > nextAttack + 4f)//The right missile is fired 1 second later than the left missile
        {
            PlayShootAttackRight();
            nextAttack = Time.time + attackRate;//Update next attack time
            spinToPlayer = true;
            shootAttackLeft = false;
        }
        //helicopterEnemyObject.transform.LookAt(lookAtPoint.transform.position);
    }



    public void PlayShootAttackLeft()
    {
        RaycastHit hit;

        Vector3 shootDirection = EnemyShootPointLeft.forward;//Shoot forward
        shootDirection = shootDirection + EnemyShootPointLeft.TransformDirection(new Vector3(Random.Range(-EnemyShootSpreadFactor, EnemyShootSpreadFactor), Random.Range(-EnemyShootSpreadFactor, EnemyShootSpreadFactor)));//Add offset to the ray direction
        if (Physics.Raycast(EnemyShootPointLeft.position, shootDirection, out hit, ShootRange))//Ray detection (here the ray detection method is shot from the center of the screen)
        {
            Transform bullet;

            bullet = Instantiate(bulletPrefab, BulletShootPointLeft.transform.position, BulletShootPointLeft.transform.rotation);//Instantiate the bullet trail special effect, including hit and bullet hole special effects

            bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;//Give the bullet trail a forward speed force (plus the offset value of the ray shot out)
                                                                                                                  //Debug.Log("Fire");
        }

        Debug.Log(hit.transform.gameObject.name + "hit");

        if (hit.transform.gameObject.transform.tag == "Player")//Judgment when hitting the player
        {
            hit.transform.gameObject.GetComponentInParent<HelicopterMove>().PlayerHealth(Random.Range(MIN_Damage, MAX_Damage));//Cause damage to the Hummer
                                                                                                                               //Debug.Log("Hummer");

        }

        audioSource.clip = attackSound;//Play gunshot sound effect
        audioSource.Play();
    }

    public void PlayShootAttackRight()
    {
        RaycastHit hit;

        Vector3 shootDirection = EnemyShootPointRight.forward;//Shoot forward
        shootDirection = shootDirection + EnemyShootPointRight.TransformDirection(new Vector3(Random.Range(-EnemyShootSpreadFactor, EnemyShootSpreadFactor), Random.Range(-EnemyShootSpreadFactor, EnemyShootSpreadFactor)));//Add offset to the ray direction
        if (Physics.Raycast(EnemyShootPointRight.position, shootDirection, out hit, ShootRange))//Ray detection (here the ray detection method is shot from the center of the screen)
        {
            Transform bullet;

            bullet = Instantiate(bulletPrefab, BulletShootPointRight.transform.position, BulletShootPointRight.transform.rotation);//Instantiate the bullet trail special effect, including hit and bullet hole special effects

            bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;//Give the bullet trail a forward speed force (plus the offset value of the ray shot out)
                                                                                                                  //Debug.Log("Fire");
        }

        //Debug.Log(hit.transform.gameObject.name + "hit");

        if (hit.transform.gameObject.transform.tag == "Player")//Judgment when hitting the player
        {
            hit.transform.gameObject.GetComponentInParent<HelicopterMove>().PlayerHealth(Random.Range(MIN_Damage, MAX_Damage));//Cause damage to the Hummer
                                                                                                                               //Debug.Log("Hummer");

        }

        audioSource.clip = attackSound;//Play gunshot sound effect
        audioSource.Play();
    }

    public void TransToAttack()
    {
        if (Time.time > nextAttack)
        {
            if (!spinToPlayer)//The enemy aims at the player
            {
                targetRotation = Quaternion.LookRotation(player.transform.position - helicopterEnemyObject.transform.position);//Get the player's rotation target position
                helicopterEnemyObject.transform.rotation = Quaternion.Slerp(helicopterEnemyObject.transform.rotation, targetRotation, spinSpeed * Time.deltaTime);//Make the enemy always face the player
                                                                                                                                                                  //Debug.Log("Look at the player");
            }

            //helicopterEnemyObject.transform.LookAt(player.transform.position);//Make the enemy always face the player

            if (Time.time > nextAttack + 3f)//The helicopter first aims at the player and then fires the missile after 3 seconds
            {
                AttackAction();
            }
            return;
        }


        if (spinToPlayer)//The enemy returns to the normal direction
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            //targetRotation = Quaternion.LookRotation(new Vector3(0, 0, 0) - helicopterEnemyObject.transform.position);//Get the player's rotation target position
            helicopterEnemyObject.transform.localRotation = Quaternion.Slerp(helicopterEnemyObject.transform.localRotation, targetRotation, spinSpeed * Time.deltaTime);//Make the enemy always face the player
                                                                                                                                                              // 目标旋转角度
            

            // 使用Quaternion.Lerp进行平滑旋转
            //helicopterEnemyObject.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            //Debug.Log("Return to normal");
        }
        //Debug.Log(distance);
        if (Time.time > nextAttack - 2f)//The helicopter returns to the normal direction within 3 seconds after attacking the player
        {
            spinToPlayer = false;
        }


    }


    public void LoadPath(GameObject go)
    {
        wayPoints.Clear(); // Clear the list before loading the path
        foreach (Transform T in go.transform) // Traverse all navigation point location information in the path prefab and add it to the list
        {
            wayPoints.Add(T.position);
        }
    }


}
