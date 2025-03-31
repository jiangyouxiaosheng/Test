using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRegionItem : MonoBehaviour
{
    public Button btnSelect;
    public GameObject select;
    public RawImage ImgBank, ImgFlag;
    public TMP_Text txtCountryCode;

    [HideInInspector] public string countryCode;

    public System.Action<UIRegionItem> onItemClick;

    public void OnItemClick()
    {
        onItemClick?.Invoke(this);
    }

    public void DownloadLogo(string url)
    {
        HttpController.Instance.RequestDownloadRawImage(url,
            (t) => { ImgBank.texture = t; });
    }

    public void DownloadFlag(string url)
    {
        HttpController.Instance.RequestDownloadRawImage(url,
            (t) => { ImgFlag.texture = t; });
    }
}