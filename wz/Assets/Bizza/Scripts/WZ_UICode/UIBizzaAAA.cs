using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIBizzaAAA : UIPage
{
    [SerializeField] private BizzaTopBanner _bizzaTopBanner;
    [SerializeField] private Button btnTreature, btnFlowTreature;
    [SerializeField] private float flowSpeed;
    [SerializeField] private Bounds flowArea;
    [SerializeField] private float flowDelay;
    [SerializeField] private float flowShowTime = 60.0f;
    [SerializeField] private Vector2 flowCD;

    //private UIScaleAnimation levelTextScaleAnimation;
    private Coroutine flowCoroutine;
    private Vector3 flowDir = new Vector3(1, 1, 0);

    private float flowCDTimer = -1;

    private bool adsEnable = false;
    
    public void Awake()
    {
        //levelTextScaleAnimation = new UIScaleAnimation(transform);
        btnTreature.onClick.AddListener(OnBtnTreatureClick);
        btnFlowTreature.onClick.AddListener(OnBtnFlowTreatureClick);
        _bizzaTopBanner.btnWithdrawal.onClick.AddListener(OnBtnWithdrawalClick);

    }

    public void OnEnable()
    {
        //levelTextScaleAnimation.Show(scaleMultiplier: 1.05f, immediately: true);
        adsEnable = AccountController.Instance.stInfo.dsaSw == 1;
        flowDir.Normalize();
        flowCDTimer = Time.time + flowDelay;
        _bizzaTopBanner.gameObject.SetActive(true);
        btnTreature.gameObject.SetActive(adsEnable);
        btnFlowTreature.gameObject.SetActive(false);
        EventModule.AddListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse,
            OnGetWithdrawalInfoResponse);
        
    }

    public void OnDisable()
    {
        //levelTextScaleAnimation.Hide(scaleMultiplier: 1.05f, immediately: false);

        _bizzaTopBanner.gameObject.SetActive(false);
        if (flowCoroutine != null)
            StopCoroutine(flowCoroutine);
        EventModule.RemoveListener<AccountController.WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse,
            OnGetWithdrawalInfoResponse);
    }

    private void Update()
    {
        WaitFlowTreature();
        MoveFlowTreature();
    }

    private void WaitFlowTreature()
    {
        if (!adsEnable) return;
        if (btnFlowTreature.gameObject.activeInHierarchy)
        {
            return;
        }

        if (flowCDTimer < Time.time)
        {
            flowCDTimer = Time.time + flowShowTime;
            btnFlowTreature.gameObject.SetActive(true);
        }
    }

    private void MoveFlowTreature()
    {
        if (!adsEnable) return;
        if (!btnFlowTreature.gameObject.activeInHierarchy)
        {
            return;
        }

        if (!CheckIsInBounds(btnFlowTreature.transform.localPosition, out Vector2 normal))
        {
            flowDir = Vector2.Reflect(flowDir, normal);
            flowDir.Normalize();
        }

        btnFlowTreature.transform.localPosition += flowDir * (flowSpeed * Time.deltaTime);

        if (flowCDTimer < Time.time)
        {
            flowCDTimer = Time.time + Random.Range(flowCD.x, flowCD.y);
            btnFlowTreature.gameObject.SetActive(false);
        }
    }

    private bool CheckIsInBounds(Vector2 pos, out Vector2 normal)
    {
        normal = Vector2.zero;
        Vector3 max = flowArea.max; // + transform.position;
        Vector3 min = flowArea.min; // + transform.position;
        if (pos.x > max.x)
        {
            normal.x = -1;
        }

        if (pos.x < min.x)
        {
            normal.x = 1;
        }

        if (pos.y > max.y)
        {
            normal.y = -1;
        }

        if (pos.y < min.y)
        {
            normal.y = 1;
        }

        normal.Normalize();

        return normal.sqrMagnitude < 0.001f;
    }

    private void OnBtnTreatureClick()
    {
        UIController.ShowPage<UITreatureAds>();
    }

    private void OnBtnFlowTreatureClick()
    {
        UIController.ShowPage<UITreatureAds>();
        btnFlowTreature.gameObject.SetActive(false);
    }
    
    
    private void OnBtnWithdrawalClick()
    {
        EventModule.BroadCast(E_GameEvent.ShowMask, true);
        AccountController.Instance.GetWithdrawalInfo();
    }
    
    private void OnGetWithdrawalInfoResponse(AccountController.WithdrawalInfo withdrawalInfo)
    {
        UIWithdrawal withdrawal = UIController.ShowPage<UIWithdrawal>();
        withdrawal.Init(withdrawalInfo);
        EventModule.BroadCast(E_GameEvent.ShowMask, false);
    }
}