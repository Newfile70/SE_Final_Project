using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifePoint : MonoBehaviour
{
    public int MAX_Damage;
    public int MIN_Damage;

    private void OnTriggerEnter(Collider other)//Knife hits the enemy
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().Health(Random.Range(MIN_Damage, MAX_Damage));
            Debug.Log("Hit it");
        }

    }
}
