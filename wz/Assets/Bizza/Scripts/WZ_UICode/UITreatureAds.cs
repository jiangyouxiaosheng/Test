using Bizza.Channel;
using LitMotion;
using OPS.Obfuscator.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AdRequestType
{
    None,
    Claim,
    ClaimX2,
    NoThanks
}

public class UITreatureAds : UIPage
{

    [SerializeField] private Image box_1, box_2;
    [SerializeField] private Button btnClaim, btnDouble, btnNoThank;
    [SerializeField] private Transform floatCoinFrom, floatCoinTarget;
    [SerializeField] private TMP_Text txtCoin;
    [SerializeField] private GameObject topGO;

    [SerializeField] private TMP_Text AddCoin;

    private bool isAdsToGetReward;
    private bool isRequestToGetReward;
    private bool isWatchClaimAd;

    private AdRequestType adRequestType = AdRequestType.None;

    private int m_AddCoin = 0;

    public void Awake()
    {
        btnClaim.onClick.RemoveAllListeners();
        btnClaim.onClick.AddListener(OnBtnClaimClick);

        btnDouble.onClick.RemoveAllListeners();
        btnDouble.onClick.AddListener(OnBtnDoubleClick);

        btnNoThank.onClick.RemoveAllListeners();
        btnNoThank.onClick.AddListener(OnBtnNoThankClick);

    }

    public void OnEnable()
    {
        isWatchClaimAd = false;
        isAdsToGetReward = false;
        isRequestToGetReward = false;
        adRequestType = AdRequestType.None;

        box_1.color = new Color(1, 1, 1, 1);
        box_2.color = new Color(1, 1, 1, 0);

        btnClaim.gameObject.SetActive(true);
        btnDouble.gameObject.SetActive(false);
        topGO.SetActive(false);
        txtCoin.text = AccountController.Instance.userInfo.udCoin.ToString();

        //Banner
        Bizza.Channel.Platform.Instance.ShowBanner();

        EventModule.AddListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);

        m_AddCoin = 0;
        AddCoin.text = m_AddCoin.ToString();

        if (AccountController.Instance == null ||
            AccountController.Instance.stInfo == null ||
            AccountController.Instance.stInfo.earningSwitch == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnDisable()
    {
        EventModule.BroadCast(E_GameEvent.ShowMask, false);

        EventModule.RemoveListener<AccountController.AdsRewardData>(E_GameEvent.GetAdsRewardFromServerResponse, OnServerRespones);
    }

    private void OnBtnClaimClick()
    {
        if (isAdsToGetReward || isRequestToGetReward) return;

        isAdsToGetReward = true;
        isWatchClaimAd = true;
        AdsUtils.OpenRewardAds("BIGWIN_CLAIM", closeCallback: OnAdClaimSuccess);

        adRequestType = AdRequestType.Claim;
    }

    [DoNotRename]
    private void OnAdClaimSuccess(bool success)
    {
        isAdsToGetReward = false;
        if (success)
        {
            isRequestToGetReward = true;
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

        //DEBUG
#if UNITY_EDITOR
        OnServerRespones(new AccountController.AdsRewardData() { doubleSwitch = "t", rewardCoin = 58 });
#endif
    }



    private void OnBtnDoubleClick()
    {
        if (isAdsToGetReward || isRequestToGetReward) return;

        isAdsToGetReward = true;
        isWatchClaimAd = true;
        AdsUtils.OpenRewardAds("BIGWIN_DOUBLE", closeCallback: OnDoubleSuccess_AdsCBK);
        adRequestType = AdRequestType.ClaimX2;

    }

    private void OnDoubleSuccess_AdsCBK(bool success)
    {
        isAdsToGetReward = false;
        if (success)
        {
            isRequestToGetReward = true;
            var rewardInfo = Bizza.Channel.AttributionUtil.RewardAdInfo;

            EventModule.BroadCast(E_GameEvent.ShowMask, true);
            //请求服务器
            AccountController.Instance.GetAdsReward(true, rewardInfo.ECPM, rewardInfo.AT_ID, rewardInfo.A_UID, 0, 100,
                100, rewardInfo.AdPlatform, rewardInfo.AdFormat, rewardInfo.AdNetwork, "", "",OverseaPlatform.IsTopon);
        }

        //DEBUG 
#if UNITY_EDITOR
        OnServerRespones(new AccountController.AdsRewardData()
        {
            rewardCoin = 158
        });
#endif
    }


    private void OnBtnNoThankClick()
    {
        if (isWatchClaimAd)
        {
            UIController.HidePage(this);
        }
        else
        {
            adRequestType = AdRequestType.NoThanks;
            AdsUtils.OpenRewardAds("BIGWIN_CLAIM", closeCallback: (x) =>
            {
                OnAdClaimSuccess(x);

                LMotion.Create(0, 0, 2.75f).WithOnComplete(() =>
                {
                    UIController.HidePage(this);
                }).RunWithoutBinding();

            });
        }

    }

    //private int coinsHash = CurrencyType.Coins.ToString().GetHashCode();

    private void PlayFloatCoin(int currentReward, System.Action onFinish)
    {
        //int rewardMult = 1;
        //if (currentReward > 20)
        //    currentReward = 20;
        //topGO.SetActive(true);

        //频闭按钮交互
        btnClaim.interactable = false;
        btnDouble.interactable = false;
        btnNoThank.interactable = false;

        UpdateUIRewardCoin(currentReward);

        LMotion.Create(0, 1, 2.5f).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
        {
            btnClaim.interactable = true;
            btnDouble.interactable = true;
            btnNoThank.interactable = true;

            onFinish?.Invoke();
        }).RunWithoutBinding();

        //FloatingCloud.SpawnCurrency(coinsHash, (RectTransform)floatCoinFrom, (RectTransform)floatCoinTarget, 10,
        //    "", () =>
        //    {
        //        CurrenciesController.Add(CurrencyType.Coins, currentReward * rewardMult);
        //        onFinish?.Invoke();
        //        topGO.SetActive(false);
        //    });
    }


    #region 服务器回调

    private void OnServerRespones(AccountController.AdsRewardData rewardData)
    {
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
        switch (adRequestType)
        {
            case AdRequestType.Claim:
                OnClaimResponse(rewardData);
                break;
            case AdRequestType.ClaimX2:
                OnDoubleResponse(rewardData);
                break;
            case AdRequestType.NoThanks:
                OnClaimResponse(rewardData);
                break;

        }
    }

    private void OnClaimResponse(AccountController.AdsRewardData rewardData)
    {
        Debug.Log("OnClaimResponse rewardData.double:" + rewardData.doubleSwitch);
        txtCoin.text = AccountController.Instance.userInfo.udCoin.ToString();
        isRequestToGetReward = false;
        PlayFloatCoin(rewardData.rewardCoin, () =>
        {
            //播放完金币动画后显示[双倍奖励]
            if (adRequestType != AdRequestType.NoThanks)
            {
                btnClaim.gameObject.SetActive(false);
                if (rewardData.doubleSwitch == "t")
                {
                    box_1.color = new Color(1, 1, 1, 0);
                    box_2.color = new Color(1, 1, 1, 1);
                    btnDouble.gameObject.SetActive(true);
                }
            }
        });
    }

    private void OnDoubleResponse(AccountController.AdsRewardData rewardData)
    {
        Debug.Log("OnDoubleResponse rewardData.double:" + rewardData.doubleSwitch);

        txtCoin.text = AccountController.Instance.userInfo.udCoin.ToString();
        isRequestToGetReward = false;
        PlayFloatCoin(rewardData.rewardCoin, () =>
        {
            UIController.HidePage(this);
        });
    }

    #endregion

    private void UpdateUIRewardCoin(int delta)
    {
        int prevCoin = m_AddCoin;
        m_AddCoin += delta;

        LMotion.Create(prevCoin, m_AddCoin, 2f).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).Bind((x) =>
        {
            AddCoin.text = x.ToString();
        });

    }
}