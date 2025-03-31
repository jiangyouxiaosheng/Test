using System.Collections.Generic;
using OPS.Obfuscator.Attribute;

[DoNotRename]
public enum E_GameEvent
{

    ShowMask,
    RefreshCoin,
    //网络消息
    Network = 10000,
    STRefreshResponse,
    GetAdsRewardFromServerResponse,
    GetWithdrawalInfoResponse,
    WithdrawalObtainResponse,
    ChangeCountryResponse,
    GetCountryListResponse,
    OrderRecordListResponse,
    RefreshUI,//刷新UI
    CanInput,//是否可以控制玩家
    UI_GetCoinAnim,//金币动画是否播放
    GetAdRewardCoinCallBack,//获取广告奖励金币的返回值
    //topon激励广告的各种事件
    Topon_RewardAdCanGetRewardCoin,
    Topon_RewardAdStarted,//展示激励广告主要用于获得Ecpm
    Topon_InterstitialShown,//展示插屏广告主要用于获得Ecpm
    Topon_InterstitialClosed,//关闭插屏广告
    UI_RefreshCoin,
    
    
    
    SetAdidNull,
}

[DoNotRename]
public class GameEventComparer : IEqualityComparer<E_GameEvent>
{
    public bool Equals(E_GameEvent x, E_GameEvent y)
    {
        return (int)x == (int)y;
    }

    public int GetHashCode(E_GameEvent obj)
    {
        return (int)obj;
    }
}