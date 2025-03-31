using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWithdrawalConfirm : UIPage
{
    public Transform Root;
    public GameObject ConfirmRoot;
    public GameObject ResultRoot;
    public TMP_Text txtCountryNum, txtDocCountryNum, txtAmount, txtCoinCount, txtGroupLink;

    public TMP_InputField inputTxt,
        inputPhone,
        inputMail,
        inputFullName,
        inputDocumentText,
        InputDocumentPhone,
        InputDocumentEmail;

    public GameObject txtGo, phoneNumGo, emailGo, fullNameGo, docTxtGo, docPhoneGo, docEmailGo, docRoot;
    public TMP_Dropdown docDropDown;
    public Button btnWithdrawal, btnClose, btnGoToLink;


    public void Awake()
    {

        btnWithdrawal.onClick.AddListener(OnBtnWithdrawalClick);
        btnClose.onClick.AddListener(() => { UIController.HidePage(this); });
        btnGoToLink.onClick.AddListener(OnGoToLinkClick);
    }


    public void OnEnable()
    {
        EventModule.AddListener<bool, AccountController.WithdrawalObtain>(E_GameEvent.WithdrawalObtainResponse,
            OnWithdrawalResponse);

        if (string.IsNullOrEmpty(AccountController.Instance.stInfo.groupLink))
        {
            btnGoToLink.gameObject.SetActive(false);
        }
        else
        {
            btnGoToLink.gameObject.SetActive(true);
            txtGroupLink.text = AccountController.Instance.stInfo.groupLink;
        }
    }

    public void OnDisable()
    {
        EventModule.RemoveListener<bool, AccountController.WithdrawalObtain>(E_GameEvent.WithdrawalObtainResponse,
            OnWithdrawalResponse);
    }

    private AccountController.PaymentProvider m_paymentProvider;
    private AccountController.WithdrawalItem m_withdrawalItem;

    public void SetData(AccountController.PaymentProvider paymentProvider,
        AccountController.WithdrawalItem withdrawalItem)
    {
        string countryNum = AccountController.Instance.GetCountryNum();

        m_paymentProvider = paymentProvider;
        m_withdrawalItem = withdrawalItem;

        ConfirmRoot.SetActive(true);
        ResultRoot.SetActive(false);

        var param = paymentProvider.inputParams;
        //param
        txtGo.SetActive(false);
        phoneNumGo.SetActive(false);
        emailGo.SetActive(false);

        if (param.accountNo.inputType == "PHONE_NUMBER")
        {
            phoneNumGo.SetActive(true);
            (inputPhone.placeholder as TMP_Text).text = param.accountNo.displayText;
            txtCountryNum.text = "+" + countryNum;
        }
        else if (param.accountNo.inputType == "EMAIL")
        {
            emailGo.SetActive(true);
            (inputMail.placeholder as TMP_Text).text = param.accountNo.displayText;
        }
        else if (param.accountNo.inputType == "NUMBER")
        {
            inputTxt.contentType = TMP_InputField.ContentType.IntegerNumber;
            txtGo.SetActive(true);
            (inputTxt.placeholder as TMP_Text).text = param.accountNo.displayText;
        }
        else
        {
            inputTxt.contentType = TMP_InputField.ContentType.Standard;
            txtGo.SetActive(true);
            (inputTxt.placeholder as TMP_Text).text = param.accountNo.displayText;
        }

        fullNameGo.SetActive(param.fullName.require == "t");
        if (param.document.require == "t")
        {
            docDropDown.ClearOptions();
            for (int i = 0; i < param.document.documentType.Count; i++)
            {
                docDropDown.options.Add(new TMP_Dropdown.OptionData(param.document.documentType[i].showName));
            }

            docDropDown.value = 0;
            docRoot.SetActive(true);
            docTxtGo.SetActive(false);
            docPhoneGo.SetActive(false);
            docEmailGo.SetActive(false);

            if (param.document.documentId.inputType == "PHONE_NUMBER")
            {
                docPhoneGo.SetActive(true);
                (InputDocumentPhone.placeholder as TMP_Text).text = param.document.documentId.displayText;
                txtDocCountryNum.text = "+" + countryNum;
            }
            else if (param.document.documentId.inputType == "EMAIL")
            {
                docEmailGo.SetActive(true);
                (InputDocumentEmail.placeholder as TMP_Text).text = param.document.documentId.displayText;
            }
            else if (param.document.documentId.inputType == "NUMBER")
            {
                inputDocumentText.contentType = TMP_InputField.ContentType.IntegerNumber;
                docTxtGo.SetActive(true);
                (inputDocumentText.placeholder as TMP_Text).text = param.document.documentId.displayText;
            }
            else
            {
                inputDocumentText.contentType = TMP_InputField.ContentType.Standard;
                docTxtGo.SetActive(true);
                (inputDocumentText.placeholder as TMP_Text).text = param.accountNo.displayText;
            }
        }
        else
        {
            docRoot.SetActive(false);
        }

        txtAmount.text = $"{withdrawalItem.currencyCode} {withdrawalItem.amount.ToString()}";
        txtCoinCount.text = $"=     {withdrawalItem.coin.ToString()}";
    }

    private void OnBtnWithdrawalClick()
    {
        var param = m_paymentProvider.inputParams;
        string accountNo;
        if (param.accountNo.inputType == "PHONE_NUMBER")
        {
            accountNo = inputPhone.text;
        }
        else if (param.accountNo.inputType == "EMAIL")
        {
            accountNo = inputMail.text;
        }
        else
        {
            accountNo = inputTxt.text;
        }

        if (string.IsNullOrEmpty(accountNo))
        {
            //TODO
            Debug.LogError("Account is null");
            return;
        }
        string documentId = null;
        string documentType = null;
        bool needDoc = m_paymentProvider.inputParams.document.require == "t";
        if (needDoc)
        {
            documentType = m_paymentProvider.inputParams.document.documentType[docDropDown.value].requestValue;

            if (m_paymentProvider.inputParams.document.documentId.inputType == "PHONE_NUMBER")
            {
                documentId = InputDocumentPhone.text;
            }
            else if (m_paymentProvider.inputParams.document.documentId.inputType == "EMAIL")
            {
                documentId = InputDocumentEmail.text;
            }
            else
            {
                documentId = inputDocumentText.text;
            }
        }

        AccountController.Instance.RequestWithdrawalObtain(false, accountNo, inputFullName.text, documentType,
            documentId, 0, m_paymentProvider.id, m_withdrawalItem.id);
    }

    private void OnWithdrawalResponse(bool success, AccountController.WithdrawalObtain obtain)
    {
        if (success)
        {
            ConfirmRoot.SetActive(false);
            ResultRoot.SetActive(true);
        }
        else
        {
            UIDialog dialog = UIController.ShowPage<UIDialog>();
            dialog.Show("Warning", "withdrawal fail", null);
        }
    }

    private void OnGoToLinkClick()
    {
        Application.OpenURL(AccountController.Instance.stInfo.groupLink);
    }
}