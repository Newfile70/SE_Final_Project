using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Weapon library
/// Character's weapon switching, adding, and removing functions
/// </summary>
public class Inventory : MonoBehaviour
{
    public List<GameObject> weapons = new List<GameObject>();//Weapon library

    public int currentWeaponID;//Weapon ID
    public bool forHelicopter;


    // Start is called before the first frame update
    void Start()
    {
        if (!forHelicopter)//There is no default value on the helicopter
        {
            currentWeaponID = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ChangeCurrentWeaponID();
    }


    /// <summary>
    /// Update weapon ID
    /// </summary>
    public void ChangeCurrentWeaponID()
    {
        //-0.1, 0, 0.1
        if (Input.GetAxis("Mouse ScrollWheel") < 0)//Switch weapons through the mouse wheel
        {
            ChangeWeapon(currentWeaponID + 1);//Next weapon
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ChangeWeapon(currentWeaponID - 1);//Previous weapon
        }

        /* Switch weapons through the numeric keypad */
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                int num = 0;
                if (i == 0)
                {
                    num = 10;
                }
                else
                {
                    num = i - 1;
                }

                if (num < weapons.Count)//Only when the number is less than the number of weapons in the list can it be further processed
                {
                    ChangeWeapon(num);
                }
            }
        }
    }

    /// <summary>
    /// Weapon switch
    /// </summary>
    /// <param name="weaponID">Weapon subscript value</param>
    public void ChangeWeapon(int weaponID)
    {
        if (weapons.Count == 0)
        {
            return;
        }


        /*
         * If you switch to the gun with the maximum index number, you will call out the first gun
         * If you switch to the gun with the smallest index number, you will call out the last gun
         */
        if (weaponID > weapons.Max(weapons.IndexOf))//IndexOf: Get the index where the element first appears in the list, Max: Take the maximum element in the list 
        {
            weaponID = weapons.Min(weapons.IndexOf);
        }
        else if (weaponID < weapons.Min(weapons.IndexOf))//Max: Take the minimum element in the list 
        {
            weaponID = weapons.Max(weapons.IndexOf);
        }

        if (weaponID == currentWeaponID)
        {
            return;
        }



        currentWeaponID = weaponID;//Update weapon index

        /* According to the weapon ID, display the corresponding weapon */
        for (int i = 0; i < weapons.Count; i++)
        {
            if (currentWeaponID == i)
            {
                weapons[i].gameObject.SetActive(true);
            }
            else
            {
                weapons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Add weapon
    /// </summary>
    public void AddWeapon(GameObject weapon)
    {
        if (weapons.Contains(weapon))
        {
            print("The collection already contains this gun");
            return;
        }
        else
        {
            if (weapons.Count < 10)
            {
                weapons.Add(weapon);//Add the corresponding weapon to the collection
                ChangeWeapon(weapons.Max(weapons.IndexOf));//Display weapon
                weapon.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Discard weapon
    /// </summary>
    public void ThrowWeapon(GameObject weapon)
    {
        if (!weapons.Contains(weapon) || weapons.Count == 0)
        {
            print("There is no this weapon, it cannot be discarded");
            return;
        }
        else
        {
            weapons.Remove(weapon);//Delete the corresponding weapon in the collection
            ChangeWeapon(currentWeaponID - 1);//Display weapon
            weapon.gameObject.SetActive(false);
        }
    }







}
