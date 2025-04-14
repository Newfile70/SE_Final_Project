using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon pickup
/// </summary>
public class MedicalKit : MonoBehaviour
{
    [Tooltip("Weapon rotation speed")] private float rotateSpeed;

    public Collider pickCollider;//Pickup collider
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pickCollider.enabled = false;
        rotateSpeed = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) < 1.15f)//When the player is close to the medical kit, the collider takes effect to prevent the medical kit collider from blocking the bullet
        {
            pickCollider.enabled = true;
        }
        else
        {
            pickCollider.enabled = false;
        }

        transform.eulerAngles += new Vector3(0, rotateSpeed * Time.deltaTime, 0);//Let the medical kit rotate automatically
    }

    private void OnTriggerEnter(Collider other)//Player blood recovery
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.playerHealth = 100f;
            player.playerHealthUIText.text = "Health: 100";//Update Health Text
            player.playerHealthUIBar.value = 0f;//Update blood volume bar
            Destroy(gameObject);//Disappear after pickup
        }
    }



}
