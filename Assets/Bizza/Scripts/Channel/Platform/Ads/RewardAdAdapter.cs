using System;
using UnityEngine;

namespace Bizza.Channel
{
    public interface IAdAdapter
    {
        double ECPM { get; }

        string Name { get; }

        bool CheckIsReady();
        void ShowAds(string adId, Action success, Action fail = null);
    }

    public abstract class RewardAdAdapter : IAdAdapter
    {
        protected Action onReward = null;
        protected Action onShowFail = null;
        public Action onLoadFail = null;
        public Action onShowFinish = null;

        public abstract double ECPM { get; }

        public abstract string Name { get; }

        protected virtual void ShowRewardAdFinish()
        {
            onReward = null;
            onShowFail = null;
            onShowFinish?.Invoke();
        }

        protected virtual void ShowRewardAdFail()
        {
            onShowFail?.Invoke();
        }

        protected virtual void LoadRewardAdFail()
        {
            Debug.Log("加载失败");
            onLoadFail?.Invoke();
            ShowRewardAdFail();
            ShowRewardAdFinish();
        }

        protected virtual void OnReward()
        {
            onReward?.Invoke();
        }

        public abstract void InitRewardAds();
        public abstract void LoadRewardAds(string[] adsIds);
        public abstract bool CheckIsReady();
        public abstract void ShowAds(string adId, Action success, Action fail = null);
    }
}