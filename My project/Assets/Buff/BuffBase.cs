using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase : ScriptableObject
{
    public string buffName; //名字
    public Sprite icon;     //图片
    public float duration;  //时间
    public bool isPermanent; //是否为永久性的
    public int maxStacks = 1; //
    public GameObject visualEffect;

    public abstract void Apply(UnitBase by,UnitBase target);
    public abstract void Tick(UnitBase by,UnitBase target);
    public abstract void Remove(UnitBase by,UnitBase target);
    
    // 用于ScriptableObject克隆
    public virtual BuffBase Clone()
    {
        return Instantiate(this);
    }
}
