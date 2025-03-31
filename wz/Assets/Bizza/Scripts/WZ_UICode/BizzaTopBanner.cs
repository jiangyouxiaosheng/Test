using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BizzaTopBanner : MonoBehaviour
{
    public Button btnWithdrawal;
    public TMP_Text txtCoin;
    public UIMarquee marquee;

    public GameObject WithDrawGuide;

    void Start()
    {
        WithDrawGuide.gameObject.SetActive(false);

    }


    private void OnEnable()
    {
        RefreshAccountCoin();
        EventModule.AddListener(E_GameEvent.RefreshCoin, RefreshAccountCoin);
        if (AccountController.Instance.stInfo.earningSwitch == 1)
            marquee.StartMarquee();
    }

    private void OnDisable()
    {
        EventModule.RemoveListener(E_GameEvent.RefreshCoin, RefreshAccountCoin);
        marquee.StopMarquee();
    }


    private void RefreshAccountCoin()
    {
        txtCoin.text = AccountController.Instance.userInfo.udCoin.ToString();

        int guideShow = PlayerPrefs.GetInt("ShowGuideCount", 0);
        if (guideShow == 0 && WithDrawGuide.gameObject != null && AccountController.Instance.userInfo.udCoin > 1000) 
        {
            WithDrawGuide.gameObject.SetActive(true);

            LMotion.Create(0, 1, 15).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
            {
                WithDrawGuide.gameObject.SetActive(false);
            }).RunWithoutBinding();
        }

        PlayerPrefs.SetInt("ShowGuideCount", 1);
    }
}