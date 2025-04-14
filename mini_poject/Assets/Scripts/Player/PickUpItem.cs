using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon pickup
/// </summary>
public class PickUpItem : MonoBehaviour
{
    [Tooltip("Weapon rotation speed")] private float rotateSpeed;
    [Tooltip("Weapon ID")] public int itemID;
    public Collider pickCollider;//Pickup collider
    private GameObject weaponModel;
    private GameObject player;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        distance = 1.15f;
        if (GameObject.Find("Player") == null) //On the helicopter
        {
            player = GameObject.Find("HPlayer/Helicopter/Player");
            distance = 3f;
        }
        pickCollider.enabled = false;
        rotateSpeed = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) < distance)//When the player is close to the weapon, the collider takes effect to prevent the weapon collider from blocking the bullet
        {
            pickCollider.enabled = true;
        }
        else
        {
            pickCollider.enabled = false;
            //Debug.Log(Vector3.Distance(gameObject.transform.position, player.transform.position));
        }

        transform.eulerAngles += new Vector3(0, rotateSpeed * Time.deltaTime, 0);//Let the weapon rotate automatically
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            weaponModel = GameObject.Find("Player/Assult_Rifle_Arm/Inventory").gameObject.transform.GetChild(itemID).gameObject;//Find and get each weapon object under the Inventory object
            //Debug.Log(weaponModel.name);
            player.PickUpWeapon(itemID, weaponModel);//Call method, add and display weapon
            Destroy(gameObject);//Disappear after pickup
        }
    }



}
