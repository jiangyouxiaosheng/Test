using System;
using System.Collections.Generic;
using Bizza.Channel.Analytics;
using Bizza;
using Bizza.Channel;

public static class AnalyticsHelper
{
    private static readonly Dictionary<string, object> publicParams = new();
    private static readonly Dictionary<string, object> userProperties = new();
    private static readonly Dictionary<string, object> cacheParams = new();

    public static Dictionary<string, object> NewEventParamsDictionary
    {
        get
        {
#if UsingThinkingData
            //if (!ThinkingData.Analytics.TDAnalytics.track)
            {
                return new Dictionary<string, object>();
            }
#endif
            cacheParams.Clear();
            return cacheParams;
        }
    }

    public static void SetPublicParam(IEnumerable<KeyValuePair<string, object>> @params)
    {
        foreach (var param in @params)
        {
            publicParams[param.Key] = param.Value;
        }

        ThinkingDataAdapter.Instance?.SetSuperProperties(publicParams);
    }

    public static void SetPublicParam(string key, object value)
    {
        publicParams[key] = value;
        ThinkingDataAdapter.Instance?.SetSuperProperties(publicParams);
    }

    public static void SetUserProperty(string key, object value)
    {
        userProperties[key] = value;
    }

    public static void UploadUserProperty()
    {
        ThinkingDataAdapter.Instance?.SetUserProperties(userProperties);
    }

    public static void SendCustomEvent(string eventName, Dictionary<string, object> @params = null)
    {
#if UNITY_EDITOR || DEBUG_MODE
        string content = string.Concat("[打点] ", "<color=white>", eventName, "</color>", " | ", @params.ToPairString());
        UnityEngine.Debug.Log(content);
#endif
        ThinkingDataAdapter.Instance?.SendCustomEvent(eventName, @params);
    }

    public static void SendRegister()
    {
        ThinkingDataAdapter.Instance?.SetUserOnceProperties(new()
            { { AnalyticsParamKey.registerTime, Bizza.DataTimeTool.Utc8Now } });
        SendCustomEvent(AnalyticsEventName.register);
#if Bizza_Platform_P8wx
        if (Platform.Instance is P8wxPlatform p8wxPlatform)
        {
            p8wxPlatform.SendFirstLoginEvent();
        }
#endif
    }

    public static void SendLogin()
    {
        SetUserProperty(AnalyticsParamKey.lastLoginTime, Bizza.DataTimeTool.Utc8Now);
        UploadUserProperty();
        SendCustomEvent(AnalyticsEventName.login);
#if Bizza_Platform_P8wx
        if (Platform.Instance is P8wxPlatform p8wxPlatform)
        {
            p8wxPlatform.SendLoginEvent();
        }
#endif
    }

    public static void SendStageStart(string levelName)
    {
        Dictionary<string, object> @params = NewEventParamsDictionary;
        @params.Add(AnalyticsParamKey.stageId, levelName);
        SendCustomEvent(AnalyticsEventName.stageStart, @params);
    }

    public static void SendStageFinish(string levelName)
    {
        Dictionary<string, object> @params = NewEventParamsDictionary;
        @params.Add(AnalyticsParamKey.stageId, levelName);
        SendCustomEvent(AnalyticsEventName.stageFinish, @params);
    }

    public static void SendStageFail(string levelName)
    {
        Dictionary<string, object> @params = NewEventParamsDictionary;
        @params.Add(AnalyticsParamKey.stageId, levelName);
        SendCustomEvent(AnalyticsEventName.stageFail, @params);
    }
}