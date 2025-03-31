#if Bizza_Platform_Oversea

using AnyThinkAds.Api;
using LitMotion;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Rendering;

namespace Bizza.Channel
{
    public class BannerInstance
    {
        public string bannerAdUnitId;

        public void InitBanner()
        {
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

            string[] topOn_BannerId = AccountController.Instance.stInfo.adConf.MAX_ID.BANNER;
            bannerAdUnitId = topOn_BannerId.Length > 0 ? topOn_BannerId[0] : "n676d2338a029d";

            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
        }

        public void ShowBanner()
        {
            if (!string.IsNullOrEmpty(bannerAdUnitId))
            {
                MaxSdk.ShowBanner(bannerAdUnitId);
            }
        }

        public void HideBanner()
        {
            if (!string.IsNullOrEmpty(bannerAdUnitId))
                MaxSdk.HideBanner(bannerAdUnitId);
        }

        #region 回调

        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        #endregion

    }

    public class RewardAd_AttributionInfo
    {
        public float ECPM;
        public string AT_ID;
        public string A_UID;

        public long adStar = 0;
        public long adEnd = 0;
        public long adDuration = 0;

        public string AdPlatform;
        public string AdFormat;
        public string AdNetwork;

        public string userSrc = "";
        public string lastClicPos = "";
    }

    public static class AttributionUtil
    {
        public static TenjinMono tenjin_Mono;
        public static BaseTenjin Tenjin_Instance;

        public static RewardAd_AttributionInfo RewardAdInfo = new RewardAd_AttributionInfo();

        public static void InitTenjin()
        {
            
            //Debug.LogError("AttributionUtil InitTenjin" + RetryTimes);

            if (tenjin_Mono == null)
            {
                //Debug.LogError("Tenjin Create MONO WRAPPER" + RetryTimes);

                tenjin_Mono = new GameObject("TenjinMono").AddComponent<TenjinMono>();
            }
        }

        public static void DoTenjinConnect(BaseTenjin tenjin)
        {
            
            Tenjin_Instance = tenjin;
            RetryTimes = 0;
            HasAttributionInfo = false;
            Tenjin_Instance.Connect();
            LMotion.Create(0, 0, 5).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
            {
                LoopCall();
                
            }).RunWithoutBinding();
          
        }

        private static int RetryTimes = 0;
        private static bool HasAttributionInfo = false;

        private static void LoopCall()
        {
            RetryTimes++;
            if (HasAttributionInfo)
            {
                return;
            }
          
            
            DoConnect();
            
            LMotion.Create(0, 0, 30).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
            {
                if (RetryTimes > 5)
                {
                    Debug.LogError("重试5次仍然没有获取到 Tenjin GetAttributionInfo");
                    return;
                }

                Debug.LogError("重试 Tenjin GetAttributionInfo " + RetryTimes);

                if (!HasAttributionInfo)
                {
                    LoopCall();
                }
            }).RunWithoutBinding();
        }

        private static void DoConnect()
        {
            if (HasAttributionInfo)
            {
                return;
            }
          
            Debug.Log("Tenjin DoConnect" + RetryTimes);
            

            Debug.Log("Tenjin SubscribeTopOnImpressions" + RetryTimes);

            //Tenjin_Instance.SubscribeTopOnImpressions();
            
            Tenjin_Instance.SubscribeAppLovinImpressions();

            Debug.Log("Tenjin GetAttributionInfo" + RetryTimes);
            
            Tenjin_Instance.GetAttributionInfo((x) =>
            {
                if (x == null || x.Count == 0)
                {
                    Debug.LogError("Tenjin GetAttributionInfo Get NULL");
                }
                else
                {
                    UploadAdNetwork(x);
                }
            });
        }

        public static void UploadAdNetwork(Dictionary<string, string> attributionInfoData)
        {
            HasAttributionInfo = true;

            if (attributionInfoData.ContainsKey("advertising_id"))
                AnalyticsHelper.SetUserProperty("advertising_id", attributionInfoData["advertising_id"]);

            if (attributionInfoData.ContainsKey("ad_network"))
                AnalyticsHelper.SetUserProperty("ad_network", attributionInfoData["ad_network"]);

            if (attributionInfoData.ContainsKey("campaign_id"))
                AnalyticsHelper.SetUserProperty("campaign_id", attributionInfoData["campaign_id"]);

            if (attributionInfoData.ContainsKey("campaign_name"))
                AnalyticsHelper.SetUserProperty("campaign_name", attributionInfoData["campaign_name"]);

            if (attributionInfoData.ContainsKey("site_id"))
                AnalyticsHelper.SetUserProperty("site_id", attributionInfoData["site_id"]);

            if (attributionInfoData.ContainsKey("creative_name"))
                AnalyticsHelper.SetUserProperty("creative_name", attributionInfoData["creative_name"]);

            if (attributionInfoData.ContainsKey("remote_campaign_id"))
                AnalyticsHelper.SetUserProperty("remote_campaign_id", attributionInfoData["remote_campaign_id"]);

            Debug.Log("Tenjin Back - UploadAdNetwork");
            AnalyticsHelper.UploadUserProperty();
        }


        public static void ReportADShow(string adType, string adId)
        {
            Bizza.Analytics.Manager.AddCommonParams();
            Bizza.Analytics.Manager.AddParam("ad_type", adType);

            if (adType == "Reward")
            {
                Bizza.Analytics.Manager.AddParam("ad_position", AdsUtils.LastShow_RewardAdEvent);
            }

            if (adType == "Insert")
            {
                Bizza.Analytics.Manager.AddParam("ad_position", AdsUtils.LastShow_InsertAdEvent);
            }

            Bizza.Analytics.Manager.AddParam("placement_id", adId);
            Bizza.Analytics.Manager.SendCustomEvent("Ad_Show");
        }


        public static void ReportADRevenue(string country, double publisher_revenue, string network_name,
            string adunit_id, string adunit_format, string adsource_id,string platform)
        {
            Debug.Log("上报数数的参数"+"countrycode"+country+"revenue"+publisher_revenue+"networkname"+network_name+"adunitid"+ adunit_format+"adsourceid"+adsource_id+"platform"+platform+"adformat"+adunit_format);
            Bizza.Analytics.Manager.AddCommonParams();
            Bizza.Analytics.Manager.AddParam("countrycode", country);
            Bizza.Analytics.Manager.AddParam("revenue", publisher_revenue);
            Bizza.Analytics.Manager.AddParam("networkname", network_name);
            Bizza.Analytics.Manager.AddParam("adunitid", adunit_id);
            Bizza.Analytics.Manager.AddParam("adformat", adunit_format);
            Bizza.Analytics.Manager.AddParam("adsource", adsource_id);
            Bizza.Analytics.Manager.AddParam("ADplatform", platform);
            Bizza.Analytics.Manager.SendCustomEvent("Ad_Revenue");
        }
    }

    public class OverseaPlatform : Platform
    {
        public override E_PlatformType Type => E_PlatformType.Ios;

        private TopOnInsertAd toponInsertAd = new();
        private TopOnRewardAd toponRewardAd = new();
        private MaxInsertAd maxInsertAd = new();
        private MaxRewardAd maxRewardAd = new();

        public BannerInstance BannerAd = new();

        public static bool HadWatchedAD = true;

        public OverseaPlatform() : base(true)
        {
            Login();
        }

        public void Login()
        {
            ////TOPON
            ////TOPON
            object rawTopOnAppId = AccountController.Instance.stInfo.adConf.topOnAppId;
            object rawTopOnAppKey = AccountController.Instance.stInfo.adConf.topOnAppKey;

            string topOn_AppId = rawTopOnAppId is string ? (string)rawTopOnAppId : "";
            string topOn_AppKey = rawTopOnAppKey is string ? (string)rawTopOnAppKey : "";
            if (!string.IsNullOrEmpty(topOn_AppId) && !string.IsNullOrEmpty(topOn_AppKey))
            {
                ATSDKAPI.initSDK(topOn_AppId, topOn_AppKey);
            }


            #region  MAX
            MaxSdk.InitializeSdk();
            MaxSdk.SetVerboseLogging(false);
            MaxSdkCallbacks.OnSdkInitializedEvent += (x) =>
            {
                InitAds();
                #region BANNER

                LMotion.Create(0, 0, 1).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
                {
                   // BannerAd.InitBanner();
                }).RunWithoutBinding();
                LMotion.Create(0, 0, 5).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
                {
                    CanPlayAd = true;
                }).RunWithoutBinding();
                LMotion.Create(0, 0, 2).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
                {
                  //  BannerAd.ShowBanner();
                }).RunWithoutBinding();

                #endregion
            };

            #endregion

            //是否看过了广告
            HadWatchedAD = PlayerPrefs.GetInt("HadWatchedAD", 0) == 1;
            PlayerPrefs.SetInt("HadWatchedAD", HadWatchedAD ? 1 : 0);

            //看过了广告 直接初始化归因
            //否则 等待第一次看广告
            if (HadWatchedAD)
            {
                AttributionUtil.InitTenjin();
                HadWatchedAD = true;
            }
            else
            {
                Debug.Log("提审模式 直接弹窗ATT归因");
                //网赚模式关闭 直接执行归因初始化 目的是为了弹ATT窗 让苹果审核通过
                if (AccountController.Instance.stInfo.earningSwitch != 1)
                {
                    AttributionUtil.InitTenjin();
                    HadWatchedAD = true;
                }
            }
        }
        
        public override void ShowBanner()
        {
            base.ShowBanner();
           // BannerAd.ShowBanner();
        }

        public override void HideBanner()
        {
            base.HideBanner();
           // BannerAd.HideBanner();
        }

        #region Init

        public override void InitAds()
        {
            toponRewardAd.InitRewardAds();
            toponInsertAd.InitInsertAds();
            maxRewardAd.InitRewardAds();
            maxInsertAd.InitInsertAds();

            //预加载广告
            LoadAds();
        }

        #endregion

        #region Load

        public static bool IsTopon;
        public static bool CanPlayAd;
        public override void LoadAds()
        {
            
            Debug.Log("重新加载广告");
            //加载所有
            string[] topOn_RewardIds = AccountController.Instance.stInfo.adConf.TOPON_ID.REWARDED_VIDEO;
            toponRewardAd.LoadRewardAds(topOn_RewardIds);
            Debug.Log("加载Topon Reward... ");
            string[] topOn_InterIds = AccountController.Instance.stInfo.adConf.TOPON_ID.INTERSTITIAL;
            toponInsertAd.LoadInsertAds(topOn_InterIds);
            Debug.Log("加载Topon Insert... ");
            if (CanPlayAd)
            {
                string[] max_RewardIds = AccountController.Instance.stInfo.adConf.MAX_ID.REWARDED_VIDEO;
                maxRewardAd.LoadRewardAds(max_RewardIds);
                Debug.Log("加载MAX Reward... ");
                string[] max_interIds = AccountController.Instance.stInfo.adConf.MAX_ID.INTERSTITIAL;
                maxInsertAd.LoadInsertAds(max_interIds);
                Debug.Log("加载MAX Insert... ");
            }
        }

        #endregion

        #region Show

        public override void ShowAds(string adsID = "", Action<bool> closeCallback = null)
        {
         
            //回调方法
            void OnAdsRewardSuccess()
            {
                Debug.Log("观看成功");
                //预加载新一轮广告
               
                LoadAds();
                //延迟2秒再回调
                LMotion.Create(0, 0, 2).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
                {
                    EventModule.BroadCast(E_GameEvent.ShowMask, false);
                    closeCallback?.Invoke(true);
                    Debug.Log("观看视频成功");
                  
                  
                }).RunWithoutBinding();
            }

            void OnAdsRewardFail()
            {
                Debug.Log("观看失败");
                //预加载新一轮广告
                LoadAds();
                //延迟2秒再回调
                LMotion.Create(0, 0, 2).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
                {
                    EventModule.BroadCast(E_GameEvent.ShowMask, false);
                    closeCallback?.Invoke(false);
                    Debug.Log("展示视频失败");
                    
                   
                }).RunWithoutBinding();
            }
            Debug.Log("max的激励广告价格"+maxRewardAd.ECPM+"max的插屏广告价格"+maxInsertAd.ECPM );
            // 修正后的代码
            List<string> adEntries = new List<string>();
            // TopOn Reward
            if (!string.IsNullOrEmpty(toponInsertAd.InsertAD_ID)) 
            {
                Debug.Log("topon插屏广告id"+toponInsertAd.InsertAD_ID);
                adEntries.Add($"{{\"key\":\"{toponInsertAd.InsertAD_ID}\",\"price\":{-1}}}");
            }

            if (toponRewardAd.RewardID != null && toponRewardAd.RewardID.Length > 0 && !string.IsNullOrEmpty(toponRewardAd.RewardID[0]))
            {
                Debug.Log("topon激励广告id" + toponRewardAd.RewardID[0]);
                adEntries.Add($"{{\"key\":\"{toponRewardAd.RewardID[0]}\",\"price\":{-1}}}");
            }
            if (!string.IsNullOrEmpty(maxRewardAd.AdUnitId)  && maxRewardAd.ECPM != 0) 
            {
                
                // MAX Reward
                adEntries.Add($"{{\"key\":\"{maxRewardAd.AdUnitId}\",\"price\":{maxRewardAd.ECPM}}}");
                Debug.Log("max激励广告id"+ maxRewardAd.AdUnitId);
            }
            if (!string.IsNullOrEmpty(maxInsertAd.AdUnitId) && maxInsertAd.ECPM != 0) 
            {
                // MAX Insert
                adEntries.Add($"{{\"key\":\"{maxInsertAd.AdUnitId}\",\"price\":{maxInsertAd.ECPM}}}");
                Debug.Log("max插屏广告id"+ maxInsertAd.AdUnitId);
            }
            string priceInfo = $"[{string.Join(",", adEntries)}]";
            if (adEntries.Count > 0 && CanPlayAd)
            {    
                
                var target = IOSPluginAdapter.CallComparePrice(priceInfo);

                if (target.Count > 0 && target.Keys.Any())
                {
                    if (target.Keys.First() == maxRewardAd.AdUnitId)
                    {
                        Debug.Log("Winner: maxRewardAd");
                        IsTopon = false;
                        maxRewardAd.ShowAds(adsID, OnAdsRewardSuccess, OnAdsRewardFail);
                    }
                    else if(target.Keys.First() == maxInsertAd.AdUnitId)
                    {
                        Debug.Log("Winner: maxInsertAd");
                        IsTopon = false;
                        maxInsertAd.ShowAds(adsID, OnAdsRewardSuccess, OnAdsRewardFail);
                    }
                    else if(target.Keys.First() == toponInsertAd.InsertAD_ID)
                    {
                        Debug.Log("Winner: toponInsertAd");
                        IsTopon = true;
                        toponInsertAd.ShowAds(adsID, OnAdsRewardSuccess, OnAdsRewardFail);
                    }
                    else if (target.Keys.First() == toponRewardAd.RewardID[0]) 
                    {
                        Debug.Log("Winner: toponRewardAd");
                        IsTopon = true;
                        toponRewardAd.ShowAds(adsID, OnAdsRewardSuccess, OnAdsRewardFail);
                    }
                    else
                    {
                        IsTopon = false;
                        Debug.Log("出错，未能选出获胜广告，播放max的激励广告");
                        maxRewardAd.ShowAds(adsID, OnAdsRewardSuccess, OnAdsRewardFail);
                    }
                }
                else
                {
                    OnAdsRewardFail();
                }
            }
            else
            {
                OnAdsRewardFail();
            }
        }

        #endregion

        //1. 初始化广告平台
        //2. 预加载广告
        //3. 优先展示最高ecpm广告
        //4. 同一回调信息
    }


}
#endif