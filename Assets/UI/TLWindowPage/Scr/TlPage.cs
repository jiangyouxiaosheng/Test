using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TlPage : UIPageBase
{
    public override EUIPageType PageType  => EUIPageType.TlWindowPage;
    public TlPre tlPre;
    public Transform content;
    public Button closeBtn;
    
    protected override void OnOpen()
    {
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(()=>
        {
            UIModule.Instance.ClosePage(this);
        });
        // 调用带量接口
        AccountController.Instance.GetProductReferral((success, products) => {
            if (success && products.Count > 0)
            {
                foreach (var product in products)
                {
                    if (product.cachedTexture != null)
                    {
                        // 在UI显示产品信息
                        Debug.Log($"显示产品：{product.title}\n" +
                                  $"描述：{product.desc}\n");
                        var obj = Instantiate(tlPre, content);
                        obj.Init(product);
                    }
                    else
                    {
                        Debug.LogWarning($"产品 {product.title} 缺少图标");
                    }
                }
            }
            else
            {
                Debug.LogError("无法获取带量产品列表");
                // 显示错误提示
            }
        });

    }

  


    protected override void OnShow()
    {
        
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
    {
      
    }
}
