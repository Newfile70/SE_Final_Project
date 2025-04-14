using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyForHummer : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator animator;
    private AudioSource audioSource;

    [Tooltip("Enemy health")] public float enemyHealth;
    [Tooltip("Enemy health bar")] public Slider slider;
    [Tooltip("Enemy damage text UI")] public Text getDamageText;
    [Tooltip("Enemy death special effect")] public GameObject dealEffect;


    public GameObject player;

    Vector3 targetPosition;

    public List<Transform> attackList = new List<Transform>();//The enemy's attack target, there are enemies (players) in the scene, stored in a list
    [Tooltip("Attack interval")] public float attackRate;//The longer the time, the slower the attack frequency
    private float nextAttack = 0;//Next attack time
    [Tooltip("Normal attack distance")] public float attackRange;
    private bool isDead;//Determine if it is dead

    [Tooltip("Soldier enemy gun attributes")] public Transform EnemyShootPoint;//Enemy bullet shooting point
    [Tooltip("Enemy collider")] public Collider collider;

    public Transform BulletShootPoint;//Bullet special effect shot position
    private float EnemyShootSpreadFactor = 0.05f;//Enemy shooting bullet offset
    private float ShootRange = 300f;//Gun range
    public Transform bulletPrefab;//Bullet
    private float bulletForce = 100f;//Bullet launch force
    public float MAX_Damage;//Maximum damage
    public float MIN_Damage;//Minimum damage
    public int DiscoveryDistance;//Discover player distance
    public bool forHelicopter;
    private bool UnderAttack;//The enemy is attacked by the player

    public AudioClip attackSound;
    public bool Hummer;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        isDead = false;//The enemy is not dead
        slider.minValue = 0;//The minimum value of the blood volume bar UI is 0
        slider.maxValue = enemyHealth;//The maximum value of the blood volume bar UI is the enemy's blood volume
        slider.value = enemyHealth;//The value of the blood volume bar UI is the enemy's blood volume
        if (forHelicopter) EnemyShootSpreadFactor = 0.15f;//The accuracy of the helicopter enemy is worse
        player = GameObject.Find("Player");
        agent.updateRotation = false;//Turn off automatic rotation


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
        if (isDead)//Do not play animation after the enemy is dead
        {
            collider.enabled = false;
            return;
        }


        audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.1f;//Update volume size
        TransToAttack();

    }

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

            Destroy(gameObject, 5f);//Destroy the enemy 5 seconds after death

        }
    }



    public void AttackAction()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)//When the player enters the enemy's attack range, trigger the attack animation
        {
            if (Time.time > nextAttack)
            {
                animator.SetTrigger("attack");//Trigger attack
                nextAttack = Time.time + attackRate;//Update next attack time

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

            bullet = Instantiate(bulletPrefab, BulletShootPoint.transform.position, BulletShootPoint.transform.rotation);//Instantiate the bullet trail special effect, including hit and bullet hole special effects

            bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;//Give the bullet trail a forward speed force (plus the offset value of the ray shot out)
            //Debug.Log("Fire");
        }

        //Debug.Log(hit.transform.gameObject.name + "hit");

        if (hit.transform.gameObject.transform.tag == "Player")//Judgment when hitting the player
        {
            if (forHelicopter)
            {
                hit.transform.gameObject.GetComponentInParent<HelicopterMove>().PlayerHealth(Random.Range(MIN_Damage, MAX_Damage));//Cause damage to the Hummer                                                                                                                          

            }
            else
            {
                hit.transform.gameObject.GetComponentInParent<HummerMove>().PlayerHealth(Random.Range(MIN_Damage, MAX_Damage));//Cause damage to the Hummer                                                                                                                          
                //Debug.Log("Hummer");
            }

        }

        audioSource.clip = attackSound;//Play gunshot sound effect
        audioSource.Play();
    }

    public void TransToAttack()
    {
        float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);//Calculate the distance between the enemy and the player
        //Debug.Log("distance true");
        if (distance < DiscoveryDistance || UnderAttack)//When the player enters the enemy's discovery range or is attacked by the player
        {
            transform.LookAt(player.transform.position);//Make the enemy always face the player
            AttackAction();
        }
        else
        {
            animator.SetTrigger("notattack");
        }

    }
}
