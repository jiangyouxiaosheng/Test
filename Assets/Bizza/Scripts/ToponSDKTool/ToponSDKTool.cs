using System.Collections;
using System.Collections.Generic;
using OPS.Obfuscator.Attribute;
using UnityEngine;
/// <summary>
/// 提供给iOS调用的方法（方法名需完全一致），不可以进行名字的更改，也不要改变挂载的物体。如果必须进行方法名字的修改，需要去对应的OC代码中也进行修改
/// </summary>
[DoNotRename]
public class ToponSDKTool : MonoBehaviour
{
    public void OnRewardedVideoSuccess(string json)
    {
        Debug.Log($"奖励发放: {json}");
        var data = JsonUtility.FromJson<RewardData>(json);
    }

    /// <summary>
    /// 开始播放topon激励广告，并且对服务器进行数据上报。
    /// </summary>
    /// <param name="json"></param>
    [DoNotRename]
    public void OnRewardedVideoStart(string json)
    {
        Debug.Log($"广告开始播放: {json}");
        AdData data = JsonUtility.FromJson<AdData>(json);
        EventModule.BroadCast(E_GameEvent.Topon_RewardAdStarted, data);
        
    }
    /// <summary>
    /// 关闭激励广告，并且发放奖励，这里我们关闭就默认播放成功了。
    /// </summary>
    /// <param name="json"></param>
    [DoNotRename]
    public void OnRewardedVideoClosed(string json)
    {
        Debug.Log($"广告关闭: {json}");
        var result = JsonUtility.FromJson<CloseData>(json);
        Debug.Log("广告事件发送" + result.rewarded);
        EventModule.BroadCast(E_GameEvent.Topon_RewardAdCanGetRewardCoin, result.rewarded);
    }
    /// <summary>
    /// 激励广告播放失败
    /// </summary>
    /// <param name="json"></param>
    public void OnRewardedVideoFailed(string json)
    {
        Debug.LogError($"广告失败: {json}");
        var error = JsonUtility.FromJson<ErrorData>(json);
    }

    /// <summary>
    /// 需要的参数
    /// </summary>
    [System.Serializable]
    class RewardData
    {
        public string placementID;
        public int amount;
    }

    [System.Serializable]
    class CloseData
    {
        public string placementID;
        public bool rewarded;
    }

    [System.Serializable]
    class ErrorData
    {
        public int code;
        public string message;
    }
    /// <summary>
    /// 自定义参数，名字也需要跟OC代码中相对应
    /// </summary>
    [System.Serializable]
    public class AdData {
        public string placementID;
        public string auid;
        public string network;
        public float ecpm;
        public string country;
        public float publisher_revenue;
        public string adunit_id;
        public string adunit_format;
        public string adsource_id;
    }
    //---------------------------------------------------------------------------插屏广告---------------------------------------------------------------
    /// <summary>
    /// 展示插屏广告，对服务发送数据
    /// </summary>
    /// <param name="json"></param>
    [DoNotRename]
    public void OnInterstitialShow(string json) {
        Debug.Log($"插屏广告展示: {json}");
        InterstitialShowData data = JsonUtility.FromJson<InterstitialShowData>(json);
        Debug.Log("插屏广告展示：");
        Debug.Log($"PlacementID: {data.placementID}");
        Debug.Log($"广告源ID: {data.auid}");
        Debug.Log($"广告平台: {data.network}");
        Debug.Log($"eCPM: {data.ecpm}");
        Debug.Log($"国家: {data.country}");
        Debug.Log($"发布者收益: {data.publisher_revenue}");
        Debug.Log($"广告单元ID: {data.adunit_id}");
        Debug.Log($"广告格式: {data.adunit_format}");
        Debug.Log($"广告源ID: {data.adsource_id}");
        EventModule.BroadCast(E_GameEvent.Topon_InterstitialShown, data);
    }
    /// <summary>
    /// 插屏广告关闭，发放奖励。
    /// </summary>
    /// <param name="json"></param>
    [DoNotRename]
    public void OnInterstitialClosed(string json) {
        Debug.Log($"插屏广告关闭: {json}");
        EventModule.BroadCast(E_GameEvent.Topon_InterstitialClosed);
       
    }
      
    // 数据模型
    [System.Serializable]
    public class InterstitialShowData {
        public string placementID;
        public string auid;
        public string network;
        public float ecpm;
        public string country;
        public float publisher_revenue;
        public string adunit_id;
        public string adunit_format;
        public string adsource_id;
    }
      
    [System.Serializable]
    class CloseInterstitialData {
        public string placementID;
    }
}
