using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy enters attack state
/// </summary>
public class AttackState : EnemyBaseState
{
    public override void EnemyState(Enemy enemy)
    {
        enemy.animState = 2;
        enemy.targetPoint = enemy.attackList[0];
    }

    public override void OnUpdate(Enemy enemy)
    {

        if (enemy.attackList.Count == 0)//When the current enemy has no target, the enemy switches back to patrol state
        {
            enemy.TranstionToState(enemy.patrolState);
        }


        if (enemy.attackList.Count > 1) //The current enemy has a target, there may be multiple targets, you need to find the nearest attack target
        {
            for (int i = 0; i < enemy.attackList.Count; i++)
            {
                if (Mathf.Abs(enemy.transform.position.x - enemy.attackList[i].position.x) <
                Mathf.Abs(enemy.transform.position.x - enemy.targetPoint.position.x))//Compare the distance between the enemy and the i-th target in the attack list with the distance between the enemy and the current attack target. If it is smaller, update the i-th target as the current attack target
                {
                    enemy.targetPoint = enemy.attackList[i];
                }
            }
        }


        if (enemy.attackList.Count == 1) //When the enemy only has one attack target, just find the first one in the List
        {
            enemy.targetPoint = enemy.attackList[0];
        }


        if (enemy.targetPoint.tag == "Player")
        {
            enemy.AttackAction();
        }


        enemy.MoveToTaget();
    }
}

