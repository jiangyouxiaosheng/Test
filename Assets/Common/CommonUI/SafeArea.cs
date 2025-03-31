using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using WeChatWASM;
#endif

public class SafeArea : MonoBehaviour
{
    void Start()
    {
        FitSafeArea();
    }

    void Update()
    {

    }

    void FitSafeArea()
    {
        /*var systemInfo = wx.getSystemInfo();
        var bannerAd = WX.CreateBannerAd(new WXCreateBannerAdParam());

        const windowInfo = wx.getSystemInfo();
        wx.getSystemInfoSync().safeArea
        Rect safeArea = windowInfo.safeArea;*/

        //Rect safeArea = Screen.safeArea;
#if UNITY_WEBGL && !UNITY_EDITOR
        var safeArea = WX.GetWindowInfo().safeArea;
        var screenHeight = (float)WX.GetWindowInfo().screenHeight;
        Debug.Log("safeArea的左上角横坐标left为" + safeArea.left + "safeArea的右下角横坐标right为" + safeArea.right + "safeArea的左上角纵坐标top为" + safeArea.top);
        Debug.Log("safeArea的右下角纵坐标bottom为" + safeArea.bottom + "safeArea的宽度width为" + safeArea.width + "safeArea的高度height为" + safeArea.height);
        Debug.Log("屏幕的高度为" + screenHeight);
        float proportion = (screenHeight - (float)safeArea.top) / screenHeight;
        RectTransform rectTrans = GetComponent<RectTransform>();
        SafeAreaOffect(rectTrans, proportion);
#else

#endif


    }

    void SafeAreaOffect(RectTransform rectTrans, float proportion)
    {
        rectTrans.anchorMax = new Vector2(rectTrans.anchorMax.x, proportion);
        rectTrans.offsetMax = new Vector2(0, 0);
    }
}
