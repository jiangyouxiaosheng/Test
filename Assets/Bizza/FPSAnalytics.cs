using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSAnalytics : MonoBehaviour
{
    private int totalFrame = 0;
    private int lagFrame = 0;
    private float totalTime = 0;

    private void OnEnable()
    {
        totalFrame = 0;
        lagFrame = 0;
        totalTime = 0;
    }

    private void OnDisable()
    {
        //float avg = totalTime / totalFrame * 1000f;
        //float lagProp = (float)lagFrame / totalFrame * 100f;
        //const string eventName = Bizza.Analytics.EventName.GameFPS;
        //Bizza.Analytics.Manager.AddParam("avgFrameTime", avg.ToString("f1"));
        //Bizza.Analytics.Manager.AddParam("logFrameProp", lagProp.ToString("f1"));
        //Bizza.Analytics.Manager.SendCustomEvent(eventName);
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        totalTime += dt;
        totalFrame++;
        const float limit = 1f / 30f;
        if (dt > limit)
        {
            lagFrame++;
        }
    }



    //bool enable = true;
    //private IEnumerator GetC()
    //{
    //    int totalFrame = 0;
    //    int lagFrame = 0;
    //    float totalTime = 0;
    //    while (enable)
    //    {
    //        float dt = Time.unscaledDeltaTime;
    //        totalTime += dt;
    //        totalFrame++;
    //        const float limit = 1f / 30f;
    //        if (totalTime > limit)
    //        {
    //            lagFrame++;
    //        }
    //        yield return null;
    //    }
    //}
}
