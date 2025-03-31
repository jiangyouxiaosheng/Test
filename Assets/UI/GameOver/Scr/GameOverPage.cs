using System.Collections;
using AssetKits.ParticleImage;
using Bizza.Channel;
using DG.Tweening;
using LitMotion;
using OPS.Obfuscator.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Ease = DG.Tweening.Ease;

public class GameOverPage : UIPageBase
{

    public override EUIPageType PageType => EUIPageType.GameOverPage;
    public TMP_Text playerHaveCoinTxt;
    public TMP_Text skipRewardTxt;
    public TMP_Text rewardTxt;
    public GameObject getRwardObj;
    public Button redeemBtn;
    public Button skipBtn;
    public Button tryAgainBtn;
    public ParticleImage particleImage;
    public Slider woodProgress;
    public TMP_Text currentWoodTxt;
    
    private int _currentWood;
    private int _playerhaveCoin;
    public Animator coinAddAnim;
    protected override void OnOpen()
    {
        
        redeemBtn.onClick.RemoveAllListeners();
        redeemBtn.onClick.AddListener(()=>{ EventModule.BroadCast(E_GameEvent.ShowMask, true); AccountController.Instance.GetWithdrawalInfo();});
        skipBtn.onClick.RemoveAllListeners();
        skipBtn.onClick.AddListener(()=>{AdsUtils.OpenRewardAds("BIGWIN_CLAIM", closeCallback: OnAdClaimSuccess);});
        tryAgainBtn.onClick.RemoveAllListeners();
        tryAgainBtn.onClick.AddListener(()=>{SenceModule.Instance.ReStartGame(); UIModule.Instance.ClosePage(this);});
        tryAgainBtn.gameObject.SetActive(false);
        playerHaveCoinTxt.text =  AccountController.Instance.userInfo.udCoin.ToString();
        _playerhaveCoin = AccountController.Instance.userInfo.udCoin;
        woodProgress.value = 0;
        EventModule.AddListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);
        EventModule.AddListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse, OnGetWithdrawalInfoResponse);
        EventModule.AddListener(E_GameEvent.UI_GetCoinAnim,PlayerAnim);
        currentWoodTxt.text = "0";
        tryAgainBtn.gameObject.SetActive(false);
        
        //记得删除↓
        WoodProgressAnimStart();
        CanShowTryAgainBtn();
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

    protected override void OnShow()
    {
       
    }
    private void OnServerRespones(AccountController.AdsRewardData rewardData)
    {
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
        skipRewardTxt.gameObject.gameObject.SetActive(false);
        getRwardObj.gameObject.SetActive(true);
        particleImage.gameObject.transform.position = skipBtn.transform.position;
        particleImage.gameObject.SetActive(true);
        rewardTxt.text = $":+{rewardData.rewardCoin.ToString()}";
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
        UIModule.Instance.OpenPage(EUIPageType.WaitMaskPage);
        Debug.LogError(rewardData.rewardCoin.ToString());
    }
    
    protected override void OnHide()
    {
        
    }

    public void WoodProgressAnimStart()
    {

    }

    public void CanShowTryAgainBtn()
    {
        StartCoroutine(ShowTryAginBnt());
    }

    private IEnumerator ShowTryAginBnt()
    {
        yield return new WaitForSeconds(3f);
        tryAgainBtn.gameObject.SetActive(true);
    }

    public void CoinAnimStart()
    {
        DOTween.To(() => _playerhaveCoin,
                x => _playerhaveCoin = x,
                AccountController.Instance.userInfo.udCoin , 1.2f)
            .SetEase(Ease.Linear).OnUpdate(() =>
            {
                playerHaveCoinTxt.text = _playerhaveCoin.ToString();
            });
    }
    private void PlayerAnim()
    {
        // 检查动画是否正在播放
        if (coinAddAnim.IsInTransition(0) || coinAddAnim.GetCurrentAnimatorStateInfo(0).IsName("GetCoinAnim"))
        {
            // 如果动画正在播放，从头开始播放
            coinAddAnim.Play("GetCoinAnim", 0, 0f);
        }
        else
        {
            // 如果动画未播放，直接播放
            coinAddAnim.SetTrigger("Play");
        }
    }
    
    
    public void AnimFinsh()
    {
        StartCoroutine(AnimF());
        
    }

    IEnumerator AnimF()
    {
        yield return new WaitForSeconds(1f);
        UIModule.Instance.ClosePage(EUIPageType.WaitMaskPage);
        UIModule.Instance.ClosePage(this);
    }
    private void OnGetWithdrawalInfoResponse(AccountController.WithdrawalInfo withdrawalInfo)
    {
        UIWithdrawal withdrawal = UIController.ShowPage<UIWithdrawal>();
        withdrawal.Init(withdrawalInfo);
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
    }
    public void CoinAnimPlay()
    {
        EventModule.BroadCast(E_GameEvent.UI_GetCoinAnim);
    }
    protected override void OnClose()
    {
        EventModule.RemoveListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);
        EventModule.RemoveListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse, OnGetWithdrawalInfoResponse);
        EventModule.RemoveListener(E_GameEvent.UI_GetCoinAnim,PlayerAnim);
    }
}
