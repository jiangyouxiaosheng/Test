using System;
using System.Collections;
using System.Collections.Generic;
using OPS.Obfuscator.Attribute;
using UnityEngine;
[DoNotRename]
public class GameModule : BaseGameModule<GameModule>
{

    //是否为提审模式
    public bool isReviewMode;
    //失败次数
    public int failCount;
    //连击次数
    public int comboCount;
    //连击重置时间
    public float comboResetTime;
    private float _currentTime;
    
    //初始化
    public override void InitGameModule()
    {
        isReviewMode = AccountController.Instance.stInfo.earningSwitch == 0;
        if (!isReviewMode)
        {
            //是否一开始就显示网赚，如果不需要，将这个代码移动你需要显示的时机。
            UIController.ShowPage<UIBizzaAAA>();
        }
        failCount = 0;
    }

    public override void ReleaseGameModule()
    {
        
    }

    
    

    private void Update()
    {
       
        _currentTime -= Time.deltaTime;
        if (_currentTime <= 0)
        {
            comboCount = 0;
        }
    }

    public void ComboCountAdd()
    {
        _currentTime = comboResetTime;
        comboCount++;
    }

    public void FailPage()
    {
        if (failCount >= 2)
        {
            failCount =0;
            
            Debug.Log("-----------------------------3----------------------------");
            UIModule.Instance.OpenPage(EUIPageType.HowPlayPage);
        }
    }
}
