using Bizza.Channel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaxInsertAd : InsertAdAdapter
{
    private string adUnitId;
    private string m_LastAtid;
    private double ecpm;
    private bool m_IsReward;
    public string m_curPlacement;
    private bool m_loadingAds = false;

    private int retryAttempt = 0;
    public override double ECPM
    {
        get
        {
            if (CheckIsReady()) return ecpm;
            Debug.Log("MAX_Insert 没有准备好 ECPM为0");
            return 0;
        }
    }

    public override string Name => "MAX_Insert";
    public string AdUnitId => adUnitId;
    public override void InitInsertAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
    }

    public override bool CheckIsReady()
    {
        if (string.IsNullOrEmpty(adUnitId)) return false;

        return MaxSdk.IsInterstitialReady(adUnitId);
    }

    public override void LoadInsertAds(string[] adId)
    {
        if (adId == null)
        {
            adUnitId = "";
            return;
        }
        Debug.Log("MAX插屏广告"+ adId[0]);
        adUnitId = adId[0];
        retryAttempt = 0;
        if (adId.Length < 1|| adId[0] == "") return;
        
        LoadAds();
    }

    public override void ShowAds(string adId, Action success, Action fail = null)
    {
        onSuccess = success;
        onFail = fail;
        if (CheckIsReady())
        {
            MaxSdk.ShowInterstitial(adUnitId, m_curPlacement, GetExtraJsomMap());
        }
        else
        {
            onFail?.Invoke();
        }
    }

    private void LoadAds()
    {
        if (m_loadingAds || CheckIsReady()) return;
        m_loadingAds = true;
        MaxSdk.LoadInterstitial(adUnitId);

        Debug.Log("Max Insert 加载中");

    }

    private string GetExtraJsomMap()
    {
        m_LastAtid = Guid.NewGuid().ToString();

        Dictionary<string, string> customData = new Dictionary<string, string>();
        customData["bundle"] = HttpController.Instance.infoData.bundle;
        customData["vc"] = HttpController.Instance.infoData.vc.ToString();
        customData["atid"] = m_LastAtid;
        customData["ts"] = HttpController.Instance.infoData.ts.ToString();
        customData["bundle"] = HttpController.Instance.infoData.bundle;
        customData["ouid"] = HttpController.Instance.infoData.ouid;
        customData["idfa"] = HttpController.Instance.infoData.ifa;
        return HttpController.UrlSafeBase64EncodeWithAes(JsonConvert.SerializeObject(customData));

    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        m_loadingAds = false;
        ecpm = adInfo.Revenue * 1000;
        m_curPlacement = adInfo.Placement;
        Debug.Log("Max Insert callback onAdLoad :" + adInfo.Revenue + " ecpm:" + ecpm);
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        m_loadingAds = false;
        retryAttempt++;
        if (retryAttempt > 3) return;
        LoadAds();
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        m_IsReward = true;
        if (adInfo == null) return;
        AttributionUtil.RewardAdInfo.AT_ID = m_LastAtid;
        AttributionUtil.RewardAdInfo.A_UID = adInfo.AdUnitIdentifier;
        AttributionUtil.RewardAdInfo.ECPM = (float)(adInfo.Revenue * 1000);
        AttributionUtil.RewardAdInfo.AdPlatform = "MAX";
        AttributionUtil.RewardAdInfo.AdFormat = adInfo.AdFormat;
        AttributionUtil.RewardAdInfo.AdNetwork = adInfo.NetworkName;
        AttributionUtil.ReportADShow("Insert", adUnitId);
        AttributionUtil.ReportADRevenue
        (
            "", adInfo.Revenue,
            adInfo.NetworkName, adInfo.AdUnitIdentifier,
            adInfo.AdFormat, adInfo.CreativeIdentifier,"max"
        );
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        onFail?.Invoke();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        Debug.Log("max插屏广告播放成功"+ m_IsReward );
        if (m_IsReward)
        {
            onSuccess?.Invoke();
        }
        else
        {
            onFail?.Invoke();
        }

        if (OverseaPlatform.HadWatchedAD == false)
        {
            OverseaPlatform.HadWatchedAD = true;
            PlayerPrefs.SetInt("HadWatchedAD", 1);

            AttributionUtil.InitTenjin();
        }
    }
}

public class MaxRewardAd : RewardTimeOutAdapter
{
    //广告Id
    private string _adUnitId;
    private bool m_IsReward;
    //重试次数
    private int retryAttempt;
    private double ecpm;
    private string m_LastAtid;
    private string m_curPlacement;
    private bool m_loadingAds = false;
    public override double ECPM
    {
        get
        {
            if (CheckIsReady()) return ecpm;

            Debug.Log("MAX_Reward 没有准备好 ECPM为0");

            return 0;
        }
    }

    public override string Name => "MaxRewardAd";
    public string AdUnitId => _adUnitId;
    public override void InitRewardAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;

        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
    }

    public override void LoadRewardAds(string[] adIds)
    {
        if (adIds == null)
        {
            _adUnitId = "";
            return;
        }
        
        Debug.Log("MAX激励广告"+ adIds[0]);
        _adUnitId = adIds[0];
        if (adIds.Length < 1||adIds[0] == "") return;
        

        retryAttempt = 0;
        LoadAds();
    }

    public override bool CheckIsReady()
    {
        if (string.IsNullOrEmpty(_adUnitId)) return false;

        return MaxSdk.IsRewardedAdReady(_adUnitId);
    }


    private string GetExtraJsomMap()
    {
        m_LastAtid = Guid.NewGuid().ToString();

        Dictionary<string, string> customData = new Dictionary<string, string>();
        customData["bundle"] = HttpController.Instance.infoData.bundle;
        customData["vc"] = HttpController.Instance.infoData.vc.ToString();
        customData["atid"] = m_LastAtid;
        customData["ts"] = HttpController.Instance.infoData.ts.ToString();
        customData["bundle"] = HttpController.Instance.infoData.bundle;
        customData["ouid"] = HttpController.Instance.infoData.ouid;
        customData["idfa"] = HttpController.Instance.infoData.ifa;
        return HttpController.UrlSafeBase64EncodeWithAes(JsonConvert.SerializeObject(customData));

    }

    private void LoadAds()
    {
        if (m_loadingAds && CheckIsReady()) return;
        Debug.Log("Max Load _adUnitId: " + _adUnitId);
        m_loadingAds = true;
        MaxSdk.LoadRewardedAd(_adUnitId);
    }

    protected override void ProtectedShowRewardAds()
    {
        if (CheckIsReady())
        {
            var jsonMap = GetExtraJsomMap();
 
            MaxSdk.ShowRewardedAd(_adUnitId, m_curPlacement, jsonMap);
        }
    }

    protected override void ProtectedDestroyRewardAd()
    {
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttempt = 0;
        m_loadingAds = false;
        ecpm = adInfo.Revenue * 1000;
        m_curPlacement = adInfo.Placement;
        Debug.Log("Max Reward callback onAdLoad :" + adInfo.Revenue + " ecpm:" + ecpm);
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        m_loadingAds = false;
        retryAttempt++;
        if (retryAttempt > 3)
        {
            return;
        }

        LoadAds();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {

        Debug.Log("MAX OnRewardedAdFailedToDisplayEvent");

        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        ShowRewardAdFail();
        ShowRewardAdFinish();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MAX OnRewardedAdHiddenEvent");

        // Rewarded ad is hidden. Pre-load the next ad
        if (!m_IsReward)
        {
            ShowRewardAdFail();
            ShowRewardAdFinish();
        }
        else
        {
            OnReward();
            ShowRewardAdFinish();
        }

        if (OverseaPlatform.HadWatchedAD == false)
        {
            OverseaPlatform.HadWatchedAD = true;
            PlayerPrefs.SetInt("HadWatchedAD", 1);

            AttributionUtil.InitTenjin();
        }
        
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        m_IsReward = true;

        if (adInfo == null) return;

        //Debug.Log("onAdVideoStartEvent " + adInfo.Revenue);

        //参数参考:https://help.toponad.com/cn/docs/hui-diao-xin-xi-shuo-ming-agVG?search=1
        AttributionUtil.RewardAdInfo.AT_ID = m_LastAtid;
        AttributionUtil.RewardAdInfo.A_UID = adInfo.AdUnitIdentifier;
        AttributionUtil.RewardAdInfo.ECPM = (float)(adInfo.Revenue * 1000);
        AttributionUtil.RewardAdInfo.AdPlatform = "MAX";
        AttributionUtil.RewardAdInfo.AdFormat = adInfo.AdFormat;
        AttributionUtil.RewardAdInfo.AdNetwork = adInfo.NetworkName;

        AttributionUtil.ReportADShow("Reward", adUnitId);
        AttributionUtil.ReportADRevenue
        (
            "", adInfo.Revenue,
            adInfo.NetworkName, adInfo.AdUnitIdentifier,
            adInfo.AdFormat, adInfo.CreativeIdentifier,"max"
        );
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
}