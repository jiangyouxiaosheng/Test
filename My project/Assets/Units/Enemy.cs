using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : UnitBase
{
    public Enemy(string name, int level, int health, int mana, Vector3 position) : base(name, level, health, mana, position)
    {
        name = "敌人";
        level = 1;
        health = 100;
        mana = 100;
        position = position;
    }

    public override void Die()
    {
 
    }

    public override void Attack(UnitBase target)
    {
    }
}


