using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    public float MAX_Damage;
    public float MIN_Damage;

    public void Start()
    {
        if (PlayerPrefs.GetString("Difficulties") == "Easy")
        {
            MAX_Damage = MAX_Damage * 0.6f;
            MIN_Damage = MIN_Damage * 0.6f;
        }
        else if (PlayerPrefs.GetString("Difficulties") == "Normal")
        {
            MAX_Damage = MAX_Damage * 1f;
            MIN_Damage = MIN_Damage * 1f;
        }
        else if (PlayerPrefs.GetString("Difficulties") == "Hard")
        {
            MAX_Damage = MAX_Damage * 1.5f;
            MIN_Damage = MIN_Damage * 1.5f;
        }
    }

    private void OnTriggerEnter(Collider other)//Player takes damage and loses health
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().PlayerHealth(Random.Range(MIN_Damage, MAX_Damage));
        }
    }
}
