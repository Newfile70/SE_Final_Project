using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon_HelicopterGun : Weapon
{
    private HummerMove hummerMove;

    private Animator animator;
    private PlayerController playerController;
    private Camera mainCamera;//Main camera
    public Camera gunCamera;//Gun model camera

    public bool IS_AUTORIFLE;//Is it an automatic weapon
    public bool IS_SEMIGUN;//Is it a semi-automatic weapon

    [Header("Weapon Component Position")]
    [Tooltip("Shooting position")] public Transform ShootPoint;//The position where the ray is shot
    public Transform BulletShootPoint;//The position where the bullet effect is shot
    [Tooltip("Bullet shell ejection position")] public Transform CasingBulletSpawnPoint;

    [Header("Bullet Prefabs and Effects")]
    public Transform bulletPrefab;//Bullet
    public Transform casingPrefab;//Bullet casing


    [Header("Gun Attributes")]
    [Tooltip("Weapon range")] public float range;
    private float fireRate;
    [Tooltip("Weapon firing speed")] public float originRate;//Original firing speed
    private float SpreadFactor;//A little offset for shooting
    private float fireTimer;//Timer to control weapon firing speed
    private float bulletForce;//The force of bullet firing
    [Tooltip("Number of bullets in each magazine of the weapon")] public int bulletMag;
    [Tooltip("Current number of bullets")] public int currentBullets;
    [Tooltip("Reserve bullets")] public int bulletLeft;
    public bool isSilencer;//Is a silencer equipped
    private int gunFragment;//The number of bullets fired by the gun at once
    public float minDamage;
    public float maxDamage;

    [Header("Effects")]
    public Light muzzleflashLight;//Fire light
    private float lightDuraion;//Light duration
    public ParticleSystem muzzlePatic;//Light flame particle effect 1
    public ParticleSystem sparkPatic;//Light flame particle effect 2 (Sparks)
    public int minSparkEmission = 1;//Random spread value of sparks
    public int maxSparkEmission = 7;

    [Header("Audio Source")]
    private AudioSource mainAudioSource;
    public SoundClips soundClips;

    [Header("UI")]
    public Image[] crossQuarterlmgs;//Crosshair
    public float currentExpanedDegree;//Current degree of crosshair expansion
    private float crossExpanedDegree;//Degree of crosshair expansion per frame
    private float maxCrossDegree;//Maximum degree of expansion
    public Text ammoTextUI;//Number of bullets
    public Text shootModeTextUI;//Shooting mode

    public PlayerController.MovementState state;
    private bool isReloading;//Is reloading
    private bool isAiming;//Is aiming
    public bool aimAllowed;//Auxiliary judgment whether to execute AimIn or AimOut

    private Vector3 sniperingFiflePosition;//The default initial position of the gun
    public Vector3 sniperingFifleOnPosition;//The model position of the gun when aiming


    [Header("Key Settings")]
    [SerializeField] [Tooltip("Reload bullet key")] private KeyCode reloadInputName = KeyCode.R;
    [SerializeField] [Tooltip("Check weapon key")] private KeyCode inspectInputName = KeyCode.I;
    [SerializeField] [Tooltip("Automatic semi-automatic switch key")] private KeyCode GunShootModelInputName = KeyCode.X;





    public ShootMode shootingMode;//Use enumeration to distinguish shooting modes (automatic and semi-automatic)
    private bool GunShootInput;//According to fully automatic and semi-automatic, the key input of shooting changes
    private int modeNum;//An intermediate parameter for mode switching (1: Fully automatic mode, 2: Semi-automatic mode)
    private string shootModeName;
    float currentVelocity = 0f;


    [Header("Sniper Scope Settings")]
    [Tooltip("Sniper scope material")] public Material scopeRenderMateriala;
    [Tooltip("Color of the sniper scope when the sniper rifle is not aiming")] public Color fadeColor;
    [Tooltip("Color of the sniper scope when the sniper rifle is aiming")] public Color defaultColor;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        hummerMove = GetComponentInParent<HummerMove>();
        mainAudioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
    }



    void Start()
    {
        sniperingFiflePosition = transform.localPosition;
        muzzleflashLight.enabled = false;
        crossExpanedDegree = 50f;
        maxCrossDegree = 300f;
        lightDuraion = 0.02f;
        range = 300f;
        if (gameObject.name == "4")//The sniper rifle has a greater firing force and a longer range
        {
            bulletForce = 500f;
            range = 800f;
        }
        else
        {
            bulletForce = 100f;
        }
        //bulletLeft = bulletMag * 3;
        currentBullets = bulletMag;
        aimAllowed = true;
        UpdateAmmoUI();

        /* Set different shooting modes at the beginning of the game according to different firearms */
        if (IS_AUTORIFLE)
        {
            modeNum = 1;
            shootModeName = "Automatic";
            shootingMode = ShootMode.AutoRifle;
            UpdateAmmoUI();
        }
        if (IS_SEMIGUN)
        {
            modeNum = 0;
            shootModeName = "Semi-automatic";
            shootingMode = ShootMode.SemiGun;
            UpdateAmmoUI();
        }

    }

    private void Update()
    {
        if (hummerMove.playerIsDead)//Player is dead
        {
            for (int i = 0; i < crossQuarterlmgs.Length; i++)//Hide the crosshair
            {
                crossQuarterlmgs[i].gameObject.SetActive(false);
            }
            ammoTextUI.gameObject.SetActive(false);//Hide the number of bullets
            mainAudioSource.Pause();
        }

        mainAudioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.2f;//Update the volume size

        /* Automatic firearms mouse input method, can be switched between GetMouseButton and GetMouseButtonDown */
        if (IS_AUTORIFLE)
        {
            /* Switch shooting mode (semi-automatic and automatic) *//*
            if (Input.GetKeyDown(GunShootModelInputName) && modeNum != 1 )
            {
                modeNum = 1;
                shootModeName = "Automatic";
                shootingMode = ShootMode.AutoRifle;
                UpdateAmmoUI();
            }
            else if (Input.GetKeyDown(GunShootModelInputName) && modeNum != 0)
            {
                modeNum = 0;
                shootModeName = "Semi-automatic";
                shootingMode = ShootMode.SemiGun;
                UpdateAmmoUI();
            }*/

            /* Control the conversion of shooting mode, and then use the code to control it dynamically */


            GunShootInput = Input.GetMouseButton(0);
            fireRate = originRate;



        }
        else
        {
            GunShootInput = Input.GetMouseButtonDown(0);
            if (gameObject.name == "2") fireRate = 0.2f;
            else if (gameObject.name == "4") fireRate = 1.2f;
        }




        if (Input.GetMouseButton(1) && !isReloading && aimAllowed && Time.timeScale == 1f)//Enter aiming (reloading, running, time is not 1 can not enter)
        {
            isAiming = true;
            animator.SetBool("Aim", isAiming);
            AimIn();
            transform.localPosition = sniperingFifleOnPosition;//When aiming, you need to adjust the model position of the gun slightly
        }
        else if (!Input.GetMouseButton(1) && !aimAllowed)
        {
            isAiming = false;
            animator.SetBool("Aim", isAiming);
            AimOut();
            transform.localPosition = sniperingFiflePosition;
        }




        //SpreadFactor = (isAiming) ? 0.01f : 0.1f;//Different accuracy for hip fire and aimed fire

        /* 
         * Different shooting accuracy in different states
         * Aiming: 0.005f
         * Not aiming: 0.05f
         */
        if (isAiming)//Aiming
        {
            SpreadFactor = 0.005f;
        }
        else//Not aiming
        {
            SpreadFactor = 0.05f;

        }

        //Debug.Log(SpreadFactor);



        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);//Get the current animation information
        if (info.IsName("reload_ammo_left") ||
            info.IsName("reload_out_of_ammo") ||
            info.IsName("reload_open") ||
            info.IsName("reload_close") ||
            info.IsName("reload_insert 1") ||
            info.IsName("reload_insert 2") ||
            info.IsName("reload_insert 3") ||
            info.IsName("reload_insert 4") ||
            info.IsName("reload_insert 5") ||
            info.IsName("reload_insert 6")
            )//If the reloading animation (including shotgun) is playing, then isReloading is true
        {
            isReloading = true;
        }
        else
        {
            isReloading = false;
        }

        if ((
           info.IsName("reload_insert 1") ||
           info.IsName("reload_insert 2") ||
           info.IsName("reload_insert 3") ||
           info.IsName("reload_insert 4") ||
           info.IsName("reload_insert 5") ||
           info.IsName("reload_insert 6"))
           && (currentBullets == bulletMag || bulletLeft <= 0))
        //Determine the end of the insert animation according to the number of bullet fills, and solve the problem of playing the insert animation once more
        {
            animator.Play("reload_close");
            isReloading = false;
        }




        if (Input.GetKeyDown(reloadInputName) && currentBullets < bulletMag && bulletLeft > 0 && !isReloading && Time.timeScale == 1f)//Press the reload key, the current bullet is less than the number of magazines, the reserve bullet is greater than 0, judge that there is no reloading at this time, and the current time speed is 1 to run the reload animation
        {
            DoReloadAnimation();

        }


        if (GunShootInput && currentBullets > 0)
        {
            /* The shotgun shoots 8 rays at once, and the rest of the firearms are normal 1 ray */
            if (IS_SEMIGUN && gameObject.name == "3")
            {
                gunFragment = 8;
            }
            else
            {
                gunFragment = 1;
            }

            GunFire();//Fire the gun

        }




        //Timer
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;

        }


        if (Input.GetKeyDown(inspectInputName) && Time.timeScale == 1f)//Only inspect animation when stationary, walking, and current time speed is 1
        {
            animator.SetTrigger("Inspect");
        }


        if (gameObject.name == "2")//Pistol press "E" for melee attack
        {
            if (Input.GetKeyDown(KeyCode.E) && Time.timeScale == 1f)
            {
                animator.SetTrigger("Knife_attack");
            }

        }

    }


    /// <summary>
    /// Shooting
    /// </summary>
    public override void GunFire()
    {
        /*
         * 1.Control the firing speed
         * 2.Currently no bullets
         * 3.Currently playing the take_out animation
         * 4.Reloading
         * 5.Currently playing the inspect animation
         * 6.The current game speed is not 1
         * Cannot fire
         */
        if (fireTimer < fireRate || currentBullets <= 0 || animator.GetCurrentAnimatorStateInfo(0).IsName("take_out") || isReloading || animator.GetCurrentAnimatorStateInfo(0).IsName("inspect") || Time.timeScale != 1f)//Control bullet firing speed
        {
            return;
        }



        StartCoroutine(MuzzleFlashLight());//Fire light
        muzzlePatic.Emit(1);//Emit a muzzle flame particle
        sparkPatic.Emit(Random.Range(minSparkEmission, maxSparkEmission));//Emit muzzle sparks

        StartCoroutine(Shoot_Crss());//Increase the size of the crosshair

        if (!isAiming)
        {
            animator.CrossFadeInFixedTime("fire", 0.1f);//Play the normal fire animation (using the animation fade in and out effect)
        }
        else
        {
            animator.Play("aim_fire", 0, 0);//Play the aiming fire animation when aiming
        }

        for (int i = 0; i < gunFragment; i++)
        {
            RaycastHit hit;
            Vector3 shootDirection = ShootPoint.forward;//Shoot forward
            shootDirection = shootDirection + ShootPoint.TransformDirection(new Vector3(Random.Range(-SpreadFactor, SpreadFactor), Random.Range(-SpreadFactor, SpreadFactor)));//Add an offset to the ray direction

            if (Physics.Raycast(ShootPoint.position, shootDirection, out hit, range))//Ray detection (here the ray detection method is shot from the center of the screen)
            {
                Transform bullet;
                if (IS_SEMIGUN && gameObject.name == "3")
                {
                    bullet = Instantiate(bulletPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//Special handling of the shotgun, set the bullet limit position to hit.point
                }
                else
                {
                    bullet = Instantiate(bulletPrefab, BulletShootPoint.transform.position, BulletShootPoint.transform.rotation);//Instantiate the bullet trail effect, including hit and bullet hole effects
                }

                bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;//Give the bullet trail a forward speed force (plus the offset value of the ray shot out)

                if (hit.transform.gameObject.transform.tag == "Enemy")//Judgment when hitting an enemy
                {
                    hit.transform.gameObject.GetComponent<EnemyForHummer>().Health(Random.Range(minDamage, maxDamage));
                }
                else if (hit.transform.gameObject.transform.tag == "HelicopterEnemy")//Judgment when hitting an enemy
                {
                    hit.transform.gameObject.GetComponentInParent<HelicopterEnemy>().Health(Random.Range(minDamage, maxDamage));
                }

                //Debug.Log( "Hit: " + hit.transform.gameObject.name);
            }

        }

        Instantiate(casingPrefab, CasingBulletSpawnPoint.transform.position, CasingBulletSpawnPoint.transform.rotation);//Instantiate the bullet casing


        mainAudioSource.clip = isSilencer ? soundClips.silencerShootSound : soundClips.shootSound;//Switch different shooting sound effects according to whether the silencer is equipped
        mainAudioSource.Play();//Play shooting sound effect




        fireTimer = 0f;//Reset timer
        currentBullets--;//Decrease the bullets in the magazine

        UpdateAmmoUI();

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

    /// <summary>
    /// Update bullet UI
    /// </summary>
    public void UpdateAmmoUI()
    {
        ammoTextUI.text = currentBullets + "/¡Þ";
        shootModeTextUI.text = shootModeName;

    }

    public override void DoReloadAnimation()
    {
        if (IS_SEMIGUN && (gameObject.name == "3" || gameObject.name == "4"))//Trigger the bullet change animation of shotgun or sniper rifle
        {
            if (currentBullets == bulletMag)
            {
                return;
            }
            animator.SetTrigger("shotgun_reload");
        }
        else//Trigger the bullet change animation of normal firearms
        {
            if (currentBullets > 0 && bulletLeft > 0)
            {
                animator.Play("reload_ammo_left", 0, 0);
                Reload();
                mainAudioSource.clip = soundClips.reloadSoundAmmotLeft;
                mainAudioSource.Play();

            }
            if (currentBullets == 0 && bulletLeft > 0)
            {
                animator.Play("reload_out_of_ammo", 0, 0);
                Reload();
                mainAudioSource.clip = soundClips.reloadSoundOutOfAmmo;
                mainAudioSource.Play();
            }

        }

    }


    /// <summary>
    /// Normal firearm ammunition loading logic, called in animation
    /// </summary>
    public override void Reload()
    {
        if (bulletLeft <= 0)//Skip when the reserve bullet is 0
        {
            return;
        }
        int bulletToLoad = bulletMag - currentBullets;//Calculate the number of bullets to be filled
        int bulletToReduce = bulletLeft >= bulletToLoad ? bulletToLoad : bulletLeft;//Calculate the number of reserve bullets to be deducted
        bulletLeft -= bulletToReduce;//Decrease reserve bullets
        currentBullets += bulletToReduce;//Increase current bullets
        UpdateAmmoUI();
        isReloading = true;
    }


    /// <summary>
    /// Shotgun or sniper rifle reloading logic
    /// Called in ReloadAmmoState animation
    /// </summary>
    public void ShotGunReload()
    {
        if (bulletLeft <= 0)
        {
            return;
        }

        if (currentBullets < bulletMag)
        {
            currentBullets++;
            bulletLeft--;
            UpdateAmmoUI();
        }
        else
        {
            animator.Play("reload_close");
            return;
        }
    }

    /// <summary>
    /// Enter aiming, hide the crosshair, the camera field of view becomes closer
    /// </summary>
    public override void AimIn()
    {

        for (int i = 0; i < crossQuarterlmgs.Length; i++)//Hide the crosshair
        {
            crossQuarterlmgs[i].gameObject.SetActive(false);
        }

        if (IS_SEMIGUN && gameObject.name == "4")//When the sniper rifle is aiming, change the field of view of the gunCamera and the color of the scope
        {
            scopeRenderMateriala.color = defaultColor;
            gunCamera.fieldOfView = 15;
        }


        mainCamera.fieldOfView = Mathf.SmoothDamp(30, 60, ref currentVelocity, 1f);//When aiming, the camera field of view becomes closer
        mainAudioSource.clip = soundClips.aimSound;
        mainAudioSource.Play();
        aimAllowed = false;

    }

    /// <summary>
    /// Exit aiming, hide the crosshair, the camera field of view restores
    /// </summary>
    public override void AimOut()
    {

        for (int i = 0; i < crossQuarterlmgs.Length; i++)//Restore the crosshair
        {
            crossQuarterlmgs[i].gameObject.SetActive(true);
        }

        if (IS_SEMIGUN && gameObject.name == "4")//When the sniper rifle is aiming, change the field of view of the gunCamera and the color of the scope
        {
            scopeRenderMateriala.color = fadeColor;
            gunCamera.fieldOfView = 35;
        }


        mainCamera.fieldOfView = Mathf.SmoothDamp(60, 30, ref currentVelocity, 1f);//When exiting aiming, the camera field of view becomes farther
        mainAudioSource.clip = soundClips.aimSound;
        mainAudioSource.Play();
        aimAllowed = true;


    }

    public enum ShootMode
    {
        AutoRifle,
        SemiGun
    }
}
