using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : UnitBase
{
    // Start is called before the first frame update
    public Player(string name, int level, int health, int mana, Vector3 position) : base(name, level, health, mana, position)
    {
        name = "Player";
        level = 1;
        health = 100;
        mana = 100;
        position =  Vector3.zero;
    }


    public override void Die()
    {
      
    }

    public override void Attack(UnitBase target)
    {
        
    }
}
