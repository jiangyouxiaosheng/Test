using System;
using UnityEngine;

namespace Bizza.Channel
{
    [CreateAssetMenu(fileName = "ChannelConfig", menuName = "BizzaGame/ChannelConfig")]
    public class ChannelConfig : ChannelConfigBase<ChannelConfig>
    {
        public string appId = string.Empty;
        public string appSecret = string.Empty;
        public E_PlatformType platformType = E_PlatformType.Editor;
        public AdConfig adConfig = new();
        public ThinkingDataConfig tkConfig = new();
    }
}