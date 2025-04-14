using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Enemy class
/// Implement state switching, load enemy patrol route
/// </summary>
public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator animator;
    private AudioSource audioSource;

    [Tooltip("Enemy health")] public float enemyHealth;
    [Tooltip("Enemy health bar")] public Slider slider;
    [Tooltip("Enemy damage text UI")] public Text getDamageText;
    [Tooltip("Enemy death effect")] public GameObject dealEffect;


    public GameObject[] wayPointObj;//Store different routes of the enemy
    public GameObject player;
    public List<Vector3> wayPoints = new List<Vector3>();//Store each patrol point of the patrol route
    public int index;//Subscript value
    [Tooltip("Enemy index ( used to allocate random routes )")] public int nameIndex;
    public int animState;//Animation state identifier, 0: idle, 1: run, 2: attack
    public Transform targetPoint;//Target position

    public EnemyBaseState currentState;//Store the current state of the enemy
    public PatrolState patrolState;//Define enemy patrol state, declare object
    public AttackState attackState;//Define enemy attack state, declare object
    private bool UnderAttack;//Enemy attacked by player

    Vector3 targetPosition;

    public List<Transform> attackList = new List<Transform>();//The attack target of the enemy, there are enemies ( players ) in the scene, stored in the list
    [Tooltip("Attack interval")] public float attackRate;//The longer the time, the slower the attack frequency
    private float nextAttack = 0;//Next attack time
    [Tooltip("Normal attack distance")] public float attackRange;
    private bool isDead;//Determine whether it is dead

    [Tooltip("Soldier enemy firearm attribute")] public Transform EnemyShootPoint;//Enemy gun bullet shot point
    [Tooltip("Enemy collider")] public Collider collider;

    public Transform BulletShootPoint;//Bullet effect shot position
    private float EnemyShootSpreadFactor = 0.05f;//Bullet offset of enemy shooting
    private float ShootRange = 300f;//Gun range
    public Transform bulletPrefab;//Bullet
    private float bulletForce = 100f;//Bullet firing force
    public float MAX_Damage;//Maximum damage
    public float MIN_Damage;//Minimum damage
    public int DiscoveryDistance;//Discover player distance
    public GameObject[] DropModel;//Drop model
    public Transform GunDropPosition;//Gun drop position

    public GameObject attackParticle01;
    public Transform attackParticle01Position;
    public AudioClip attackSound;
    private PlayerController playerController;
    private HummerMove hummerMove;
    public bool Hummer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        patrolState = transform.gameObject.AddComponent<PatrolState>();
        attackState = transform.gameObject.AddComponent<AttackState>();
    }

    void Start()
    {
        isDead = false;//Enemy is not dead
        slider.minValue = 0;//The minimum value of the blood volume bar UI is 0
        slider.maxValue = enemyHealth;//The maximum value of the blood volume bar UI is enemy blood volume
        slider.value = enemyHealth;//The value of the blood volume bar UI is enemy blood volume
        index = 0;
        TranstionToState(patrolState);//At the beginning of the game, the enemy enters the patrol state
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        if (Hummer)
        {
            hummerMove = player.GetComponent<HummerMove>();
        }

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

    void Update()
    {
        if (isDead)//Do not play animation after enemy death
        {
            collider.enabled = false;
            return;
        }


        if (gameObject.name == "Paladin" || gameObject.name == "Mutant" || gameObject.name == "Paladin(Clone)" || gameObject.name == "Mutant(Clone)")//Standard size is 0.4f
        {
            audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.4f;//Update volume size
        }
        else//Standard size is 0.1f
        {
            audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.1f;//Update volume size
        }



        currentState.OnUpdate(this);//Indicates that the current state continues to execute, the enemy movement method needs to be executed continuously
        animator.SetInteger("state", animState);//Play animation

        //Debug.Log(GetComponent <NavMeshAgent>().isOnNavMesh);
        GetComponent<NavMeshAgent>().enabled = true;

        TransToAttack();
    }

    /// <summary>
    /// The enemy moves towards the navigation point
    /// </summary>
    public void MoveToTaget()
    {
        if (attackList.Count == 0)//The enemy has no attack target, go to the patrol point
        {
            targetPosition = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);
            //Debug.Log("Patrol point");

        }
        else//The enemy scans the player and walks towards the player
        {
            if (animator.GetCurrentAnimatorStateInfo(1).IsName("attack"))//The enemy does not move when playing the attack animation
            {
                targetPosition = Vector3.MoveTowards(transform.position, transform.position, agent.speed * Time.deltaTime);
            }
            else
            {
                targetPosition = Vector3.MoveTowards(transform.position, attackList[0].transform.position, agent.speed * Time.deltaTime);
            }
            //Debug.Log("Player");
        }

        agent.destination = targetPosition;
    }




    /// <summary>
    /// Load route
    /// </summary>
    /// <param name="go"></param>
    public void LoadPath(GameObject go)
    {
        wayPoints.Clear();//Clear the list before loading the route
        foreach (Transform T in go.transform)//Traverse all navigation point position information in the route prefab and add it to the list
        {
            wayPoints.Add(T.position);
        }
    }

    /// <summary>
    /// Switch enemy state
    /// </summary>
    /// <param name="state"></param>
    public void TranstionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnemyState(this);
    }

    /// <summary>
    /// The enemy takes damage and deducts blood volume
    /// </summary>
    /// <param name="damage"></param>
    public void Health(float damage)
    {
        if (isDead)
        {
            return;
        }

        UnderAttack = true;//Under attack
        getDamageText.text = Mathf.Round(damage).ToString();//Set the UI text of the enemy's damage, float rounded
        enemyHealth -= Mathf.Round(damage);
        slider.value = enemyHealth;//Update blood volume bar UI

        if (slider.value <= 0)//When the enemy's blood volume is less than or equal to 0
        {
            isDead = true;
            animator.SetTrigger("dying1");//Play enemy death animation
            animator.SetTrigger("dying2");
            //animator.Play("Death");
            //Debug.Log("Death");
            slider.gameObject.SetActive(false);//Hide the blood volume bar
            if (gameObject.name == "Paladin" || gameObject.name == "Mutant" || gameObject.name == "Paladin(Clone)" || gameObject.name == "Mutant(Clone)")//Paladin and Mutant play death explosion special effects
            {
                Destroy(Instantiate(dealEffect, transform.position, Quaternion.identity), 3f);//The enemy's death explosion effect lasts for 3 seconds
                Destroy(gameObject, 5f);//Destroy the enemy 5 seconds after death
            }
            if (gameObject.name != "Paladin" && gameObject.name != "Mutant" && gameObject.name != "Paladin(Clone)" && gameObject.name != "Mutant(Clone)")//Soldier drops weapons
            {
                int dropIndex = Random.Range(0, DropModel.Length);//Randomly drop weapons
                Instantiate(DropModel[dropIndex], GunDropPosition);
                Destroy(gameObject, 30f);//Destroy the enemy 30 seconds after death
            }


        }
    }


    /// <summary>
    /// The enemy attacks the player
    /// Normal attack
    /// </summary>
    public void AttackAction()
    {
        if (Vector3.Distance(transform.position, targetPoint.position) < attackRange)//When the player enters the enemy's attack range, trigger the attack animation
        {
            if (Time.time > nextAttack)
            {
                animator.SetTrigger("attack");//Trigger attack
                nextAttack = Time.time + attackRate;//Update the next attack time

            }
        }

    }


    /// <summary>
    /// Slash attack Animation Event
    /// </summary>
    public void PlayAttackSound()//Play enemy attack sound effect
    {
        audioSource.clip = attackSound;
        audioSource.Play();
    }



    /// <summary>
    /// Jump attack Animation Event
    /// </summary>
    public void PlayMustatAttackEff()
    {
        if (gameObject.name == "Mutant" || gameObject.name == "Mutant(Clone)")
        {
            GameObject attackPar01 = Instantiate(attackParticle01, attackParticle01Position.position, attackParticle01Position.rotation);//Instantiate and play attack special effect
            audioSource.clip = attackSound;//Play attack sound
            audioSource.Play();
            Destroy(attackPar01, 3f);
        }
    }

    public void MutantMoveStraight()
    {
        gameObject.transform.Translate(Vector3.forward * 3f);
    }

    /// <summary>
    /// Soldier Shoot Animation Event
    /// </summary>
    public void PlayShootAttack()
    {
        RaycastHit hit;
        if (!Hummer)
        {
            if (playerController.isCrouching)//When the player crouches, the enemy's shooting point moves down
            {
                EnemyShootPoint.localPosition = new Vector3(-0.095f, -0.45f, 0.4628f);
            }
            else
            {
                EnemyShootPoint.localPosition = new Vector3(0, 0.0259f, 0.4628f);

            }
        }
        Vector3 shootDirection = EnemyShootPoint.forward;//Shoot forward
        shootDirection = shootDirection + EnemyShootPoint.TransformDirection(new Vector3(Random.Range(-EnemyShootSpreadFactor, EnemyShootSpreadFactor), Random.Range(-EnemyShootSpreadFactor, EnemyShootSpreadFactor)));//Add offset to the ray direction
        if (Physics.Raycast(EnemyShootPoint.position, shootDirection, out hit, ShootRange))//Ray detection (here the ray detection method is shot from the center of the screen)
        {
            Transform bullet;

            bullet = Instantiate(bulletPrefab, BulletShootPoint.transform.position, BulletShootPoint.transform.rotation);//Instantiate the bullet trail special effect, including hit and bullet hole special effects

            bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;//Give the bullet trail a forward speed force (plus the offset value of the ray shot out)
            Debug.Log("Fire");
        }

        //Debug.Log(hit.transform.gameObject.name + "hit");

        if (hit.transform.gameObject.transform.tag == "Player")//Judgment when hitting the player
        {
            if (Hummer)
            {
                hit.transform.gameObject.GetComponent<HummerMove>().PlayerHealth(Random.Range(MIN_Damage, MAX_Damage));//Cause damage to the Hummer
                Debug.Log("Hummer");
            }
            else
            {
                hit.transform.gameObject.GetComponent<PlayerController>().PlayerHealth(Random.Range(MIN_Damage, MAX_Damage));//Cause damage to the player
            }
        }

        audioSource.clip = attackSound;//Play gunshot sound effect
        audioSource.Play();
    }



    /*    private void OnTriggerEnter(Collider other)
    {
        if (!isDead && !attackList.Contains(other.transform) && !other.CompareTag("Bullet"))//Exclude bullets from the attack list
        {
            attackList.Add(other.transform);//Add the player to the enemy's attack list
        }
    }*/

    /*private void OnTriggerExit(Collider other)
    {
        if (!isDead && !other.CompareTag("Bullet"))
        {
            attackList.Remove(other.transform);//Remove the player from the enemy's attack list
        }
    }*/


    /// <summary>
    /// The enemy switches to attack mode
    /// </summary>
    public void TransToAttack()
    {
        float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);//Calculate the distance between the enemy and the player
        if (distance < DiscoveryDistance || UnderAttack)//When the player enters the enemy's discovery range or is attacked by the player
        {
            if (!isDead && !attackList.Contains(player.transform))
            {
                attackList.Add(player.transform);//Add the player to the enemy's attack list
            }
        }
        else
        {
            if (!isDead && attackList.Contains(player.transform))
            {
                attackList.Remove(player.transform);//Remove the player from the enemy's attack list
            }
        }

    }



}

