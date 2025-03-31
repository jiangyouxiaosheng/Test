using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWithdrawalItem : MonoBehaviour
{
    public TMP_Text txtTitle, txtReward, txtRewardTimes, txtCoinCost;
    public Button btnSelect;
    public GameObject selection;


    private System.Action<UIWithdrawalItem> onWithSelect;
    public AccountController.WithdrawalItem WithdrawalItem;
    public void Init(AccountController.WithdrawalItem item,
        System.Action<UIWithdrawalItem> onClick)
    {
        WithdrawalItem = item;
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(OnBtnSelect);
        onWithSelect = onClick;
        txtTitle.text = $"{item.allTimes}/D";
        txtReward.text = $"{item.currencyCode} {item.amount}";
        txtRewardTimes.text = $"{item.remainTimes}/{item.allTimes}";
        txtCoinCost.text = item.coin.ToString();
        
    }

    private void OnBtnSelect()
    {
        onWithSelect?.Invoke(this);
    }

    public void ShowSelection()
    {
        selection.SetActive(true);
    }
    
    public void HideSelection()
    {
        selection.SetActive(false);
    }
}