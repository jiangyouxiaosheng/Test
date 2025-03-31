using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : UIPage
{
    public Transform dialogRoot;
    public TMP_Text txtTitle, txtMsg;
    public Button btnOk;


    public void OnEnable()
    {
    }

    public void OnDisable()
    {
    }

    public void Show(string title, string msg, System.Action onBtnClick)
    {
        txtTitle.text = title;
        txtMsg.text = msg;
        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(() =>
        {
            UIController.HidePage(this);
            onBtnClick?.Invoke();
        });
    }
}