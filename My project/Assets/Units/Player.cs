using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : UnitBase
{

    
    private void Start()
    {
        Init("Player", 1, 1, 200, 100, Vector3.zero);
      
    }

    public override void Die()
    {
      
    }

    public override void Attack(UnitBase target)
    {
        
    }

    private void Update()
    {
        
    }
}
