using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIClickBlock<T> : MonoBehaviour where T : UIClickBlock<T>
{
    public static T Instance { get; protected set; }
    protected static readonly HashSet<object> blockReason = new();
    public static HashSet<object> BlockReason => blockReason;
    public static bool IsBlock
    {
        get { return blockReason.Count > 0; }
    }
    public void CheckBlock()
    {
        return;
        gameObject.SetActive(blockReason.Count > 0);
    }
    public static void AddBlock(object reason)
    {
        blockReason.Add(reason);
        if (Instance != null)
        {
            Debug.Log("add block:::" + blockReason.Count);
            Instance.CheckBlock();
        }
    }
    public static void RemoveBlock(object reason)
    {
        blockReason.Remove(reason);
        if (Instance != null)
        {
            Debug.Log("remove block:::" + blockReason.Count);
            Instance.CheckBlock();
        }
    }

    private void Awake()
    {
        Instance = this as T;
        CheckBlock();
        gameObject.SetActive(false);

        EventModule.AddListener<bool>(E_GameEvent.ShowMask, OnShowMask);
    }

    private void OnShowMask(bool showMask)
    {
        gameObject.SetActive(showMask);
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
        EventModule.RemoveListener<bool>(E_GameEvent.ShowMask, OnShowMask);

    }
}
