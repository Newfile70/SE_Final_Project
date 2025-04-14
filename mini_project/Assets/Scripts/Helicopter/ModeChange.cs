using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeChange : MonoBehaviour
{
    public GameObject[] fistEnemy;
    public GameObject[] secondEnemy;
    private HelicopterMove helicopterMove;
    private bool bornSencondEnemy;
    public GameObject WeaponBackpackUI;//Weapon library UI
    public GameObject SuccessUI;//Success interface UI
    // Start is called before the first frame update
    void Start()
    {
        helicopterMove = GetComponentInParent<HelicopterMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!helicopterMove.canLanding)
        {
            int DeadEnemy = 0;
            for (int i = 0; i < fistEnemy.Length; i++)//Traverse the fistEnemy list, if any element is null, it means this enemy is dead
            {
                if (fistEnemy[i] == null)
                {
                    DeadEnemy += 1;
                }
            }

            if (DeadEnemy == fistEnemy.Length)//When the number of dead enemies equals the number of generated enemies, it means all enemies have been cleared
            {
                helicopterMove.canLanding = true;
            }
        }

        if (helicopterMove.playerExit)//Generate enemies for the second time after the player leaves the plane
        {
            if (!bornSencondEnemy)//Only show the second enemy once
            {
                for (int i = 0; i < secondEnemy.Length; i++)
                {
                    secondEnemy[i].SetActive(true);
                }
                bornSencondEnemy = true;
            }

            int DeadEnemy = 0;
            for (int i = 0; i < secondEnemy.Length; i++)//Traverse the secondEnemy list, if any element is null, it means this enemy is dead
            {
                if (secondEnemy[i] == null)
                {
                    DeadEnemy += 1;
                }
            }

            if (DeadEnemy == secondEnemy.Length)//When the number of dead enemies equals the number of generated enemies, it means all enemies have been cleared
            {
                Time.timeScale = 0f;
                WeaponBackpackUI.SetActive(false);
                SuccessUI.SetActive(true);//Display success interface
                Cursor.lockState = CursorLockMode.None;//Unlock the mouse
                if (PlayerPrefs.GetInt("Battle") <= 1) PlayerPrefs.SetInt("Battle", 1);//Set to have completed battle1
            }
        }
    }
}
