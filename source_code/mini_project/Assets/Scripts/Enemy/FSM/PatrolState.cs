using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy enters patrol state
/// </summary>
public class PatrolState : EnemyBaseState
{
    public override void EnemyState(Enemy enemy)
    {
        enemy.animState = 0;
        enemy.LoadPath(enemy.wayPointObj[WayPointManager.Instance.usingIndex[enemy.nameIndex]]);//Randomly load the route
    }

    public override void OnUpdate(Enemy enemy)
    {
        if (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))//Cannot play Walk animation when playing idle animation
        {
            enemy.animState = 1;//Play Walk animation
            enemy.MoveToTaget();//The enemy moves to the target navigation point
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.wayPoints[enemy.index]);//Calculate the distance between the enemy and the navigation point
        if (distance <= 0.5f)//When the distance is very small, it means that it has reached the navigation point
        {
            //enemy.animState = 0;//The enemy stops and plays the idle animation when it reaches the navigation point
            enemy.index++;//Set the next navigation point
            enemy.index = Mathf.Clamp(enemy.index, 0, enemy.wayPoints.Count - 1);//Limit the size of index to prevent overflow
            if (Vector3.Distance(enemy.transform.position, enemy.wayPoints[enemy.wayPoints.Count - 1]) <= 0.5f)//Judge the distance between the enemy and the last navigation point on the patrol route. If the distance is very small, the current route has been completed, reset the navigation point subscript, and let the enemy repeat the patrol route
            {
                enemy.index = 0;
            }

        }
        //Debug.Log(distance);

        if (enemy.attackList.Count > 0)//Enemies appear within the enemy's patrol scanning range, and enter the attack state at this time
        {
            enemy.TranstionToState(enemy.attackState);
        }
    }
}
