using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class UIWithdrawal : UIPage
{
    public Button btnBack, btnHistory, btnChangeCountry, btnWithdrawal;
    public TMP_Text txtMyBalance, txtCountryCode, txtRewardAmount;
    public RawImage imgCountryFlag;
    public GameObject rewardRoot;
    public GameObject bankRoot;
    public UIWithdrawalItem WithdrawalItem;
    public UIWithdrawalBankFlag WithdrawalBankFlag;


    private List<UIWithdrawalBankFlag> banks = new();
    private List<UIWithdrawalItem> items = new();

    private int bankSelectIdx = -1;
    private int itemSelectIdx = -1;


    public void Awake()
    {
        
        btnBack.onClick.AddListener(() => { UIController.HidePage(this); });
        btnHistory.onClick.AddListener(() => { UIController.ShowPage<UIOrderHistory>(); });
        btnChangeCountry.onClick.AddListener(OnBtnChangeCountryClick);
        btnWithdrawal.onClick.AddListener(OnBtnWithdrawalClick);
    }

    public void OnEnable()
    {
        //_uiFadeAnimation.Show();
        UIController.HidePage<UIBizzaAAA>();
        EventModule.AddListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse,
            Init);
        EventModule.AddListener<AccountController.CountryList>(E_GameEvent.GetCountryListResponse,
            OnGetChangeCountryListResponse);
    }

    public void OnDisable()
    {
        //_uiFadeAnimation.Hide(onCompleted: () => { UIController.OnPageClosed(this); });
        UIController.ShowPage<UIBizzaAAA>();
        EventModule.RemoveListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse,
            Init);
        EventModule.RemoveListener<AccountController.CountryList>(E_GameEvent.GetCountryListResponse,
            OnGetChangeCountryListResponse);
    }

    public void Init(AccountController.WithdrawalInfo withdrawalInfo)
    {
        string countryCode = withdrawalInfo.countryCode;

        txtMyBalance.text = $"{withdrawalInfo.currencyCode} {withdrawalInfo.udAmount}";
        txtCountryCode.text = $"{countryCode}/{withdrawalInfo.currencyCode}";
        txtRewardAmount.text = "--";

        HttpController.Instance.RequestDownloadRawImage(withdrawalInfo.nationalFlagUrl,
            (s) => { imgCountryFlag.texture = s; });

        for (int i = 0; i < banks.Count; i++)
        {
            Destroy(banks[i].gameObject);
        }

        banks.Clear();
        //初始化 银行
        if (withdrawalInfo.paymentProviders.Count > 0)
        {
            btnWithdrawal.gameObject.SetActive(true);
            for (int i = 0; i < withdrawalInfo.paymentProviders.Count; i++)
            {
                UIWithdrawalBankFlag withdrawalBankFlag = Instantiate(WithdrawalBankFlag, bankRoot.transform);
                withdrawalBankFlag.SetData(withdrawalInfo.paymentProviders[i]);
                withdrawalBankFlag.onClick += OnBankSelect;
                banks.Add(withdrawalBankFlag);
            }

            bankSelectIdx = -1;
            OnBankSelect(banks[0]);
        }
        else
        {
            btnWithdrawal.gameObject.SetActive(false);
        }
        //初始化 选项
    }

    private void OnItemSelect(UIWithdrawalItem item)
    {
        int idx = items.IndexOf(item);
        if (idx == itemSelectIdx) return;
        itemSelectIdx = idx;
        for (int i = 0; i < items.Count; i++)
        {
            if (i == itemSelectIdx)
            {
                items[i].ShowSelection();
            }
            else
            {
                items[i].HideSelection();
            }
        }

        txtRewardAmount.text = $"{item.WithdrawalItem.currencyCode} {item.WithdrawalItem.amount}";
    }

    private void OnBankSelect(UIWithdrawalBankFlag bankFlag)
    {
        int idx = banks.IndexOf(bankFlag);
        if (idx == bankSelectIdx) return;
        bankSelectIdx = idx;
        var data = bankFlag.paymentProvider;
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }

        items.Clear();
        if (data.items.Count > 0)
        {
            for (int i = 0; i < data.items.Count; i++)
            {
                UIWithdrawalItem withdrawalItem = Instantiate(WithdrawalItem, rewardRoot.transform);
                withdrawalItem.Init(data.items[i], OnItemSelect);
                items.Add(withdrawalItem);
            }

            itemSelectIdx = -1;
            OnItemSelect(items[0]);
        }
    }

    private void OnBtnChangeCountryClick()
    {
        // 切换国家
        // TODO 获取国家列表
        
        EventModule.BroadCast(E_GameEvent.ShowMask, true);
        AccountController.Instance.GetChangeCountryList();
    }

    private void OnGetChangeCountryListResponse(AccountController.CountryList countryList)
    {
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
        UIChangeCountry changeCountry = UIController.ShowPage<UIChangeCountry>();
        changeCountry.ShowReginlist(countryList.detail);
    }

    private void OnBtnWithdrawalClick()
    {
        UIWithdrawalConfirm withdrawalConfirm = UIController.ShowPage<UIWithdrawalConfirm>();
        withdrawalConfirm.SetData(banks[bankSelectIdx].paymentProvider, items[itemSelectIdx].WithdrawalItem);
    }
}