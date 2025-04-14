using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Ugly EnemyBaseState
/// Used to extend and implement some basic states of the enemy
/// </summary>
public abstract class EnemyBaseState : MonoBehaviour
{
    public abstract void EnemyState(Enemy enemy);//First enter the state

    public abstract void OnUpdate(Enemy enemy);//Continuously execute
}
