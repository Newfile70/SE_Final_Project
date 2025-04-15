using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Internal class for weapon sound effects
/// </summary>

public class MachineSoundClips
{
    public AudioClip shootSound;//Fire sound effect
}



public class Weapon_MachineGun : Weapon
{
    public HummerMove hummerMove;

    [Header("Weapon part positions")]
    [Tooltip("Shooting position")] public Transform ShootPoint;//Position where the ray is shot
    public Transform BulletShootPoint;//Position where the bullet effect is shot
    [Tooltip("Position where the bullet casing is ejected")] public Transform CasingBulletSpawnPoint;

    [Header("Bullet prefab and effects")]
    public Transform bulletPrefab;//Bullet
    public Transform casingPrefab;//Bullet casing


    [Header("Weapon properties")]
    [Tooltip("Weapon range")] public float range;
    private float fireRate;
    [Tooltip("Weapon firing speed")] public float originRate;//Original firing speed
    private float SpreadFactor;//A bit of offset for shooting
    private float fireTimer;//Timer to control weapon firing speed
    private float bulletForce;//Force of bullet firing
    private int gunFragment;//Number of bullets fired by a gun at once
    public float minDamage;
    public float maxDamage;

    [Header("Effects")]
    public Light muzzleflashLight;//Fire light
    private float lightDuraion;//Light duration
    public ParticleSystem muzzlePatic;//Light flame particle effect 1
    public ParticleSystem sparkPatic;//Light flame particle effect 2 (sparks)
    public int minSparkEmission = 3;//Random spread value of sparks
    public int maxSparkEmission = 8;

    [Header("Audio source")]
    private AudioSource mainAudioSource;
    public SoundClips soundClips;

    [Header("UI")]
    public Image[] crossQuarterlmgs;//Crosshair
    public float currentExpanedDegree;//Current degree of crosshair expansion
    private float crossExpanedDegree;//Degree of crosshair expansion per frame
    private float maxCrossDegree;//Maximum degree of expansion
    public Text ammoTextUI;//Bullet count

    private bool GunShootInput;//Press the fire button

    private void Awake()
    {
        mainAudioSource = GetComponent<AudioSource>();
        hummerMove = GetComponentInParent<HummerMove>();
    }


    void Start()
    {
        gunFragment = 1;
        muzzleflashLight.enabled = false;
        crossExpanedDegree = 10f;
        maxCrossDegree = 300f;
        lightDuraion = 0.02f;
        range = 300f;
        bulletForce = 200f;
        fireRate = 0.15f;
        SpreadFactor = 0.02f;
    }


    private void Update()
    {
        if (hummerMove.playerIsDead)
        {
            for (int i = 0; i < crossQuarterlmgs.Length; i++)//Hide the crosshair
            {
                crossQuarterlmgs[i].gameObject.SetActive(false);
            }
            ammoTextUI.gameObject.SetActive(false);//Hide bullet count
            mainAudioSource.Pause();
        }

        mainAudioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.25f;//Update volume size

        ExpandingCrossUpdate(crossExpanedDegree);//Update the degree of crosshair expansion

        //Timer
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;

        }

        GunShootInput = Input.GetMouseButton(0);
        if (GunShootInput)
        {
            GunFire();//Fire the gun
        }

    }




    public override void GunFire()
    {
        /*
         * Control the firing speed
         * Cannot fire
         */
        if (fireTimer < fireRate)//Control bullet firing speed
        {
            return;
        }



        StartCoroutine(MuzzleFlashLight());//Fire light
        muzzlePatic.Emit(1);//Emit a muzzle flame particle
        sparkPatic.Emit(Random.Range(minSparkEmission, maxSparkEmission));//Emit muzzle sparks

        StartCoroutine(Shoot_Crss());//Increase the size of the crosshair


        for (int i = 0; i < gunFragment; i++)
        {
            RaycastHit hit;
            Vector3 shootDirection = ShootPoint.forward;//Shoot forward
            shootDirection = shootDirection + ShootPoint.TransformDirection(new Vector3(Random.Range(-SpreadFactor, SpreadFactor), Random.Range(-SpreadFactor, SpreadFactor)));//Add an offset to the ray direction

            if (Physics.Raycast(ShootPoint.position, shootDirection, out hit, range))//Ray detection (here the ray detection method is shot from the center of the screen)
            {
                Transform bullet;

                bullet = Instantiate(bulletPrefab, BulletShootPoint.transform.position, BulletShootPoint.transform.rotation);//Instantiate the bullet trail effect, including hit and bullet hole effects
                bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;//Give the bullet trail a forward speed force (plus the offset value of the ray shot out)

                if (hit.transform.gameObject.transform.tag == "Enemy")//Judgment when hitting an enemy
                {
                    if (hit.transform.gameObject.transform.name == "Military4x4_02-green-tC02")
                    {
                        hit.transform.gameObject.GetComponent<HummerEnemy>().Health(Random.Range(minDamage, maxDamage));
                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<EnemyForHummer>().Health(Random.Range(minDamage, maxDamage));
                    }
                }

                //Debug.Log(hit.transform.gameObject.name + "Hit");
            }

        }

        //Instantiate(casingPrefab, CasingBulletSpawnPoint.transform.position, CasingBulletSpawnPoint.transform.rotation);//Instantiate the bullet casing

        mainAudioSource.clip = soundClips.shootSound;//Switch different shooting sound effects according to whether the silencer is equipped
        mainAudioSource.Play();//Play shooting sound effect




        fireTimer = 0f;//Reset timer

    }


    /// <summary>
    /// Set the fire light
    /// </summary>
    public IEnumerator MuzzleFlashLight()
    {
        muzzleflashLight.enabled = true;
        yield return new WaitForSeconds(lightDuraion);
        muzzleflashLight.enabled = false;
    }


    /// <summary>
    /// Increase or decrease the degree of crosshair expansion according to the specified size
    /// </summary>
    /// <param name="expanDegree"></param>
    public override void ExpandingCrossUpdate(float expanDegree)
    {
        if (currentExpanedDegree < expanDegree - 5)
        {
            ExpendCross(150 * Time.deltaTime);
        }
        else if (currentExpanedDegree > expanDegree + 5)
        {
            ExpendCross(-300 * Time.deltaTime);
        }

    }

    /// <summary>
    ///Change the degree of crosshair expansion and record the current degree of crosshair expansion 
    /// </summary>
    public void ExpendCross(float add)
    {
        crossQuarterlmgs[0].transform.localPosition += new Vector3(-add, 0, 0);//Left crosshair
        crossQuarterlmgs[1].transform.localPosition += new Vector3(add, 0, 0);//Right crosshair
        crossQuarterlmgs[2].transform.localPosition += new Vector3(0, add, 0);//Upper crosshair
        crossQuarterlmgs[3].transform.localPosition += new Vector3(0, -add, 0);//Lower crosshair

        currentExpanedDegree += add;//Save the current degree of crosshair expansion
        currentExpanedDegree = Mathf.Clamp(currentExpanedDegree, 0, maxCrossDegree);//Limit the degree of crosshair expansion  

    }
    /// <summary>
    /// Coroutine, call the degree of crosshair expansion, execute 5 times in 1 frame
    /// Only responsible for instantly increasing the crosshair when shooting
    /// </summary>
    /// <returns></returns>
    public IEnumerator Shoot_Crss()
    {
        yield return null;
        for (int i = 0; i < 5; i++)
        {
            ExpendCross(Time.deltaTime * 500);
        }

    }










    public override void Reload()
    {

    }


    public override void DoReloadAnimation()
    {

    }

    public override void AimIn()
    {

    }

    public override void AimOut()
    {

    }
}