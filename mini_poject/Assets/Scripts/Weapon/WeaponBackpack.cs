using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBackpack : MonoBehaviour
{
    public GameObject inventoryGameObject;
    public GameObject WeaponBackpackUI;
    private Inventory inventory;
    private List<GameObject> weaponList;
    public List<GameObject> weaponImages = new List<GameObject>();
    public List<GameObject> position = new List<GameObject>();
    public List<GameObject> weaponBackgroundImages = new List<GameObject>();
    public List<GameObject> changeWeaponButtonGameObject = new List<GameObject>();
    public List<Button> changeWeaponButton = new List<Button>();
    public List<Color> buttonColor = new List<Color>(6);
    private int currentWeaponID;//Current weapon ID
    // Start is called before the first frame update
    void Start()
    {
        inventory = inventoryGameObject.GetComponent<Inventory>();
        WeaponBackpackUI.SetActive(false);
        for (int i = 0; i < changeWeaponButton.Count; i++)
        {
            buttonColor[i] = changeWeaponButton[i].GetComponent<Button>().colors.normalColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        weaponList = inventory.weapons;//Get the weapon list in the Inventory weapon library
        currentWeaponID = inventory.currentWeaponID;

        if (Input.GetKeyDown(KeyCode.Tab) && Time.timeScale == 1)//Press and hold the Tab key to open the weapon library, slow down the time, unlock the cursor (cannot be opened in the interface where the game time is 0)
        {
            WeaponBackpackUI.SetActive(true);
            SetWeaponImages();
            Time.timeScale = 0.05f;
            Cursor.lockState = CursorLockMode.None;
        }

        else if (Input.GetKeyUp(KeyCode.Tab) && WeaponBackpackUI.activeSelf)
        {
            WeaponBackpackUI.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }

        ChangeButtonColor();


    }

    /// <summary>
    /// Place the texture of the weapon in the weapon library on the correct Weapon position
    /// </summary>
    public void SetWeaponImages()
    {
        for (int i = 0; i < weaponList.Count; i++)
        {
            for (int j = 0; j < weaponImages.Count; j++)
            {
                if (weaponList[i].name == weaponImages[j].name)
                {
                    weaponImages[j].transform.position = position[i].transform.position;
                    weaponImages[j].SetActive(true);
                    weaponBackgroundImages[i].SetActive(false);
                    changeWeaponButtonGameObject[i].SetActive(true);
                }
            }
        }
    }

    public void ChangeButtonColor()//Change the color transparency of the weapon library selection box
    {
        for (int i = 0; i < changeWeaponButton.Count; i++)
        {
            if (currentWeaponID == i)
            {
                TransparencyCtrl(buttonColor[i], changeWeaponButton[i], 0.862745098f);
            }
            else
            {
                TransparencyCtrl(buttonColor[i], changeWeaponButton[i], 0.2745098039f);
            }
        }
    }

    public void TransparencyCtrl(Color color, Button button, float transparency)//Change the transparency of the difficulty key
    {
        color.a = transparency;
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        button.colors = cb;
    }
}
