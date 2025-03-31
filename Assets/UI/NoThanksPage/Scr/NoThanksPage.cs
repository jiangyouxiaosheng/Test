using System.Collections;
using AssetKits.ParticleImage;
using Bizza.Channel;
using DG.Tweening;
using OPS.Obfuscator.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoThanksPage : UIPageBase
{
    public override EUIPageType PageType => EUIPageType.NoThanksPage;

    public Button getRewardBtn;
    public Button exitBtn;
    public ParticleImage particleImage;
    public Button redeemBtn;
    public TMP_Text haveCoinTxt;
    public Transform startPos;
    public CoinAnim coinAnim;
    private int _haveCoin;
    protected override void OnOpen()
    {
        getRewardBtn.onClick.RemoveAllListeners();
        getRewardBtn.onClick.AddListener(() =>
        {
            AdsUtils.OpenRewardAds("BIGWIN_CLAIM", closeCallback: OnAdClaimSuccess);
        });
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() =>
        {
            UIModule.Instance.ClosePage(this);
        });
        redeemBtn.onClick.RemoveAllListeners();
        redeemBtn.onClick.AddListener(()=>{ EventModule.BroadCast(E_GameEvent.ShowMask, true); AccountController.Instance.GetWithdrawalInfo();});
        exitBtn.gameObject.SetActive(false);
        StartCoroutine(ExitBtnActive());
        haveCoinTxt.text = AccountController.Instance.userInfo.udCoin.ToString();
        _haveCoin = AccountController.Instance.userInfo.udCoin;
        EventModule.AddListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);
        EventModule.AddListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse, OnGetWithdrawalInfoResponse);
    }

    IEnumerator ExitBtnActive()
    {
        yield return new WaitForSeconds(3f);
        exitBtn.gameObject.SetActive(true);
    }
    [DoNotRename]
    private void OnAdClaimSuccess(bool success)
    {
        if (success)
        {
            var rewardInfo = Bizza.Channel.AttributionUtil.RewardAdInfo;
            EventModule.BroadCast(E_GameEvent.ShowMask, true);
            AccountController.Instance.GetAdsReward(false, rewardInfo.ECPM, rewardInfo.AT_ID, rewardInfo.A_UID, 0, 100,
                100, rewardInfo.AdPlatform, rewardInfo.AdFormat, rewardInfo.AdNetwork, "", "",OverseaPlatform.IsTopon);
        }
        else
        {
            UIDialog dialog = UIController.ShowPage<UIDialog>();
            dialog.Show("Error", "Ad is loading, please try again later.", null);
        }
#if UNITY_EDITOR
        OnServerRespones(new AccountController.AdsRewardData() { doubleSwitch = "t", rewardCoin = 58 });
#endif
    }
    
    
    private void OnGetWithdrawalInfoResponse(AccountController.WithdrawalInfo withdrawalInfo)
    {
        UIWithdrawal withdrawal = UIController.ShowPage<UIWithdrawal>();
        withdrawal.Init(withdrawalInfo);
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
    }
    
    
    private void OnServerRespones(AccountController.AdsRewardData rewardData)
    {
        particleImage.gameObject.transform.position = startPos.position;
        particleImage.gameObject.SetActive(true);
        UIModule.Instance.OpenPage(EUIPageType.WaitMaskPage);
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
    }

    public void ParticleImageAnimEnd()
    {
        StartCoroutine(NextLv());
    }

    IEnumerator NextLv()
    {
        yield return new WaitForSeconds(0.5f);
        UIModule.Instance.ClosePage(this);
        UIModule.Instance.ClosePage(EUIPageType.WaitMaskPage);
        UIModule.Instance.OpenPage(EUIPageType.GameWinPage, true);
        
        
    }
    public void CoinAnimStart()
    {
        DOTween.To(() =>  _haveCoin,
                x =>  _haveCoin = x,
                AccountController.Instance.userInfo.udCoin , 1.2f)
            .SetEase(Ease.Linear).OnUpdate(() =>
            {
                haveCoinTxt.text =  _haveCoin.ToString();
            });
    }
    public void GetCoinAnim()
    {
        coinAnim.GetAnimCoin();
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
