using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using Bizza.Channel;
using DG.Tweening;
using OPS.Obfuscator.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWinPage : UIPageBase<bool>
{
    public override EUIPageType PageType  => EUIPageType.GameWinPage;
    public TMP_Text playerHaveCoinTxt;
    public Button redeemBtn;
    public Image closeBox;
    public Image openBox;
    public Transform startPos;
    public Button watchAdBtn;
    public Button doubleWatchBtn;
    public Transform doublestartPos;
    public Button exitBtn;
    public ParticleImage particleImage;
    public ParticleImage gameWinParticleImage;
    public CoinAnim coinAnim;
    public Animation anim;
    public TMP_Text rewardCoinTxt;
    
    private int _haveCoin;
    private bool _isDouble;
    
    protected override void OnOpen(bool isDouble)
    {
        redeemBtn.onClick.RemoveAllListeners();
        redeemBtn.onClick.AddListener(()=>{ EventModule.BroadCast(E_GameEvent.ShowMask, true); AccountController.Instance.GetWithdrawalInfo();});
        watchAdBtn.onClick.RemoveAllListeners();
        watchAdBtn.onClick.AddListener(()=>{AdsUtils.OpenRewardAds("BIGWIN_CLAIM", closeCallback: OnAdClaimSuccess);});
        doubleWatchBtn.onClick.RemoveAllListeners();
        doubleWatchBtn.onClick.AddListener(()=>{AdsUtils.OpenRewardAds("BIGWIN_CLAIM", closeCallback: OnAdClaimSuccess);});
        doubleWatchBtn.gameObject.SetActive(false);
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(()=>{
            if (_isDouble)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                UIModule.Instance.OpenPage(EUIPageType.NoThanksPage);
            }
            UIModule.Instance.ClosePage(this);
             });
        exitBtn.gameObject.SetActive(false);
        playerHaveCoinTxt.text = AccountController.Instance.userInfo.udCoin.ToString();
        _haveCoin = AccountController.Instance.userInfo.udCoin;
        EventModule.AddListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);
        EventModule.AddListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse, OnGetWithdrawalInfoResponse);
        _isDouble = false;
        openBox.gameObject.SetActive(false);
        closeBox.gameObject.SetActive(true);
        if (isDouble)
        {
            FirstCoinAnimEnd();
            
        }
        gameWinParticleImage.gameObject.SetActive(true);
        StartCoroutine(FirstAnimEnd());
    }
    IEnumerator FirstAnimEnd()
    {
        yield return new WaitForSeconds(4.5f);
        exitBtn.gameObject.SetActive(true);
    }
    
    IEnumerator DoubleAnimEnd()
    {
        yield return new WaitForSeconds(6.5f);
        exitBtn.gameObject.SetActive(true);
    }
    
    
    [DoNotRename]
    private void OnAdClaimSuccess(bool success)
    {
        if (success)
        {
            var rewardInfo =AttributionUtil.RewardAdInfo;
            EventModule.BroadCast(E_GameEvent.ShowMask, true);
            if (_isDouble)
            {
                AccountController.Instance.GetAdsReward(true, rewardInfo.ECPM, rewardInfo.AT_ID, rewardInfo.A_UID, 0, 100,
                    100, rewardInfo.AdPlatform, rewardInfo.AdFormat, rewardInfo.AdNetwork, "", "",OverseaPlatform.IsTopon);
            }
            else
            {
                AccountController.Instance.GetAdsReward(false, rewardInfo.ECPM, rewardInfo.AT_ID, rewardInfo.A_UID, 0, 100,
                    100, rewardInfo.AdPlatform, rewardInfo.AdFormat, rewardInfo.AdNetwork, "", "",OverseaPlatform.IsTopon);
            }
            
            
        }
        else
        {
            UIDialog dialog = UIController.ShowPage<UIDialog>();
            Debug.Log("获取广告奖励失败");
            dialog.Show("Error", "Ad is loading, please try again later.", null);
        }
#if UNITY_EDITOR
        OnServerRespones(new AccountController.AdsRewardData() { doubleSwitch = "t", rewardCoin = 58 });
#endif
    }

    private void OnServerRespones(AccountController.AdsRewardData rewardData)
    {
        particleImage.gameObject.transform.position =
            _isDouble ? doublestartPos.transform.position : startPos.transform.position;
        openBox.gameObject.SetActive(true);
        closeBox.gameObject.SetActive(false);
        particleImage.gameObject.SetActive(true);
        UIModule.Instance.OpenPage(EUIPageType.WaitMaskPage);
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
       
#if UNITY_IOS && !UNITY_EDITOR
        rewardCoinTxt.text = rewardData.rewardCoin.ToString();
#else
        Debug.LogError("打包时替换");
#endif
        
    }

    
    public void FirstCoinAnimEnd()
    {
        if (!_isDouble && UIModule.Instance.FindOpenedPage(EUIPageType.GameWinPage))
        {
            exitBtn.gameObject.SetActive(false);
            anim.Play("well done2");
            _isDouble = true;
            StartCoroutine(DoubleAnimEnd());
        }
        UIModule.Instance.ClosePage(EUIPageType.WaitMaskPage);
    }

   

    public void IsDoubleAimEnd()
    {
        if (_isDouble)
        {
            UIModule.Instance.ClosePage(EUIPageType.WaitMaskPage);
            UIModule.Instance.ClosePage(this);
        }
     
    }
    private void OnGetWithdrawalInfoResponse(AccountController.WithdrawalInfo withdrawalInfo)
    {
        UIWithdrawal withdrawal = UIController.ShowPage<UIWithdrawal>();
        withdrawal.Init(withdrawalInfo);
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
    }
    protected override void OnShow()
    {
        
    }

    protected override void OnHide()
    {
     
    }
    public void CoinAnimStart()
    {
        DOTween.To(() =>  _haveCoin,
                x =>  _haveCoin = x,
                AccountController.Instance.userInfo.udCoin , 1.2f)
            .SetEase(Ease.Linear).OnUpdate(() =>
            {
               playerHaveCoinTxt.text =  _haveCoin.ToString();
            });
    }
    protected override void OnClose()
    {
        _isDouble = false;
        EventModule.RemoveListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);
        EventModule.RemoveListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse, OnGetWithdrawalInfoResponse);
    }
    public void GetCoinAnim()
    {
        coinAnim.GetAnimCoin();
    }

}
