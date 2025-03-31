using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIChangeCountry : UIPage
{
    public Button btnClose, btnEnter;
    public Transform itemRoot;
    public UIRegionItem itemPref;

    private List<UIRegionItem> items = new List<UIRegionItem>();
    private string currSelectCountryCode;

    public void Awake()
    {
        btnClose.onClick.AddListener(OnBtnClose);
        btnEnter.onClick.AddListener(OnBtnOKClick);
    }

    public void OnEnable()
    {
        EventModule.AddListener<bool>(E_GameEvent.ChangeCountryResponse, OnChangeCountrySuccess);
    }

    public void OnDisbale()
    {
        EventModule.RemoveListener<bool>(E_GameEvent.ChangeCountryResponse, OnChangeCountrySuccess);
    }

    public void ShowReginlist(List<AccountController.CountryDetail> countrys)
    {
        if (countrys == null) return;
        currSelectCountryCode = AccountController.Instance.userInfo.countryCode;
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }

        items.Clear();

        for (int i = 0; i < countrys.Count; i++)
        {
            var countryDetail = countrys[i];
            for (int m = 0; m < countryDetail.paymentProvidersLogoUrl.Count; m++)
            {
                UIRegionItem itemClone = Instantiate(this.itemPref, itemRoot);
                itemClone.txtCountryCode.text = $"{countryDetail.countryCode}/{countryDetail.countryName}";
                itemClone.DownloadLogo(countryDetail.paymentProvidersLogoUrl[m]);
                itemClone.DownloadFlag(countryDetail.nationalFlagUrl);
                itemClone.btnSelect.onClick.AddListener(itemClone.OnItemClick);
                itemClone.onItemClick = OnReginItemSelect;
                itemClone.countryCode = countryDetail.countryCode;
                if (countryDetail.countryCode == currSelectCountryCode)
                {
                    itemClone.select.gameObject.SetActive(true);
                }
                else
                {
                    itemClone.select.gameObject.SetActive(false);
                }
                items.Add(itemClone);
            }
        }
        //TODO 选中默认
    }

    private void OnReginItemSelect(UIRegionItem item)
    {
        currSelectCountryCode = item.countryCode;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].countryCode == currSelectCountryCode)
            {
                items[i].select.SetActive(true);
            }
            else
            {
                items[i].select.SetActive(false);
            }
        }
    }

    public void OnBtnOKClick()
    {
        EventModule.BroadCast(E_GameEvent.ShowMask, true);

        AccountController.Instance.ChangeCountry(currSelectCountryCode);
    }
    
    public void OnBtnClose()
    {
        UIController.HidePage(this);
    }

    private void OnChangeCountrySuccess(bool success)
    {
        EventModule.BroadCast(E_GameEvent.ShowMask, false);

        if (success)
        {
            //切换成功
            AccountController.Instance.GetWithdrawalInfo();

            UIController.HidePage(this);
        }

    }
}