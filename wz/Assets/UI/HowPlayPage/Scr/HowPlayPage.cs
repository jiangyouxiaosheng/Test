using System.Collections;
using System.Collections.Generic;
using OPS.Obfuscator.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[DoNotRename]
public class HowPlayPage : UIPageBase
{
    public override EUIPageType PageType => EUIPageType.HowPlayPage;


    public Button nextBtn;
    public TMP_Text nextBtnText;
    public Button closeBtn;
    public GameObject normalObj;
    public GameObject wzObj;
    protected override void OnOpen()
    {
        
        
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(() =>
        {
            if (GameModule.Instance.isReviewMode)
            {
               StartCoroutine(CanInput());
            }
            else
            {
                wzObj.SetActive(true);
                normalObj.SetActive(false);
            }
        });
        
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(()=>{StartCoroutine(CanInput());});
        nextBtnText.text = GameModule.Instance.isReviewMode ? "Tap To Continue" : "Next";
        wzObj.SetActive(false);
        normalObj.SetActive(true);
        EventModule.BroadCast(E_GameEvent.CanInput,false);
    }

    IEnumerator CanInput()
    {
        yield return new WaitForSeconds(0.1f);
        EventModule.BroadCast(E_GameEvent.CanInput,true);
        UIModule.Instance.ClosePage(this);
    }
    protected override void OnShow()
    {
     
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
    {
       
    }
}
