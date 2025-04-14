using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Internal class for weapon sound effects
/// </summary>
[System.Serializable]
public class SoundClips
{
    public AudioClip shootSound;//Fire sound effect
    public AudioClip silencerShootSound;//Fire sound effect with silencer
    public AudioClip reloadSoundAmmotLeft;//Reload bullet sound effect
    public AudioClip reloadSoundOutOfAmmo;//Reload bullets and pull the bolt (a magazine is finished)
    public AudioClip aimSound;
}



public class Weapon_AutomaticGun : Weapon
{
    private Animator animator;
    private PlayerController playerController;
    private Camera mainCamera;//Main camera
    public Camera gunCamera;//Gun model camera

    public bool IS_AUTORIFLE;//Is it an automatic weapon
    public bool IS_SEMIGUN;//Is it a semi-automatic weapon

    [Header("Weapon part position")]
    [Tooltip("Shooting position")] public Transform ShootPoint;//The position where the ray is shot
    public Transform BulletShootPoint;//The position where the bullet effect is shot
    [Tooltip("Position where the bullet shell is thrown out")] public Transform CasingBulletSpawnPoint;

    [Header("Bullet prefab and special effects")]
    public Transform bulletPrefab;//Bullet
    public Transform casingPrefab;//Bullet shell


    [Header("Gun attributes")]
    [Tooltip("Weapon range")] public float range;
    private float fireRate;
    [Tooltip("Weapon firing speed")] public float originRate;//Original firing speed
    private float SpreadFactor;//A little offset of the shot
    private float fireTimer;//Timer to control weapon firing speed
    private float bulletForce;//The force of bullet firing
    [Tooltip("Number of bullets in each magazine of the weapon")] public int bulletMag;
    [Tooltip("Current number of bullets")] public int currentBullets;
    [Tooltip("Backup bullets")] public int bulletLeft;
    public bool isSilencer;//Is the silencer equipped
    private int gunFragment;//The number of bullets fired by the gun at once
    public float minDamage;
    public float maxDamage;

    [Header("Special effects")]
    public Light muzzleflashLight;//Fire light
    private float lightDuraion;//Light duration
    public ParticleSystem muzzlePatic;//Light flame particle effect 1
    public ParticleSystem sparkPatic;//Light flame particle effect 2 (Mars)
    public int minSparkEmission = 1;//The random spread value of Mars
    public int maxSparkEmission = 7;

    [Header("Audio source")]
    private AudioSource mainAudioSource;
    public SoundClips soundClips;

    [Header("UI")]
    public Image[] crossQuarterlmgs;//Crosshair
    public float currentExpanedDegree;//The current opening degree of the crosshair
    private float crossExpanedDegree;//Crosshair opening degree per frame
    private float maxCrossDegree;//Maximum opening degree
    public Text ammoTextUI;//Number of bullets
    public Text shootModeTextUI;//Shooting mode

    public PlayerController.MovementState state;
    private bool isReloading;//Determine whether it is reloading
    private bool isAiming;//Determine whether it is aiming
    public bool aimAllowed;//Auxiliary judgment whether to execute AimIn or AimOut

    private Vector3 sniperingFiflePosition;//The default initial position of the gun
    public Vector3 sniperingFifleOnPosition;//The model position of the gun when aiming is turned on


    [Header("Key settings")]
    [SerializeField] [Tooltip("Reload bullet key")] private KeyCode reloadInputName = KeyCode.R;
    [SerializeField] [Tooltip("Check weapon key")] private KeyCode inspectInputName = KeyCode.I;
    [SerializeField] [Tooltip("Automatic semi-automatic switching key")] private KeyCode GunShootModelInputName = KeyCode.X;





    public ShootMode shootingMode;//Use enumeration to distinguish shooting mode (automatic and semi-automatic)
    private bool GunShootInput;//According to fully automatic and semi-automatic, the key input of shooting changes
    private int modeNum;//An intermediate parameter for mode switching (1: fully automatic mode, 2: semi-automatic mode)
    private string shootModeName;
    float currentVelocity = 0f;


    [Header("Sniper scope settings")]
    [Tooltip("Sniper scope material")] public Material scopeRenderMateriala;
    [Tooltip("Color of the sniper scope when the sniper gun is not aiming")] public Color fadeColor;
    [Tooltip("Color of the sniper scope when the sniper gun is aiming")] public Color defaultColor;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
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
        if (gameObject.name == "4")// The sniper rifle has a greater firing force and a longer range
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

        /* Set different shooting modes at the beginning of the game based on different firearms */
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
        if (playerController.playerIsDead)// Player is dead
        {
            for (int i = 0; i < crossQuarterlmgs.Length; i++)// Hide the crosshair
            {
                crossQuarterlmgs[i].gameObject.SetActive(false);
            }
            ammoTextUI.gameObject.SetActive(false);// Hide the bullet count
            shootModeTextUI.gameObject.SetActive(false);// Hide the shooting mode
            mainAudioSource.Pause();
        }

        mainAudioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.2f;// Update the volume size

        /* Automatic firearms mouse input method, can switch between GetMouseButton and GetMouseButtonDown */
        if (IS_AUTORIFLE)
        {
            /* Switch shooting mode (semi-automatic and automatic) *//*
            if (Input.GetKeyDown(GunShootModelInputName) && modeNum != 1 )
            {
                modeNum = 1;
                shootModeName = "全自动";
                shootingMode = ShootMode.AutoRifle;
                UpdateAmmoUI();
            }
            else if (Input.GetKeyDown(GunShootModelInputName) && modeNum != 0)
            {
                modeNum = 0;
                shootModeName = "半自动";
                shootingMode = ShootMode.SemiGun;
                UpdateAmmoUI();
            }*/

            /* Control the conversion of shooting modes, and later you need to use code to control dynamically */


            GunShootInput = Input.GetMouseButton(0);
            fireRate = originRate;



        }
        else
        {
            GunShootInput = Input.GetMouseButtonDown(0);
            if (gameObject.name == "2") fireRate = 0.2f;
            else if (gameObject.name == "4") fireRate = 1.2f;
        }







        state = playerController.state;// Here, the movement state of the character is obtained in real time (walking, running, squatting)
        if (state == PlayerController.MovementState.walking && Vector3.SqrMagnitude(playerController.moveDirection) > 0 && state != PlayerController.MovementState.running && state != PlayerController.MovementState.crouching)
        {
            ExpandingCrossUpdate(crossExpanedDegree);// Crosshair opening degree when moving
        }
        else if (state != PlayerController.MovementState.walking && Vector3.SqrMagnitude(playerController.moveDirection) > 0 && state == PlayerController.MovementState.running && state != PlayerController.MovementState.crouching)
        {
            ExpandingCrossUpdate(crossExpanedDegree * 2);// Crosshair opening degree when running (2 times)
        }
        else
        {
            ExpandingCrossUpdate(0);// Do not adjust the crosshair opening degree when standing or squatting
        }


        if (Input.GetMouseButton(1) && !isReloading && !playerController.isRun && aimAllowed && Time.timeScale == 1f)// Enter aiming (cannot enter when reloading, running, time is not 1)
        {
            isAiming = true;
            animator.SetBool("Aim", isAiming);
            AimIn();
            transform.localPosition = sniperingFifleOnPosition;// When aiming, you need to slightly adjust the position of the gun model
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
         * Aimed crouch still: 0.01f
         * Aimed still: 0.02f
         * Aimed crouch walking: 0.03f
         * Aimed walking: 0.04f
         * Unaimed crouch still: 0.05f
         * Unaimed still: 0.06f
         * Unaimed crouch walking: 0.08f
         * Unaimed walking: 0.1f
         */
        if (isAiming && gameObject.name != "4")//Aiming
        {
            if (playerController.isWalk)//Moving
            {
                if (playerController.isCrouching)//Crouch walking
                {
                    SpreadFactor = 0.03f;
                }
                else//Walking
                {
                    SpreadFactor = 0.04f;
                }

            }
            else//Still
            {
                if (playerController.isCrouching)//Crouch still
                {
                    SpreadFactor = 0.01f;
                }
                else//Still
                {
                    SpreadFactor = 0.02f;
                }
            }
        }
        else if (!isAiming && gameObject.name != "4")
        {
            if (playerController.isWalk)//Moving
            {
                if (playerController.isCrouching)//Crouch walking
                {
                    SpreadFactor = 0.08f;
                }
                else//Walking
                {
                    SpreadFactor = 0.1f;
                }

            }
            else//Still
            {
                if (playerController.isCrouching)//Crouch still
                {
                    SpreadFactor = 0.05f;
                }
                else//Still
                {
                    SpreadFactor = 0.06f;
                }
            }
        }

        /* 
        * Different shooting accuracy for sniper rifle in different states
        * Aimed: 0.005f
        * Unaimed: 0.05f
        */
        if (isAiming && gameObject.name == "4")//Aiming
        {
            SpreadFactor = 0.005f;
        }
        else if (!isAiming && gameObject.name == "4")
        {
            SpreadFactor = 0.05f;

        }




        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);//Get current animation information
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
        //End the insert animation based on the number of bullets loaded to solve the problem of playing the insert animation once more
        {
            animator.Play("reload_close");
            isReloading = false;
        }





        if (Input.GetKeyDown(reloadInputName) && currentBullets < bulletMag && bulletLeft > 0 && !isReloading && Time.timeScale == 1f)//Press the reload key, the current bullet is less than the magazine number, the spare bullet is greater than 0, judge that there is no reloading at this time, and the current time speed is 1 to run the reload animation
        {
            DoReloadAnimation();

        }


        if (GunShootInput && currentBullets > 0)
        {
            /* The shotgun shoots 8 rays at the same time, and the rest of the firearms are normal 1 ray */
            if (IS_SEMIGUN && gameObject.name == "3")
            {
                gunFragment = 8;
            }
            else
            {
                gunFragment = 1;
            }

            GunFire();//Gun shooting

        }




        //Timer
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;

        }


        /* Play walking, running, inspection animation */
        animator.SetBool("Run", playerController.isRun);
        animator.SetBool("Walk", playerController.isWalk);
        if (Input.GetKeyDown(inspectInputName) && Time.timeScale == 1f)//Only inspect animation when stationary, walking state and current time speed is 1
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk") || animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            {
                animator.SetTrigger("Inspect");
            }
        }


        if (gameObject.name == "2")//Pistol press "E" melee attack
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
         * 1. Control the rate of fire
         * 2. No bullets currently
         * 3. Currently playing take_out animation
         * 4. Reloading
         * 5. Currently playing inspect animation
         * 6. Currently running
         * 7. The current game speed is not 1
         * Just can't fire
         */
        if (fireTimer < fireRate || currentBullets <= 0 || state == PlayerController.MovementState.running || animator.GetCurrentAnimatorStateInfo(0).IsName("take_out") || isReloading || animator.GetCurrentAnimatorStateInfo(0).IsName("inspect") || Time.timeScale != 1f)//Control the speed of bullet shooting
        {
            return;
        }



        StartCoroutine(MuzzleFlashLight());//Fire light
        muzzlePatic.Emit(1);//Emit a muzzle flame particle
        sparkPatic.Emit(Random.Range(minSparkEmission, maxSparkEmission));//Emit muzzle spark

        StartCoroutine(Shoot_Crss());//Increase the size of the crosshair

        if (!isAiming)
        {
            animator.CrossFadeInFixedTime("fire", 0.1f);//Play normal fire animation (using animation fade in and out effect)
        }
        else
        {
            animator.Play("aim_fire", 0, 0);//Play aimed fire animation when aiming
        }

        for (int i = 0; i < gunFragment; i++)
        {
            RaycastHit hit;
            Vector3 shootDirection = ShootPoint.forward;//Shoot forward
            shootDirection = shootDirection + ShootPoint.TransformDirection(new Vector3(Random.Range(-SpreadFactor, SpreadFactor), Random.Range(-SpreadFactor, SpreadFactor)));//Add offset to the direction of the ray

            if (Physics.Raycast(ShootPoint.position, shootDirection, out hit, range))//Ray detection (here the ray detection method is shot from the center of the screen)
            {
                Transform bullet;
                if (IS_SEMIGUN && gameObject.name == "3")
                {
                    bullet = Instantiate(bulletPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//Special handling of shotgun, set the bullet limit position to hit.point
                }
                else
                {
                    bullet = Instantiate(bulletPrefab, BulletShootPoint.transform.position, BulletShootPoint.transform.rotation);//Instance shoots bullet tail effect, including hit and bullet hole effect
                }

                bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + shootDirection) * bulletForce;//Give the bullet tail a forward speed force (plus the offset value of the ray shot out)

                if (hit.transform.gameObject.transform.tag == "Enemy")//Judgment when hitting an enemy
                {
                    hit.transform.gameObject.GetComponent<Enemy>().Health(Random.Range(minDamage, maxDamage));
                }

                Debug.Log("Hit: " + hit.transform.gameObject.name);
            }

        }


        Instantiate(casingPrefab, CasingBulletSpawnPoint.transform.position, CasingBulletSpawnPoint.transform.rotation);//Instantiate the shell casing


        mainAudioSource.clip = isSilencer ? soundClips.silencerShootSound : soundClips.shootSound;//Switch different shooting sound effects based on whether the silencer is equipped
        mainAudioSource.Play();//Play shooting sound effect




        fireTimer = 0f;//Reset timer
        currentBullets--;//Decrease bullets in the magazine

        UpdateAmmoUI();

    }

    /// <summary>
    /// Set the light of firing
    /// </summary>
    public IEnumerator MuzzleFlashLight()
    {
        muzzleflashLight.enabled = true;
        yield return new WaitForSeconds(lightDuraion);
        muzzleflashLight.enabled = false;
    }


    /// <summary>
    /// Increase or decrease the opening degree of the crosshair according to the specified size
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
    ///Change the opening degree of the crosshair and record the current opening degree of the crosshair 
    /// </summary>
    public void ExpendCross(float add)
    {
        crossQuarterlmgs[0].transform.localPosition += new Vector3(-add, 0, 0);//Left crosshair
        crossQuarterlmgs[1].transform.localPosition += new Vector3(add, 0, 0);//Right crosshair
        crossQuarterlmgs[2].transform.localPosition += new Vector3(0, add, 0);//Upper crosshair
        crossQuarterlmgs[3].transform.localPosition += new Vector3(0, -add, 0);//Lower crosshair

        currentExpanedDegree += add;//Save the current crosshair opening degree
        currentExpanedDegree = Mathf.Clamp(currentExpanedDegree, 0, maxCrossDegree);//Limit the size of the crosshair opening degree  

    }
    /// <summary>
    /// Coroutine, call the crosshair opening degree, execute 5 times in 1 frame
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
        ammoTextUI.text = currentBullets + "/" + bulletLeft;
        shootModeTextUI.text = shootModeName;

    }

    public override void DoReloadAnimation()
    {
        if (IS_SEMIGUN && (gameObject.name == "3" || gameObject.name == "4"))//Trigger shotgun or sniper rifle reload animation
        {
            if (currentBullets == bulletMag)
            {
                return;
            }
            animator.SetTrigger("shotgun_reload");
        }
        else//Trigger normal firearm reload animation
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
        if (bulletLeft <= 0)//Skip when the spare bullet is 0
        {
            return;
        }
        int bulletToLoad = bulletMag - currentBullets;//Calculate the number of bullets to be filled
        int bulletToReduce = bulletLeft >= bulletToLoad ? bulletToLoad : bulletLeft;//Calculate the number of spare bullets to be deducted
        bulletLeft -= bulletToReduce;//Spare bullets decrease
        currentBullets += bulletToReduce;//Current bullets increase
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
    /// Enter aiming, hide the crosshair, the camera's field of view becomes closer
    /// </summary>
    public override void AimIn()
    {

        for (int i = 0; i < crossQuarterlmgs.Length; i++)//Hide the crosshair
        {
            crossQuarterlmgs[i].gameObject.SetActive(false);
        }

        if (IS_SEMIGUN && gameObject.name == "4")//When the sniper rifle is aiming, change the field of view of gunCamera and the color of the scope
        {
            scopeRenderMateriala.color = defaultColor;
            gunCamera.fieldOfView = 15;
        }


        mainCamera.fieldOfView = Mathf.SmoothDamp(30, 60, ref currentVelocity, 1f);//The camera's field of view becomes closer when aiming
        mainAudioSource.clip = soundClips.aimSound;
        mainAudioSource.Play();
        aimAllowed = false;

    }

    /// <summary>
    /// Exit aiming, hide the crosshair, the camera's field of view is restored
    /// </summary>
    public override void AimOut()
    {

        for (int i = 0; i < crossQuarterlmgs.Length; i++)//Restore the crosshair
        {
            crossQuarterlmgs[i].gameObject.SetActive(true);
        }

        if (IS_SEMIGUN && gameObject.name == "4")//When the sniper rifle is aiming, change the field of view of gunCamera and the color of the scope
        {
            scopeRenderMateriala.color = fadeColor;
            gunCamera.fieldOfView = 35;
        }


        mainCamera.fieldOfView = Mathf.SmoothDamp(60, 30, ref currentVelocity, 1f);//The camera's field of view becomes farther when exiting aiming
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

