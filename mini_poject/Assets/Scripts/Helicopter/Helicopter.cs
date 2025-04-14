using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    private Animation animation;
    private HelicopterEnemy helicopterEnemy;
    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
        helicopterEnemy = GetComponentInParent<HelicopterEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "MSH_N2")
        {
            if (helicopterEnemy.Explosion)
            {
                animation.Stop();
            }
            else
            {
                animation.Play();
            }
        }
        else
        {
            animation.Play();
        }
    }
}
