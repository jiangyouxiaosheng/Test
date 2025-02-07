using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : UnitBase
{
 
    private void Start()
    {
        Init("Enemy", 1, 1, 200, 100, Vector3.zero);
    }

    public override void Die()
    {
 
    }

    public override void Attack(UnitBase target)
    {
    }
}


