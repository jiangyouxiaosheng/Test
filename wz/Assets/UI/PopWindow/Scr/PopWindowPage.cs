using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using Bizza.Channel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopWindowPage :  UIPageBase
{
    public override EUIPageType PageType => EUIPageType.PopWindowPage;
    public Button adBtn;
    public Button exitBtn;
    public Button redeemBtn;
    public ParticleImage adCoin;
    public ParticleImage gameCoin;
    public TMP_Text playerHaveAdCoinTxt;
    public TMP_Text playerHaveGameCoinTxt;
    public TMP_Text canGetAdCoinTxt;
    public TMP_Text canGetGameCoinTxt;
    protected override void OnOpen()
    {
        redeemBtn.onClick.RemoveAllListeners();
        redeemBtn.onClick.AddListener(()=>{ EventModule.BroadCast(E_GameEvent.ShowMask, true); AccountController.Instance.GetWithdrawalInfo();});
        adBtn.onClick.RemoveAllListeners();
        adBtn.onClick.AddListener(()=>{AdsUtils.OpenRewardAds("BIGWIN_CLAIM", closeCallback: OnAdClaimSuccess);});
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(()=> { UIModule.Instance.ClosePage(this); });
        playerHaveAdCoinTxt.text = AccountController.Instance.userInfo.udCoin.ToString();
        EventModule.AddListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);
        EventModule.AddListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse, OnGetWithdrawalInfoResponse);
    }

    private void OnAdClaimSuccess(bool success)
    {
        if (success)
        {
            var rewardInfo =AttributionUtil.RewardAdInfo;
            EventModule.BroadCast(E_GameEvent.ShowMask, true);
            AccountController.Instance.GetAdsReward(false, rewardInfo.ECPM, rewardInfo.AT_ID, rewardInfo.A_UID, 0, 100,
                100, rewardInfo.AdPlatform, rewardInfo.AdFormat, rewardInfo.AdNetwork, "", "",OverseaPlatform.IsTopon);
        }
#if UNITY_EDITOR
        OnServerRespones(new AccountController.AdsRewardData() { doubleSwitch = "t", rewardCoin = 58 });
#endif
    }

    private void OnServerRespones(AccountController.AdsRewardData rewardData)
    {
        adCoin.gameObject.SetActive(true);
        gameCoin.gameObject.SetActive(true);
        canGetAdCoinTxt.text = "+"+rewardData.rewardCoin.ToString();
        canGetGameCoinTxt.text = "+"+ 5000;
        EventModule.BroadCast(E_GameEvent.UI_RefreshCoin);
        playerHaveAdCoinTxt.text = AccountController.Instance.userInfo.udCoin.ToString();
        UIModule.Instance.OpenPage(EUIPageType.WaitMaskPage);
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
        StartCoroutine(CloseThisPage());
    }

    private void OnGetWithdrawalInfoResponse(AccountController.WithdrawalInfo withdrawalInfo)
    {
        UIWithdrawal withdrawal = UIController.ShowPage<UIWithdrawal>();
        withdrawal.Init(withdrawalInfo);
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
    }
    IEnumerator CloseThisPage()
    {
        yield return new WaitForSeconds(3f);
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
        EventModule.RemoveListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);
        EventModule.RemoveListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse, OnGetWithdrawalInfoResponse);
        UIModule.Instance.ClosePage(EUIPageType.WaitMaskPage);
    }
}
