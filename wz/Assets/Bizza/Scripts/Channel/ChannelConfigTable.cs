using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Bizza.Channel
{
    [Serializable]
    public class AdConfig
    {
        [LabelText("激励视频广告位")] public string[] reward = new string[] { "test" }; // Ŀǰֻ�õ�һ��
        [LabelText("插屏广告位")] public string insert = "test";
    }

    [Serializable]
    public class ThinkingDataConfig
    {
        public string appId = "test";
        public string serverUrl = "https://global-receiver-ta.thinkingdata.cn";
        public bool debug = false;
    }

    [CreateAssetMenu(fileName = "ChannelConfigTable", menuName = "BizzaGame/ChannelConfigTable")]
    public class ChannelConfigTable : ScriptableObject
    {
        public const string folderPath = "Bizza/ChannelConfig/";
        public const string resourcesPath = folderPath + nameof(ChannelConfigTable);
        
#if UNITY_EDITOR
        [LabelText("渠道号")]
        [ValueDropdown(nameof(GetLayerSelection))]
#endif
        public string editorChannel = "editor";
        
#if UNITY_EDITOR
        private IEnumerable GetLayerSelection()
        {
            var array = Resources.LoadAll<ChannelConfig>(ChannelConfig.ResourcesPath);
            var valueDropList = new ValueDropdownList<string>();
            foreach (var t in array)
            {
                valueDropList.Add(t.channel, t.channel);
            }
            return valueDropList;
        }
#endif
    }
}