using System;
using UnityEngine;

namespace Bizza
{
    public abstract class InstanceClass<T> where T : InstanceClass<T>
    {
        public static T Instance { get; private set; }

        public InstanceClass()
        {
            Instance = this as T;
        }
    }
}

namespace Bizza.Channel
{
    public interface IShortcutPlatform
    {
        void ShortcutExist(Action<bool> callback);
        void CreateShortcut(Action<bool> callback);
    }
}

namespace Bizza.Channel
{
    public abstract class Platform : InstanceClass<Platform>
    {
        public static bool isInit = false;

        // ---- Static ----
        public static void InitPlatform()
        {
            if (isInit)
            {
                return;
            }

            Debug.Log("BizzaPlatform InitPlatform 加载...");
            isInit = true;

            var cfg = ChannelConfig.Instance;
            switch (cfg.platformType)
            {
                case E_PlatformType.Editor:
                case E_PlatformType.Debug:
                    new DebugPlatform();
                    break;
#if Bizza_Platform_WeChat
                case E_PlatformType.WeChat:
                    new WeChatPlatform();
                    break;
#endif
#if Bizza_Platform_ByteNative
                case E_PlatformType.ByteNative:
                    new ByteNativePlatform();
                    break;
#endif

#if Bizza_Platform_Oversea
                case E_PlatformType.Oversea:
                    new OverseaPlatform();
                    break;
#endif
            }
        }
        // ---- Instance ----
        
        public abstract E_PlatformType Type { get; }
        protected PlayerPrefsSystem saveSystem;
        public PlayerPrefsSystem SaveSystem => saveSystem;
        public virtual string version => Application.version;
        
        protected Platform(bool @default)
        {
            if (@default)
            {
                saveSystem = new UnityPlayerPrefs();
            }
        }

        public abstract void InitAds();
        public abstract void LoadAds();
        public abstract void ShowAds(string adsID = "", Action<bool> closeCallback = null);

        public virtual void ShowBanner()
        {
            
        }

        public virtual void HideBanner()
        {
            
        }
    }
}