using UnityEngine;
using UnityEngine.UI;

public class UIWithdrawalBankFlag : MonoBehaviour
{
    public RawImage imgLogo;

    public Button btnBank;

    public System.Action<UIWithdrawalBankFlag> onClick;

    public AccountController.PaymentProvider paymentProvider;
    // Start is called before the first frame update
    private void Start()
    {
        btnBank.onClick.AddListener(OnBtnBankClick);
    }


    public void SetData(AccountController.PaymentProvider data)
    {
        paymentProvider = data;
        HttpController.Instance.RequestDownloadRawImage(data.logoUrl, (t) =>
        {
            imgLogo.texture = t;
        });
    }
    
    private void OnBtnBankClick()
    {
        onClick?.Invoke(this);
    }
    
}
