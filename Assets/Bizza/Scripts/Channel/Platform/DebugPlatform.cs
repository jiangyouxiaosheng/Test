using LitMotion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza.Channel
{
    public class DebugPlatform : Platform, IShortcutPlatform
    {
        public DebugPlatform() : base(true)
        {
        }

        public override E_PlatformType Type => E_PlatformType.Debug;
        private readonly DebugRewardAdAdapter reward = new();
        private readonly DebugInsertAdAdapter insert = new();

        public void ShortcutExist(Action<bool> callback)
        {
            LitMotionUtil.SetTimeOut(() => callback(false), 0.2f);
        }

        public void CreateShortcut(Action<bool> callback)
        {
            LitMotionUtil.SetTimeOut(() => callback(true), 0.5f);
        }

        public override void InitAds()
        {
            
        }

        public override void LoadAds()
        {
            
        }

        public override void ShowAds(string adsID = "", Action<bool> closeCallback = null)
        {

            LMotion.Create(0, 1, 3).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() => 
            {
                EventModule.BroadCast(E_GameEvent.ShowMask, false);
                if (closeCallback != null)
                {
                    closeCallback(true);
                }
            }).RunWithoutBinding();

         
        }
    }
}