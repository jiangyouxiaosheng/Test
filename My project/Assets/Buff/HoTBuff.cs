using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/HoTBuff")]
public class HoTBuff : BuffBase
{
    public float healPerSecond = 5f;

    public override void Apply(UnitBase by,UnitBase target) { }

    public override void Tick(UnitBase by,UnitBase target)
    {
        target.Heal(by,10);
    }

    public override void Remove(UnitBase by,UnitBase target) { }
}
