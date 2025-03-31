using AnyThinkAds.Api;
using Bizza.Channel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using AnyThinkAds.ThirdParty.LitJson;
using UnityEngine;

public class TopOnInsertAd : InsertAdAdapter
{
    public string InsertAD_ID;
    private string m_LastAtid;
    private bool m_IsReward;
    private double ecpm;
    private bool isLoaded = false;
    public override double ECPM
    {
        get
        {
            if (CheckIsReady()) return ecpm;
            return 0;
        }
    }

    public override string Name => "TopOnInsertAd";

    public override void InitInsertAds()
    {
        EventModule.AddListener(E_GameEvent.Topon_InterstitialClosed,CloseInserAd);
        EventModule.AddListener<ToponSDKTool.InterstitialShowData>(E_GameEvent.Topon_InterstitialShown,GetInterstitialCoin );
    }

    private void GetInterstitialCoin(ToponSDKTool.InterstitialShowData data)
    {
        AttributionUtil.RewardAdInfo.ECPM = data.ecpm;
        AttributionUtil.RewardAdInfo.A_UID = data.auid;
        AttributionUtil.RewardAdInfo.AT_ID = m_LastAtid;
        AttributionUtil.RewardAdInfo.AdPlatform = "topon";
        AttributionUtil.RewardAdInfo.AdNetwork = data.network;
        AttributionUtil.RewardAdInfo.AdFormat = "ToponInsertAd";
        AttributionUtil.ReportADShow("Insert", InsertAD_ID);
        AttributionUtil.ReportADRevenue
        (
            data.country, data.ecpm/1000,
            data.network, data.adunit_id,
            "ToponInsertAd", data.adsource_id,"topon"
        );
    }


    public override bool CheckIsReady()
    {
        bool isReady = IOSPluginAdapter.IsInterstitialAdReady(InsertAD_ID);
        return isReady;
    }

    public override void LoadInsertAds(string[] adId)
    {
        if (adId == null)
        {
            InsertAD_ID = "";
            return;
        }
        if (adId.Length >= 1)
        {
            InsertAD_ID = adId[0];
        }
        if (adId.Length < 1 || adId[0] == "")
        {
            InsertAD_ID = "";
            return;
        }
        var jsonMap = GetExtraJsomMap();
        IOSPluginAdapter.LoadInterstitialAd(InsertAD_ID,JsonMapper.ToJson(jsonMap));
       
    }

    

    public override void ShowAds(string adId, Action success, Action fail = null)
    {
        onSuccess = success;
        onFail = fail;
        if (CheckIsReady())
        {
            IOSPluginAdapter.ShowInterstitialAd(InsertAD_ID,
                HttpController.UrlSafeBase64EncodeWithAes(JsonMapper.ToJson(GetExtraJsomMap())));
        }
        else
        {
            onFail?.Invoke();
        }
    }

    private Dictionary<string, string> GetExtraJsomMap()
    {
        // m_LastAtid上报给服务器的与获取奖励传递给服务器的一定要一致。
        m_LastAtid = Guid.NewGuid().ToString();
        Dictionary<string, string> jsonmap = new();
        Dictionary<string, string> customData = new Dictionary<string, string>();
        customData["bundle"] = HttpController.Instance.infoData.bundle;
        customData["vc"] = HttpController.Instance.infoData.vc.ToString();
        customData["atid"] = m_LastAtid;
        customData["ts"] = HttpController.Instance.infoData.ts.ToString();
        customData["ouid"] = HttpController.Instance.infoData.ouid;
        customData["idfa"] = HttpController.Instance.infoData.ifa;
        string data = HttpController.UrlSafeBase64EncodeWithAes(JsonConvert.SerializeObject(customData));
        jsonmap.Add(ATConst.USERID_KEY, data);
        

        return customData;
    }

    #region 回调
    
    private void CloseInserAd()
    {
        Debug.Log("开始获取插屏广告奖励");
        onSuccess?.Invoke();
        if (OverseaPlatform.HadWatchedAD == false)
        {
            OverseaPlatform.HadWatchedAD = true;
            PlayerPrefs.SetInt("HadWatchedAD", 1);

            AttributionUtil.InitTenjin();
        }
    }


    #endregion
}

public class TopOnRewardAd : RewardTimeOutAdapter
{
    public string[] RewardID = new string[0];

    private bool m_IsReward = false;
    private string m_LastShowId;

    private string m_TopOnAdId;
    private string m_LastAtid;
    private bool m_isLoaded = false;
    private double ecpm;
    public override double ECPM
    {
        get
        {
            if (CheckIsReady()) return ecpm;
            return 0;
        }
    }

    public override string Name => "TopOnRewardAd";

    public override void InitRewardAds()
    {
  
        EventModule.AddListener<bool>(E_GameEvent.Topon_RewardAdCanGetRewardCoin,ToponCanGetRewardCoin);
        EventModule.AddListener<ToponSDKTool.AdData>(E_GameEvent.Topon_RewardAdStarted,SetRewardEcpm);
      
    }

    private void SetRewardEcpm(ToponSDKTool.AdData data)
    {
        
        this.ecpm = ecpm;
        AttributionUtil.RewardAdInfo.ECPM = data.ecpm;
        AttributionUtil.RewardAdInfo.A_UID = data.auid;
        AttributionUtil.RewardAdInfo.AT_ID = m_LastAtid;
        AttributionUtil.RewardAdInfo.AdPlatform = "topon";
        AttributionUtil.RewardAdInfo.AdNetwork = data.network;
        AttributionUtil.RewardAdInfo.AdFormat = "ToponReward";
        AttributionUtil.ReportADShow("Reward", RewardID[0]);
        AttributionUtil.ReportADRevenue
        (
            data.country, data.ecpm / 1000,
            data.network, data.adunit_id,
            "ToponReward", data.adsource_id, "topon"
        );

    }

    public override void LoadRewardAds(string[] adsIds)
    {
        if (adsIds == null)
        {
            RewardID = new string[1];
            RewardID[0] = "";
            m_TopOnAdId = "";
            return;
        }
        if (adsIds.Length >= 1)
        {
            RewardID = adsIds; 
        }
        if (adsIds.Length < 1 || adsIds[0] == "")
        {
            RewardID = new string[1];
            RewardID[0] = "";
            return;
        }
        m_TopOnAdId = adsIds[0];
        var jsonmap = GetExtraJsomMap();
        IOSPluginAdapter.LoadRewardeAd(m_TopOnAdId, JsonMapper.ToJson(jsonmap));
    }

    public override bool CheckIsReady()
    {
        bool isReady_TopOn = IOSPluginAdapter.IsRewardedAdReady(m_TopOnAdId);
        return isReady_TopOn;
    }

    private Dictionary<string, string> GetExtraJsomMap()
    {
        Dictionary<string, string> jsonmap = new();
        Dictionary<string, string> customData = new Dictionary<string, string>();
        //如果需要通过开发者的服务器进行奖励的下发（部分广告平台支持此服务器激励），则需要传递下面两个key
        //ATConst.USERID_KEY必传，用于标识每个用户;ATConst.USER_EXTRA_DATA为可选参数，传入后将透传到开发者的服务器
        jsonmap.Add(ATConst.USERID_KEY, HttpController.Instance.infoData.ouid);
        // m_LastAtid上报给服务器的与获取奖励传递给服务器的一定要一致。
        m_LastAtid = Guid.NewGuid().ToString();
        customData["bundle"] = HttpController.Instance.infoData.bundle;
        customData["vc"] = HttpController.Instance.infoData.vc.ToString();
        customData["atid"] = m_LastAtid;
        customData["ts"] = HttpController.Instance.infoData.ts.ToString();
        customData["ouid"] = HttpController.Instance.infoData.ouid;
        customData["idfa"] = HttpController.Instance.infoData.ifa;
        string data = HttpController.UrlSafeBase64EncodeWithAes(JsonConvert.SerializeObject(customData));
        jsonmap.Add(ATConst.USER_EXTRA_DATA, data);
        
        return  customData;
    }

    protected override void ProtectedDestroyRewardAd()
    {
        
    }

    protected override void ProtectedShowRewardAds()
    {
        if (!CheckIsReady())
        {
            ShowRewardAdFail();
            return;
        }
        m_IsReward = false;
        m_LastShowId = m_TopOnAdId;
        IOSPluginAdapter.ShowRewardedAd(m_LastShowId,HttpController.UrlSafeBase64EncodeWithAes(JsonMapper.ToJson(GetExtraJsomMap())));
      
    }

    #region TopOn回调
    
    private void ToponCanGetRewardCoin(bool canGetRewardCoin)
    {
        if (!canGetRewardCoin)
        {
            ecpm = 0;
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
   
    #endregion
}