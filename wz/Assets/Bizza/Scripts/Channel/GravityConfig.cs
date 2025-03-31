using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza.Channel
{

    /// <summary>
    /// 引力引擎配置
    /// </summary>
    [CreateAssetMenu(fileName = "GravityConfig", menuName = "BizzaGame/GravityConfig")]
    public class GravityConfig : ChannelConfigBase<GravityConfig>
    {
        public string accessToken = string.Empty;
        public bool debug = false;
    }
}

