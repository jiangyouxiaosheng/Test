using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private static AdManager instance;
    public static AdManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 查找场景中是否已经存在该实例
                instance = FindObjectOfType<AdManager>();

                // 如果不存在，创建一个新的实例
                if (instance == null)
                {
                    GameObject obj = new GameObject("AdManager");
                    instance = obj.AddComponent<AdManager>();
                    DontDestroyOnLoad(obj); // 确保在场景切换时不会被销毁
                }
            }
            return instance;
        }
    }
}
