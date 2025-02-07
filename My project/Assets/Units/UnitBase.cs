using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class UnitBase : MonoBehaviour
{
    // 基本属性
    public string Name { get; protected set; } // 角色名称
    public int Level { get; protected set; }   // 角色等级
    public int Health { get; protected set; } // 生命值
    public int MaxHealth { get; protected set; } // 最大生命值
    public int Mana { get; protected set; }   // 魔法值
    public int MaxMana { get; protected set; } // 最大魔法值
    public Vector3 Position { get; protected set; } // 角色位置（三维坐标）

   
    
    // 构造函数
    public void Init(string name, int level, int health, int maxHealth,int mana, Vector3 position)
    {
        Name = name;
        Level = level;
        Health = health;
        MaxHealth = maxHealth;
        Mana = mana;
        MaxMana = mana;
        Position = position;
    }

  

    // 生命值恢复方法
    public void Heal(UnitBase by,int amount)
    {
        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        Debug.Log($"{Name} healed for {amount}. Current Health: {Health} ,来自{by.Name}的治疗");
    }

    // 魔法值恢复方法
    public void RestoreMana(int amount)
    {
        Mana += amount;
        if (Mana > MaxMana)
        {
            Mana = MaxMana;
        }
        Debug.Log($"{Name} restored {amount} mana. Current Mana: {Mana}");
    }

    // 受到伤害方法
    public void TakeDamage(UnitBase by,int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
        else
        {
            Debug.Log($"{Name} took {amount} damage. Current Health: {Health},伤害来自于{by.Name}");
        }
    }

    // 死亡方法（抽象方法，子类需要实现）
    public abstract void Die();

    // 移动方法
    public void Move(Vector3 newPosition)
    {
        Position = newPosition;
        Debug.Log($"{Name} moved to position {Position}");
    }

    // 攻击方法（抽象方法，子类需要实现）
    public abstract void Attack(UnitBase target);

    // 打印角色状态
    public void PrintStatus()
    {
        Debug.Log($"Name: {Name}, Level: {Level}, Health: {Health}/{MaxHealth}, Mana: {Mana}/{MaxMana}, Position: {Position}");
    }
}
