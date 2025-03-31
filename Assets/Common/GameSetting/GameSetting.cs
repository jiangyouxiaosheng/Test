using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class GameSetting : MonoBehaviour
{
    public int targetWoods;
    public bool isWin;
    private int _currentWoods;
    public WorldCanvas worldCanvas;
 


    private void Awake()
    {
        _currentWoods = 0;
        isWin = false;
        var cannvas = Instantiate(worldCanvas);
        cannvas.Init();
        UIModule.Instance.OpenPage(EUIPageType.BattlePage);
    }

    private void OnEnable()
    {
       
    }

    public void AddWood(int wood)
    {
        _currentWoods += wood;
        EventModule.BroadCast(E_GameEvent.RefreshUI);
    }

    public void ReduceWood(int wood)
    {
        _currentWoods -= wood;
        EventModule.BroadCast(E_GameEvent.RefreshUI);
    }

    public int GetWood()
    {
        return _currentWoods;
    }
}
