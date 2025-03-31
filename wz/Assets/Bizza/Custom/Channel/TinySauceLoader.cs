// #define UsingVoodoo

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bizza.Channel.Analytics
{
    public class TinySauceLoader : MonoBehaviour
    {
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
        }
    }
}

