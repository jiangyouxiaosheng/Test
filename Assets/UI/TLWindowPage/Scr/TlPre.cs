
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TlPre : MonoBehaviour
{
   public RawImage appIcon;
   public TMP_Text appName;
   public TMP_Text appDescription;
   public Button appdownload;
   
   public void Init(AccountController.ProductInfo info)
   {
      appName.text = info.title;
      appDescription.text = info.desc;
      appIcon.texture = info.cachedTexture;
      appdownload.onClick.RemoveAllListeners();
      appdownload.onClick.AddListener(() =>
      {
         Debug.Log(info.downloadUrl);
         Application.OpenURL(info.downloadUrl);
      });
   }
}
