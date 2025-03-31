using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bizza.Analytics
{
    public class TinySauceManager : MonoBehaviour
    {
        private static bool needInit = true;

        public void Awake()
        {
            static void LoadScene(bool adConsent, bool trackingConsent)
            {
                SceneManager.LoadScene(1);
            }

#if UsingVoodoo
            TinySauce.SubscribeOnInitFinishedEvent(LoadScene);
#else
            LoadScene(false, false);
#endif

            // Manager.OnCustomEvent += SendCustomEvent;

            needInit = false;
        }

        public static void SendCustomEvent(string eventName, Dictionary<string, object> @params)
        {
#if UsingVoodoo
            if (needInit) { return; }
            TinySauce.TrackCustomEvent(eventName, @params);
#endif
        }

        public static void SendGameStartEvent(string levelName)
        {
#if UsingVoodoo
            if (needInit) { return; }
            TinySauce.OnGameStarted(levelName);
#endif
        }

        public static void SendGameFinishEvent(string levelName, bool success, float score)
        {
#if UsingVoodoo
            if (needInit) { return; }
            TinySauce.OnGameFinished(success, score, levelName);
#endif
        }
    }
}

