using System;
using UnityEngine;
using LitMotion;

namespace Bizza
{
    public interface ITimer
    {
        void TimeOut(float timeOut, Action callback);
        void Stop();
    }
}

public class LitMotionTimer : Bizza.ITimer
{
    private MotionHandle handler;
    private Action callback;

    public LitMotionTimer()
    {
        handler.StorageId = -1;
    }

    void OnComplete()
    {
        handler.StorageId = -1;
        this.callback?.Invoke();
        this.callback = null;
    }

    public void TimeOut(float timeOut, Action callback)
    {
        if (handler.StorageId > -1)
        {
            return;
        }

        this.callback = callback;
        handler = LMotion.Create(0f, 1f, timeOut)
            .WithScheduler(MotionScheduler.InitializationIgnoreTimeScale)
            .WithOnComplete(OnComplete)
            .RunWithoutBinding();
    }

    public void Stop()
    {
        if (handler.StorageId > -1)
        {
            try
            {
                MotionHandleExtensions.Cancel(handler);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            handler.StorageId = -1;
            callback = null;
        }
    }
}

namespace Bizza.Channel
{
    public abstract class RewardTimeOutAdapter : RewardAdAdapter
    {
        protected ITimer timer = new LitMotionTimer();
        public float maxWaitTime = 10;

        protected override void ShowRewardAdFinish()
        {
            Debug.Log("ShowRewardAdFinish");
            onReward = null;
            onShowFail = null;
            onShowFinish?.Invoke();
            timer.Stop();
            ProtectedDestroyRewardAd();
        }

        protected abstract void ProtectedShowRewardAds();
        protected abstract void ProtectedDestroyRewardAd();

        public override void ShowAds(string adId, Action success, Action fail = null)
        {
            onReward = success;
            onShowFail = fail;
            //请求展示广告 
            bool isShowSuccess = CheckIsReady();
            Debug.Log("广告加载中"+isShowSuccess);
            if (!isShowSuccess)
            {
                timer.TimeOut(maxWaitTime, () => { LoadRewardAdFail(); });
            }
            else
            {
                ProtectedShowRewardAds();
            }
        }
    }

    public class DebugRewardAdAdapter : RewardTimeOutAdapter
    {
        public override double ECPM => 0;

        public override string Name => "DebugRewardAdAdapter";

        public override void InitRewardAds()
        {
            // not need
        }

        public override void LoadRewardAds(string[] adIds)
        {
            
        }

        public override bool CheckIsReady()
        {
            return true;
        }

        protected override void ProtectedShowRewardAds()
        {
            const float rate = 100;
            const float delay = 2f;
            bool success = UnityEngine.Random.Range(0, 100) < rate;
            LMotion.Create(0f, 1f, delay)
                .WithScheduler(MotionScheduler.InitializationIgnoreTimeScale)
                .WithOnComplete(success ? Success : Fail)
                .RunWithoutBinding();

            void Success()
            {
                OnReward();
                ShowRewardAdFinish();
            }

            void Fail()
            {
                ShowRewardAdFail();
                ShowRewardAdFinish();
            }
        }

        protected override void ProtectedDestroyRewardAd()
        {
            // not need
        }
    }
}