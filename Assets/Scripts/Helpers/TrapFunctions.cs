//code by vincent kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrapFunctions
{
    public static void DoSomething(Collider2D otherCollider, TrapCollisionEvent thisGameObject)
    {
        Debug.Log("This Works");
    }

    public static void BearTrap(Collider2D otherCollider, TrapCollisionEvent thisGameObject)
    {
        if(otherCollider.gameObject.tag == "Enemy")
        {
            otherCollider.GetComponent<EnemyBaseBehavior>().Kill();
            GameObject.Destroy(thisGameObject.gameObject);
        }
    }

    public static void RopeTrap(Collider2D otherCollider, TrapCollisionEvent thisGameObject)
    {
        if (otherCollider.gameObject.tag == "Enemy")
        {
            otherCollider.gameObject.GetComponent<EnemyBaseBehavior>().Stun();
            GameObject.Destroy(thisGameObject.gameObject);
        }
    }

    public static void SpikeTrap(Collider2D otherCollider, TrapCollisionEvent thisGameObject)
    {
        if (otherCollider.gameObject.tag == "Enemy")
        {
            otherCollider.gameObject.GetComponent<EnemyBaseBehavior>().speed /= 2;
            GameObject.Destroy(thisGameObject.gameObject);
        }
    }
}
