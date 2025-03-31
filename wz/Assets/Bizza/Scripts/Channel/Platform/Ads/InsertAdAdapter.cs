using Bizza.Channel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza.Channel
{
    public abstract class InsertAdAdapter : IAdAdapter //: InstanceClass<InsertAdAdapter>
    {
        protected Action onSuccess = null;
        protected Action onFail = null;
        public abstract double ECPM { get; }

        public abstract string Name { get; }

        public abstract void InitInsertAds();
        public abstract void LoadInsertAds(string[] adId);
        public abstract bool CheckIsReady();
        public abstract void ShowAds(string adId, Action success, Action fail = null);
    }

    public class DebugInsertAdAdapter : InsertAdAdapter
    {
        public bool opening = false;
        public override double ECPM => 0;

        public override string Name => "DEBUG";

        public DebugInsertAdAdapter()
        {
            //EventModule.AddListener(E_GameEvent.OnDebugInsertADClose, OnClose);
        }

        public override void InitInsertAds()
        {
        }

        public override bool CheckIsReady()
        {
            return !opening;
        }

        public override void LoadInsertAds(string[] adId)
        {
            
        }

        public override void ShowAds(string adId, Action success, Action fail = null)
        {
            opening = true;
            //EventModule.BroadCast(E_GameEvent.ShowDebugInsertAD, true);
        }
    }
}

