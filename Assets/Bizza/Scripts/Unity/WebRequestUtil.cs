using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using LitMotion;

public static class LitMotionUtil
{
    public static void SetTimeOut(Action action, float delay)
    {
        LMotion.Create(0, 1, delay)
            .WithScheduler(LitMotion.MotionScheduler.InitializationIgnoreTimeScale)
            .WithOnComplete(action)
            .RunWithoutBinding();
    }
}

namespace Bizza.Unity
{
    public static class WebRequestUtil
    {
        public static string DictToQuery(Dictionary<string, string> keyValuePairs)
        {
            StringBuilder sb = new();
            foreach (var pair in keyValuePairs)
            {
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(pair.Key);
                sb.Append('=');
                sb.Append(pair.Value);
            }
            return sb.ToString();
        }
        
        public static void RequestGet(string url, Action<string> onSuccess, Action onFail = null, int maxCount = 3, float interval = 3f)
        {
            Do();
            return;
            
            void Do()
            {
                if (maxCount-- <=0)
                {
                    onFail?.Invoke();
                    return;
                }
                var webRequest = UnityWebRequest.Get(url);
                webRequest.timeout = 10;
                
                webRequest.SetRequestHeader("Content-Type", "application/json");
                var task = webRequest.SendWebRequest();
                task.completed += (AsyncOperation operation) =>
                {
                    if (operation is UnityWebRequestAsyncOperation _operation)
                    {
                        var _webRequest = _operation.webRequest;
                        if (_webRequest.result == UnityWebRequest.Result.Success)
                        {
                            var result = _webRequest.downloadHandler.text;
                            onSuccess?.Invoke(result);
                        }
                        else
                        {
                            LitMotionUtil.SetTimeOut(Do, interval);
                            Debug.Log("try request get again. url is ::: " + url);
                        }
                        _webRequest.Dispose();
                    }
                };
            }
        }
    }
}

