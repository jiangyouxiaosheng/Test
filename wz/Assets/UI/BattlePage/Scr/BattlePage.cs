using System;
using System.Collections;
using System.Collections.Generic;
using OPS.Obfuscator.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[DoNotRename]
public class BattlePage : UIPageBase
{
    public override EUIPageType PageType => EUIPageType.BattlePage;
    public TMP_Text gameLvTxt;
    public TMP_Text woodTxt;
    public Image woodPress;
    
    protected override void OnOpen()
    {
        EventModule.BroadCast(E_GameEvent.CanInput,false);
        woodPress.fillAmount = 0;
        gameLvTxt.text = $"Level {SenceModule.Instance.NowLv}";
        StartCoroutine(CanInput());
    }

    IEnumerator CanInput()
    {
        yield return new WaitForSeconds(2.5f);
        EventModule.BroadCast(E_GameEvent.CanInput, UIModule.Instance.FindOpenedPage(EUIPageType.HowPlayPage) == null);
    }
    protected override void OnShow()
    {
      
    }

    public void CantInputAnimEvent()
    {
        EventModule.BroadCast(E_GameEvent.CanInput,false);
    }
    private void Update()
    {
        //woodTxt.text = $"{GameManager.instance.gameSetting.GetWood()}/{GameManager.instance.gameSetting.targetWoods}";
        //woodPress.fillAmount =GameManager.instance.gameSetting.GetWood() /(float)GameManager.instance.gameSetting.targetWoods;
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
    {
        
    }
}
