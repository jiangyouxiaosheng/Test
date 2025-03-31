using UnityEngine;

namespace Bizza.Channel
{
    /// <summary>
    /// 渠道区分SO基类
    /// </summary>
    public abstract class ChannelConfigBase<T> : ScriptableObject where T : ChannelConfigBase<T>
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    string ch = "editor";
#if !UNITY_EDITOR
                var table = Resources.Load<ChannelConfigTable>(ChannelConfigTable.resourcesPath);
                if (table != null)
                {
                    ch = table.editorChannel;
                }
#endif
                    string path = ResourcesPath;
                    var array = Resources.LoadAll<T>(path);
                    if (array.Length != 0)
                    {
                        foreach (var cfg in array)
                        {
                            if (cfg.channel == ch)
                            {
                                instance = cfg;
                                return instance;
                            }
                        }
                    }
                    instance = CreateInstance<T>();
                }
                return instance;
            }
        }

        public string channel = "editor";
        
        public static string ResourcesPath => ChannelConfigTable.folderPath + typeof(T).Name;
    }
}