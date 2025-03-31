using System;
using UnityEngine;

public class AdsUtils
{
    private static float s_lastShowADTime = 25;

    private static string s_AdsName;

    public const float INSERTAD_PERIOD = 30;

    private static Bizza.Channel.AdConfig adConfig;

    public const string RewardBlock = "ShowRewardAd";
    public const string InsertBlock = "ShowInsertAd";

    private static bool isBGMBeforeShowAD = false;

    //展示记录的Log
    public static string LastShow_RewardAdEvent = string.Empty;
    public static string LastShow_InsertAdEvent = string.Empty;

    public static void InitAdsUtil()
    {
        adConfig = Bizza.Channel.ChannelConfig.Instance.adConfig;
    }

    private static void InitRewardAds()
    {
        // var reward = Bizza.Channel.RewardAdAdapter.Instance;
        // if (reward == null)
        // {
        //     Debug.Log("激励视频广告模块不存在");
        // }
        // else
        // {
        //     static void OnShowRewardFinish()
        //     {
        //         // 解除阻断                
        //         EventModule.BroadCast(E_GameEvent.ShowMask, false);
        //         s_lastShowADTime = Time.time;
        //     }
        //     reward.onShowFinish += OnShowRewardFinish;
        //
        // }
    }

    private static void InitInsertAds()
    {
        // var insert = Bizza.Channel.InsertAdAdapter.Instance;
        // if (insert == null)
        // {
        //     Debug.Log("插频广告模块不存在");
        // }
        // else
        // {
        //     static void OnInsertClose()
        //     {
        //         // 解除插屏阻断
        //         // TopBlock.RemoveBlock(InsertBlock);
        //     }
        //
        //     insert.onClose += OnInsertClose;
        //
        //     string topOn_InsertId = AccountController.Instance.stInfo.adConf.TOPON_ID.INTERSTITIAL[0];
        //     //string max_InsertId = AccountController.Instance.stInfo.adConf.MAX_ID.INTERSTITIAL[0];
        //
        //     string[] ids = new string[1]
        //     {
        //         topOn_InsertId,
        //         //max_InsertId
        //     };
        //
        //     insert.InitInsertAd(ids);
        //     s_lastShowADTime = Time.time;
        // }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="adsEventName"></param>
    /// <param name="adsID"></param> 内部广告位ID
    /// <param name="closeCallback"></param>
    public static void OpenRewardAds(string adsEventName, string adsID = "", Action<bool> closeCallback = null)
    {
        if (AccountController.Instance.stInfo.dsaSw != 1) return;

        var platform = Bizza.Channel.Platform.Instance;
        if (platform == null)
        {
            return;
        }

        // 开始激励视频阻断
        EventModule.BroadCast(E_GameEvent.ShowMask, true);
        LastShow_RewardAdEvent = adsEventName;

        Debug.Log("OpenRewardAds 备注:此处做了修改 网赚产品的Adid使用的服务器下发的");

        platform.ShowAds(null, closeCallback);
        s_lastShowADTime = Time.time;
        s_AdsName = adsEventName;
    }

  
}