using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIOrderHistory : UIPage
{
    public Button btnClose;
    public Transform itemRoot;
    public UIOrderItem itemPref;
    private List<UIOrderItem> items = new ();
    public void Awake ()
    {
        btnClose.onClick.AddListener(() =>
        {
            UIController.HidePage(this);
        });
    }

    public void OnEnable()
    {
        EventModule.AddListener<AccountController.WithdrawalRecord>(E_GameEvent.OrderRecordListResponse,
            OnOrderListResponse);
        AccountController.Instance.RequestWithdrawalRecords(0, 999, 0);
    }

    public void OnDisable()
    {
        EventModule.RemoveListener<AccountController.WithdrawalRecord>(E_GameEvent.OrderRecordListResponse,
            OnOrderListResponse);
    }


    public void OnOrderListResponse(AccountController.WithdrawalRecord record)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }

        items.Clear();
        
        for (int i = 0; i < record.record.Count; i++)
        {
            var r = record.record[i];
            UIOrderItem item = Instantiate(itemPref, itemRoot);
            item.txtOrderId.text = r.orderId;
            item.txtOrderData.text = r.createTime;
            item.txtOrderAmount.text = r.amount.ToString();
            item.txtState.text = GetStatusTxt(r.status);
            items.Add(item);
        }
    }

    private string GetStatusTxt(int status)
    {
        switch (status)
        {
            case 0: return "Pending review";
            case 1: return "Review passed";
            case 2: return "Review failed";
            case 3: return "Successful";
            case 4: return "Exception";
            default: return "Unknow:" + status.ToString();
        }
    }
}