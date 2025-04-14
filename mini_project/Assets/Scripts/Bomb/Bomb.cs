using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    public GameObject player;
    public float playerDistance;
    public GameObject BombDefuseUICanvas;
    public GameObject WeaponBackpackUI;//Weapon library UI
    public Slider BombDefuseUISlider;
    public GameObject SuccessUI;//Success interface UI
    public float leftTime;//Bomb explosion time
    public GameObject ExplosionEff;//Explosion effect
    public AudioClip ExplosionSound;//Explosion sound
    private AudioSource audioSource;
    private int ExplosionTimes;
    private MeshRenderer BombMeshRenderer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //leftTime = 180f;//Bomb explosion time is 180s
        BombDefuseUICanvas.SetActive(false);
        ExplosionTimes = 0;//Explosion times
        BombMeshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = PlayerPrefs.GetFloat("AudioValue", 60f) / 60 * 0.4f;//Update volume size

        
        leftTime -= Time.deltaTime;//Update remaining explosion time when there is remaining time
        


        playerDistance = Vector3.Distance(gameObject.transform.position, player.transform.position);
        if (playerDistance < 2f && Time.timeScale != 0)//Player enters the bomb defusal area
        {
            BombDefuseUICanvas.SetActive(true);//Display bomb defusal UI
            //BombDefuseUISlider.value = 0f;
            if (Input.GetKey(KeyCode.F))//Long press "F" key to defuse the bomb
            {
                BombDefuseUISlider.value += Time.deltaTime;//Update bomb defusal progress bar
                if (BombDefuseUISlider.value >= 10f)//Bomb defusal successful
                {
                    Time.timeScale = 0f;
                    BombDefuseUICanvas.SetActive(false);
                    WeaponBackpackUI.SetActive(false);
                    SuccessUI.SetActive(true);//Display success interface
                    Cursor.lockState = CursorLockMode.None;//Mouse unlock
                }
            }
            else
            {
                BombDefuseUISlider.value = 0f;
            }
        }
        else
        {
            BombDefuseUISlider.value = 0f;
            BombDefuseUICanvas.SetActive(false);
        }

        if (leftTime <= 0f && ExplosionTimes == 0)
        {
            Explosion();
            ExplosionTimes += 1;
        }

        if (leftTime <= -0.3f && ExplosionTimes == 1)//Explosion causes damage to the player
        {
            if (playerDistance < 13f)//Must die within 13f distance
            {
                player.GetComponent<PlayerController>().PlayerHealth(100f);
            }
            else if (playerDistance > 13f && playerDistance < 23f)//Damage decreases within 13f to 23f distance
            {
                player.GetComponent<PlayerController>().PlayerHealth(100f * (23f - playerDistance) / 10f);
            }
            ExplosionTimes += 1;
        }



    }

    public void Explosion()
    {
        GameObject InstanceOfExplosion = Instantiate(ExplosionEff, gameObject.transform);//Instantiate explosion effect
        audioSource.clip = ExplosionSound;//Play explosion sound effect
        audioSource.Play();
        Destroy(InstanceOfExplosion, 2f);//Destroy explosion effect after a delay of 2s
        BombMeshRenderer.enabled = false;//Turn off the Bomb's MeshRenderer

        
    }

}

