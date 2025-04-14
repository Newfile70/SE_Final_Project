using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Character controller
/// </summary>
public class PlayerController : MonoBehaviour

{
    private CharacterController characterController;
    public Vector3 moveDirection; //Set the direction of character movement
    private AudioSource audioSource;//Set sound effect

    public GameObject DeadToMenu;//Button to return to the main menu after death
    public GameObject mainTimer;
    public GameObject WeaponBackpackUI;

    [Header("Player values")]
    public float Speed;
    [Tooltip("Walking speed")] public float walkSpeed;
    [Tooltip("Running speed")] public float runSpeed;
    [Tooltip("Crouch walking speed")] public float crouchSpeed;
    [Tooltip("Player health")] public float playerHealth;

    [Tooltip("Jump force")] public float jumpForce;
    [Tooltip("Fall force")] public float fallForce;
    [Tooltip("Player height when crouching")] public float crouchHeight;
    [Tooltip("Player height when standing normally")] public float standHeight;



    [Header("Key settings")]
    [Tooltip("Run key")] public KeyCode runInputName = KeyCode.LeftShift;//Set run key
    [Tooltip("Jump key")] public KeyCode jumpInputName = KeyCode.Space;
    [Tooltip("Crouch key")] public KeyCode crouchInputName = KeyCode.LeftControl;

    [Header("Player attribute judgment")]
    public MovementState state;
    private CollisionFlags collisionFlags;
    public bool isWalk;//Determine if the player is walking
    public bool isRun;//Determine if the player is running
    public bool isJump;//Determine if the player is jumping
    public bool isGround;//Determine if the player is on the ground
    public bool isCanCrouch;//Determine if the player can crouch
    public bool isCrouching;//Determine if the player is crouching
    public bool playerIsDead;//Determine if the player is dead
    private bool isDamage;//Determine if the player is damaged

    public LayerMask crouchLayerMask;//Layer
    public Text playerHealthUIText;
    public Slider playerHealthUIBar;
    public Image hurtImage;//Player blood fog effect
    private Color flashColor = Color.red;
    private Color clearColor = Color.clear;

    [Header("Sound effects")]
    [Tooltip("Walking sound effect")] public AudioClip walkingSound;
    [Tooltip("Running sound effect")] public AudioClip runningSound;


    private Inventory inventory;

    private bool aimAllowed;

    void Awake()
    {
        Application.targetFrameRate = 90;//Set frame rate to 90
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();//Get sound effect
        inventory = GetComponentInChildren<Inventory>();
        aimAllowed = true;
        playerHealth = 100f;
        Time.timeScale = 1f;

        /* Set speed values */
        walkSpeed = 4f;
        runSpeed = 7.5f;
        crouchSpeed = 2f;
        jumpForce = 0f;
        fallForce = 10f;
        crouchHeight = 1f;
        standHeight = characterController.height;
        playerHealthUIText.text = "Health: " + playerHealth;
        playerHealthUIBar.value = 100f - playerHealth;

        DynamicGI.UpdateEnvironment();//Update lighting environment to prevent darkening when switching scenes

    }


    // Update is called once per frame
    void Update()
    {
        audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.4f;//Update volume size

        if (isDamage)//Blood fog effect when the player is damaged
        {
            hurtImage.color = flashColor;
        }
        else
        {
            hurtImage.color = Color.Lerp(hurtImage.color, clearColor, Time.deltaTime * 5);//Blood fog effect fades
        }
        isDamage = false;


        if (playerIsDead)//Stop all movement behavior after the player dies
        {
            audioSource.Pause();
            return;
        }


        CanCrouch();
        if (Input.GetKey(crouchInputName))
        {
            Crouch(true);

        }
        else
        {
            Crouch(false);
        }

        Jump();
        PlayerFootSoundSet();
        Moving();





    }

    /// <summary>
    /// Character movement
    /// <summary>

    public void Moving()
    {
        /* Get keyboard value and map it to h and v */
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Debug.Log(inventory.weapons.Count);

        if (inventory.weapons.Count != 0)//If the number of weapons in the weapon library is not 0
        {
            aimAllowed = GetComponentInChildren<Weapon_AutomaticGun>().aimAllowed;
        }

        isRun = Input.GetKey(runInputName) && Vector3.SqrMagnitude(moveDirection) > 0;//Determine if running
        isWalk = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0) ? true : false;//Determine if walking
        if (isRun && isGround && isCanCrouch && !isCrouching && aimAllowed)//Running
        {
            state = MovementState.running;
            Speed = runSpeed;
        }
        else if (isCrouching)//Crouch walking
        {
            state = MovementState.crouching;
            Speed = crouchSpeed;

        }
        else if (isGround)//Normal walking
        {
            state = MovementState.walking;
            Speed = walkSpeed;

        }





        /* Set the direction of character movement, and normalize the speed to prevent the speed from increasing when walking diagonally */
        moveDirection = (transform.right * h + transform.forward * v).normalized;
        characterController.Move(moveDirection * Speed * Time.deltaTime);//Character movement
    }


    /// <summary>
    /// Jump
    /// </summary>
    public void Jump()
    {
        if (!isCanCrouch) return;
        isJump = Input.GetKeyDown(jumpInputName);//Determine if the jump key is pressed
        /* Determine if the player presses the jump key, and if it is on the ground at this time, it can jump */
        if (isJump && isGround)
        {
            isGround = false;
            jumpForce = 5f;//Set jump force

        }

        /* If the space jump is not currently pressed and it is detected on the ground, then isGround is judged to be false */
        else if (!isJump && isGround)
        {
            isGround = false;
        }

        /* At this time, the jump key is pressed and jumped up, and it is not on the ground */
        if (!isGround)
        {
            jumpForce = jumpForce - fallForce * Time.deltaTime;// Every second, the jump force is accumulated and reduced, causing the player to fall
            Vector3 jump = new Vector3(0, jumpForce * Time.deltaTime, 0);//Convert the jump force to V3 coordinates
            collisionFlags = characterController.Move(jump);//Call the character controller move method, the upward method simulates jumping

            /* Determine if the player is on the ground
             CollisionFlags:characterController built-in collision position identification number
             CollisionFlags.Below --> On the ground
             */
            if (collisionFlags == CollisionFlags.Below)
            {
                isGround = true;
                jumpForce = -2f;
            }

            /* Supplementary judgment, if the current character is not hit by anything, it means it is not on the ground */
            //if (isGround && collisionFlags == CollisionFlags.None)
            //{
            //    isGround = false;
            //}

        }

    }


    /// <summary>
    /// Determine whether the character can crouch
    /// isCanCrouch == true -->Indicates that the character can crouch, the character is standing at this time
    /// isCanCrouch == false -->Indicates that the character cannot crouch, the character is crouching at this time, there is a collision on the top of the head
    /// </summary>
    public void CanCrouch()
    {

        /* Get the height V3 position on the top of the character */
        Vector3 sphereLocation = transform.position + new Vector3(0, 0.3f, 0) + Vector3.up * standHeight;//Get the current position of the player and add the crouching height
        isCanCrouch = (Physics.OverlapSphere(sphereLocation, characterController.radius, crouchLayerMask).Length) == 0;//Generate a spherical collision detection with a radius of characterController size at the top of the player's head, generate its (array) length, judge whether the number is 0, if it is 0, there is no object collision on the top of the head

        //Debug.Log("isCanCrouch: " + isCanCrouch);

    }

    /// <summary>
    /// Crouch
    /// </summary>
    public void Crouch(bool newCrouching)
    {
        if (!isCanCrouch) return;//Cannot crouch (in the tunnel), cannot stand
        isCrouching = newCrouching;

        characterController.height = isCrouching ? crouchHeight : standHeight;//Set the height of crouching and standing according to the crouching state
        characterController.center = characterController.height / 2.0f * new Vector3(0, 1, 0);//The center position Y of characherController is reduced by half of the height from the head



    }

    /// <summary>
    /// Character movement sound effect
    /// </summary>
    public void PlayerFootSoundSet()
    {
        if (isGround && moveDirection.sqrMagnitude > 0)
        {
            audioSource.clip = isRun ? runningSound : walkingSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();//Play walking or running sound effect
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Pause();//Pause walking or running sound effect
        }
        if (isCrouching)//Do not play sound effects when crouching
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }

    }

    /// <summary>
    /// Pick up weapon
    /// </summary>
    public void PickUpWeapon(int itemID, GameObject weapon)
    {
        /* After picking up the weapon, add it to the weapon library, if it already exists, replenish the bullets */
        if (inventory.weapons.Contains(weapon))
        {
            weapon.GetComponent<Weapon_AutomaticGun>().bulletLeft = weapon.GetComponent<Weapon_AutomaticGun>().bulletMag * 3;
            weapon.GetComponent<Weapon_AutomaticGun>().currentBullets = weapon.GetComponent<Weapon_AutomaticGun>().bulletMag;
            weapon.GetComponent<Weapon_AutomaticGun>().UpdateAmmoUI();
            Debug.Log("The collection already contains this gun, replenish bullets");
            return;
        }
        else
        {
            inventory.AddWeapon(weapon);
        }
    }


    /// <summary>
    /// Player health
    /// </summary>
    /// <param name="damage">Damage received</param>>
    public void PlayerHealth(float damage)
    {
        playerHealth -= damage;
        playerHealth = Mathf.Round(playerHealth);//Round off the blood volume
        isDamage = true;
        playerHealthUIText.text = "Health: " + playerHealth;//Update Health Text
        playerHealthUIBar.value = 100f - playerHealth;//Update blood volume bar
        if (playerHealth <= 0)
        {
            playerIsDead = true;
            playerHealthUIText.text = "Player Dead";
            Time.timeScale = 0;//Game pause
            DeadToMenu.SetActive(true);
            mainTimer.SetActive(false);
            WeaponBackpackUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;

        }
    }



    public enum MovementState
    {
        walking,
        running,
        crouching,
        idle
    }
}

