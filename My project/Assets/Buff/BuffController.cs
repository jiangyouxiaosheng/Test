using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BuffController : MonoBehaviour
{
    private Dictionary<BuffBase, BuffInstance> activeBuffs = new Dictionary<BuffBase, BuffInstance>();

    public void AddBuff(UnitBase by,BuffBase buff)
    {
        if (activeBuffs.TryGetValue(buff, out BuffInstance existing))
        {
            existing.Refresh();
        }
        else
        {
            BuffInstance instance = new BuffInstance(buff.Clone());
            activeBuffs.Add(buff, instance);
            instance.Apply(by,GetComponent<UnitBase>());
            StartCoroutine(BuffDurationTimer(by,instance));
        }
    }

    private IEnumerator BuffDurationTimer(UnitBase by,BuffInstance instance)
    {
        float timer = 0;
        while (timer < instance.buff.duration && !instance.buff.isPermanent)
        {
            timer += Time.deltaTime;
            instance.Tick(by,GetComponent<UnitBase>());
            yield return null;
        }
        RemoveBuff(by,instance.buff);
    }

    public void RemoveBuff(UnitBase by,BuffBase buff)
    {
        if (activeBuffs.TryGetValue(buff, out BuffInstance instance))
        {
            instance.Remove(by,GetComponent<UnitBase>());
            activeBuffs.Remove(buff);
        }
    }

    private class BuffInstance
    {
        public BuffBase buff;
        public int currentStacks;
        public float remainingTime;

        public BuffInstance(BuffBase buff)
        {
            this.buff = buff;
            remainingTime = buff.duration;
            currentStacks = 1;
        }

        public void Apply(UnitBase by,UnitBase target)
        {
            buff.Apply(by,target);
            if (buff.visualEffect != null)
            {
                Instantiate(buff.visualEffect, target.transform);
            }
        }

        public void Tick(UnitBase by,UnitBase target)
        {
            buff.Tick(by,target);
        }

        public void Remove(UnitBase by,UnitBase target)
        {
            buff.Remove(by,target);
        }

        public void Refresh()
        {
            if (currentStacks < buff.maxStacks)
            {
                currentStacks++;
            }
            remainingTime = buff.duration;
        }
    }
}
