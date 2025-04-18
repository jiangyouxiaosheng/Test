using UnityEngine;

namespace Bizza.Channel
{
    public class ThinkingDataLoader : MonoBehaviour
    {
        public static void ManualLoad()
        {
            Debug.Log("数数初始化 ManualLoad");
            var cfg = ChannelConfig.Instance;
            string ch = cfg.channel;
            string appId_tk = cfg.tkConfig.appId;
            string serverUrl = cfg.tkConfig.serverUrl;
            bool debug = cfg.tkConfig.debug;
#if UsingThinkingData
            Analytics.ThinkingDataAdapter adapter = new();
            adapter.Init(appId_tk, serverUrl, cfg.appId, debug);
            adapter.SetUserOnceProperties(new() { { AnalyticsParamKey.channel, ch } });
#endif
        }
    }
}
