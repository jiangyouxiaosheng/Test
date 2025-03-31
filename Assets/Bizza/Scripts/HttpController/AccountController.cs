using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AccountController : MonoBehaviour
{
    private static AccountController m_instance;
    public static AccountController Instance => m_instance;
    [SerializeField]
    private CountryCode CountryCodeAsset;

    #region Server Data

    [HideInInspector] public string accountToken;
    [HideInInspector] public UserInfoData userInfo;

    [HideInInspector] public STInfo stInfo;

    #endregion

    #region Loading Task

    #endregion

    private AccountLoginTask loginTask;
    private AccountSTTask stTask;
    private int retryTimes;

    private void Awake()
    {
        if (!m_instance)
            m_instance = this;
    }

    public static void InitAccountCtrl()
    {
        if (!m_instance)
            m_instance = FindObjectOfType<AccountController>();
        m_instance.loginTask = new AccountLoginTask();
        GameLoading.AddTask(m_instance.loginTask);
        m_instance.stTask = new AccountSTTask();
        GameLoading.AddTask(m_instance.stTask);
    }

    public string GetCountryNum(string countryCode = null)
    {
        if (string.IsNullOrEmpty(countryCode) || countryCode == "DEFAULT")
        {
            countryCode = userInfo.countryCode;
        }

        return CountryCodeAsset.GetCountryNum(countryCode);
    }

    #region Login 登录

    [Serializable]
    public class LoginData : UserInfoData
    {
        public string accessToken;
    }

    public void Login()
    {
        retryTimes = 3;
        HttpController.Instance.RequestToServer<LoginData>("user/login", OnLoginResponse);
    }

    private void OnLoginResponse(bool result, LoginData jsonData, string msg)
    {
        if (result)
        {
            HttpController.Instance.accountToken = jsonData.accessToken;
            userInfo = jsonData;

            loginTask.CompleteTask(LoadingTask.CompleteStatus.Completed);
        }
        else if (retryTimes > 0) //登录重试
        {
            retryTimes--;
            HttpController.Instance.RequestToServer<LoginData>("user/login", OnLoginResponse);
        }
        else
        {
            loginTask.CompleteTask(LoadingTask.CompleteStatus.Failed);
        }
    }

    #endregion

    #region UserInfo 获取用户信息

    [Serializable]
    public class UserInfoData
    {
        public int udCoin;
        public float udAmount;
        public string countryCode;
        public string userNo;
        public string currencyCode;
    }

    public void GetUserInfo()
    {
        HttpController.Instance.RequestToServer<UserInfoData>("user/info", OnGetUserInfoResponse, "GET");
    }

    private void OnGetUserInfoResponse(bool result, UserInfoData jsonData, string msg)
    {
        userInfo = jsonData;
        EventModule.BroadCast(E_GameEvent.RefreshCoin);
    }

    #endregion

    #region GetChangeCountryList 获取国家列表

    [Serializable]
    public class CountryList
    {
        public List<CountryDetail> detail = new();
    }

    [Serializable]
    public class CountryDetail
    {
        public string countryCode;
        public string countryName;
        public string nationalFlagUrl;
        public List<string> paymentProvidersLogoUrl;
    }

    public void GetChangeCountryList()
    {
        HttpController.Instance.RequestToServer<CountryList>("user/changeCountryList", OnGetCountryListResponse, "GET");
    }

    private void OnGetCountryListResponse(bool result, CountryList jsonData, string msg)
    {
        if (result)
        {
            EventModule.BroadCast(E_GameEvent.GetCountryListResponse, jsonData);
        }
    }

    #endregion

    #region ChangeCountry 切换国家

    public void ChangeCountry(string countryCode)
    {
        HttpController.Instance.AddParam("countryCode", countryCode);
        HttpController.Instance.RequestToServer<string>("user/changeCountry", OnChangeCountryResponse);
    }

    private void OnChangeCountryResponse(bool result, string json, string msg)
    {
        EventModule.BroadCast(E_GameEvent.ChangeCountryResponse, result);
        GetUserInfo();
    }

    #endregion

    #region Ads Reward 广告奖励

    [Serializable]
    public struct AdsRewardData
    {
        public float udAmount;

        /// <summary>
        /// 奖励的钱
        /// </summary>
        public float rewardAmount;

        public int udCoin;

        /// <summary>
        /// 奖励的金币
        /// </summary>
        public int rewardCoin;

        public string currencyCode;
        public string doubleSwitch;
    }

    public void GetAdsReward(bool isDouble, float ecpm, string atid, string auid, long adStar, long adEnd,
        long adDuration, string adPlatform, string adFormat, string adNetwork, string userSrc, string lastClickPos,bool isTopon)
    {
        if (isTopon)
        {
            HttpController.Instance.AddParam("scene", isDouble ? "DOUBLE" : "COMMON");
            HttpController.Instance.AddParam("ecpm", ecpm);
            HttpController.Instance.AddParam("bundle", HttpController.Instance.infoData.bundle);
            HttpController.Instance.AddParam("vc", HttpController.Instance.infoData.vc.ToString());
            HttpController.Instance.AddParam("atid", atid);
            HttpController.Instance.AddParam("ts", HttpController.Instance.infoData.ts.ToString());
            HttpController.Instance.AddParam("ouid", HttpController.Instance.infoData.ouid);
            HttpController.Instance.AddParam("idfa", HttpController.Instance.infoData.ifa);
            HttpController.Instance.AddParam("auid", auid);
            HttpController.Instance.AddParam("pbTs", adStar);
            HttpController.Instance.AddParam("peTs", adEnd);
            HttpController.Instance.AddParam("pDur", adDuration);
            HttpController.Instance.AddParam("adNetwork", adNetwork);

// 构建所有参数的字符串
            string allParams =
                $"ecpm: {ecpm}, " +
                $"bundle: {HttpController.Instance.infoData.bundle}, " +
                $"vc: {HttpController.Instance.infoData.vc}, " +
                $"atid: {atid}, " +
                $"ts: {HttpController.Instance.infoData.ts}, " +
                $"ouid: {HttpController.Instance.infoData.ouid}, " +
                $"idfa: {HttpController.Instance.infoData.ifa}, " +
                $"auid: {auid}, " +
                $"pbTs: {adStar}, " +
                $"peTs: {adEnd}, " +
                $"pDur: {adDuration}, " +
                $"adNetwork: {adNetwork}";

// 打印所有参数
            Debug.Log(allParams);
        }
        else
        {
            Debug.Log("获取max的广告奖励");
            HttpController.Instance.AddParam("scene", isDouble ? "DOUBLE" : "COMMON");
            HttpController.Instance.AddParam("ecpm", ecpm);
            HttpController.Instance.AddParam("atid", atid);
            HttpController.Instance.AddParam("auid", auid);
            HttpController.Instance.AddParam("pbTs", adStar);
            HttpController.Instance.AddParam("peTs", adEnd);
            HttpController.Instance.AddParam("pDur", adDuration);
            HttpController.Instance.AddParam("adPlatform", adPlatform);
            HttpController.Instance.AddParam("adFormat", adFormat);
            HttpController.Instance.AddParam("adNetwork", adNetwork);
            HttpController.Instance.AddParam("userSrc", userSrc);
            HttpController.Instance.AddParam("LastC", lastClickPos);
        }
        Debug.Log("获取广告奖励数据");
        
        HttpController.Instance.RequestToServer<AdsRewardData>("reward/imp", OnGetAdsRewardResponse);
    }

    private void OnGetAdsRewardResponse(bool result, AdsRewardData json, string msg)
    {
        Debug.Log("获取广告奖励" +result);
        if (result)
        {
            userInfo.udCoin += json.rewardCoin;
            userInfo.udAmount = json.udAmount;
            Bizza.Analytics.Manager.AddParam("Coins", json.rewardCoin);
            Bizza.Analytics.Manager.AddCommonParams();
            Bizza.Analytics.Manager.SendCustomEvent("Coin");
            
            EventModule.BroadCast(E_GameEvent.GetAdsRewardFromServerResponse, json);
            EventModule.BroadCast(E_GameEvent.RefreshCoin);
        }
        else
        {
            Debug.Log("获取奖励失败，导致加载界面一直存在");
            EventModule.BroadCast(E_GameEvent.ShowMask, false);
            UIDialog dialog = UIController.ShowPage<UIDialog>();
            Debug.Log("获取广告奖励失败");
            dialog.Show("Error", "Ad is loading, please try again later.", null);
        }

        RequestST("REWARD");
    }

    #endregion

    #region Ads 推量
 #region 带量功能

[Serializable]
public class ProductReferralResponse
{
    public List<ProductInfo> products;
}

[Serializable]
public class ProductInfo
{
    public string bundle;
    public string title;
    public string iconUrl;
    public string downloadUrl;
    public string desc;
    [NonSerialized] 
    public Texture cachedTexture; // 缓存下载的纹理
}

public void GetProductReferral(Action<bool, List<ProductInfo>> callback)
{
    StartCoroutine(GetProductReferralCoroutine(callback));
}

private IEnumerator GetProductReferralCoroutine(Action<bool, List<ProductInfo>> callback)
{
    bool requestCompleted = false;
    List<ProductInfo> resultProducts = null;
    
    // 发送请求
    HttpController.Instance.RequestToServer<ProductReferralResponse>(
        "common/productReferral/get",
        (success, response, msg) => {
            if (success && response?.products != null)
            {
                resultProducts = response.products;
                Debug.Log($"获取到{resultProducts.Count}个带量产品");
            }
            else
            {
                Debug.LogError($"带量请求失败: {msg}");
                resultProducts = new List<ProductInfo>();
            }
            requestCompleted = true;
        },
        "GET"
    );

    // 等待请求完成
    yield return new WaitUntil(() => requestCompleted);
    
    if (resultProducts == null || resultProducts.Count == 0)
    {
        callback?.Invoke(false, null);
        yield break;
    }

    // 预加载图片
    yield return StartCoroutine(PreloadAllProductImages(resultProducts));
    
    callback?.Invoke(true, resultProducts);
}

private IEnumerator PreloadAllProductImages(List<ProductInfo> products)
{
    List<Coroutine> downloadCoroutines = new List<Coroutine>();
    
    foreach (var product in products)
    {
        if (!string.IsNullOrEmpty(product.iconUrl))
        {
            var coroutine = StartCoroutine(DownloadProductImage(product));
            downloadCoroutines.Add(coroutine);
        }
    }
    
    // 等待所有下载完成
    foreach (var coroutine in downloadCoroutines)
    {
        yield return coroutine;
    }
}

private IEnumerator DownloadProductImage(ProductInfo product, int maxRetry = 2)
{
    int retryCount = 0;
    bool downloadSuccess = false;
    
    while (retryCount < maxRetry && !downloadSuccess)
    {
        bool downloadCompleted = false;
        Texture downloadedTexture = null;
        
       HttpController.Instance.RequestDownloadRawImage(product.iconUrl, texture => {
            downloadedTexture = texture;
            downloadCompleted = true;
        });
        
        // 等待下载完成或超时（15秒）
        float timeout = 15f;
        float timer = 0;
        while (!downloadCompleted && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (downloadCompleted && downloadedTexture != null)
        {
            product.cachedTexture = downloadedTexture;
            downloadSuccess = true;
            Debug.Log($"成功下载：{product.title} 的图标");
        }
        else
        {
            retryCount++;
            Debug.Log($"图标下载失败 ({product.title})，第 {retryCount} 次重试");
        }
    }
    
    if (!downloadSuccess)
    {
        Debug.Log($"无法下载产品图标：{product.title}");
    }
}


#endregion

    
    
    #endregion
    #region Withdrawal Info 获取提现信息

    [Serializable]
    public class WithdrawalInfo
    {
        public float udAmount;
        public float udAvailableAmount;
        public float udPendingAmount;
        public int udCoin;
        public string countryCode;
        public string currencyCode;
        public string nationalFlagUrl;
        public List<PaymentProvider> paymentProviders;
    }

    [Serializable]
    public class PaymentProvider
    {
        public string id;
        public string name;
        public string logoUrl;
        public string accountNoType;
        public ProviderInputParams inputParams;
        public List<WithdrawalItem> items;
    }

    [Serializable]
    public class ProviderInputParams
    {
        public ProviderAccountNo accountNo;
        public ProviderFullName fullName;
        public ProviderDocument document;
    }

    [Serializable]
    public class ProviderAccountNo
    {
        public string inputType;
        public string displayText;
    }

    [Serializable]
    public class ProviderFullName
    {
        public string require;
        public string displayText;
    }

    [Serializable]
    public class ProviderDocument
    {
        public string require;
        public List<ProviderDocumentType> documentType;
        public ProviderDocumentId documentId;
    }

    [Serializable]
    public class ProviderDocumentType
    {
        public string showName;
        public string requestValue;
    }

    [Serializable]
    public class ProviderDocumentId
    {
        public string inputType;
        public string displayText;
    }

    [Serializable]
    public class WithdrawalItem
    {
        public string id;
        public float amount;
        public int coin;
        public string currencyCode;
        public int remainTimes;
        public int allTimes;
        public float commissionAmount;
        public int commissionCoin;
        public int commissionSwitch;
        public int type;
    }

    public void GetWithdrawalInfo()
    {
        HttpController.Instance.RequestToServer<WithdrawalInfo>("withdrawal/info", OnWithdrawalInfoResponse, "GET");
    }

    private void OnWithdrawalInfoResponse(bool result, WithdrawalInfo json, string msg)
    {
        if (result)
        {
            EventModule.BroadCast(E_GameEvent.GetWithdrawalInfoResponse, json);
        }
        else
        {
            EventModule.BroadCast<WithdrawalInfo>(E_GameEvent.GetWithdrawalInfoResponse, null);
        }
    }

    #endregion

    #region Withdrawal obtain 提现

    [Serializable]
    public class WithdrawalObtain
    {
        public string orderId;
        public int deductionCoin;
        public float reduceAmount;
        public int udCoin;
        public float udAmount;
        public float deductionAmount;
        public int reduceCoin;
        public float udAvailableAmount;
        public float udPendingAmount;
        public string currencyCode;
        public int status;
        public float commissionAmount;
        public int commissionCoin;

        //"orderid": 1888904239574900000
        //"deductionCoin": 60946,
        //"reduceAmount": -1000,
        //"udCoin": 163924,
        //"udAmount": 2689.64,
        //"deductionAmount": 1000,
        //"reduceCoin": -60946,
        //"udAvailableAmount": 2689.64,
        //"udPendingAmount": 3645.79,
        //"currencyCode": "IDR",
        //"status": 6,
        //"commissionAmount": 45.0,
        //"commissionCoin": 2742
    }

    public void RequestWithdrawalObtain(bool isCustom, string accountNo, string fullName, string documentType,
        string documentId, int customWithdrawalAmount, string paymentProviderId, string paymentProviderItemId)
    {
        HttpController.Instance.AddParam("withdrawalType", isCustom ? 2 : 1);
        HttpController.Instance.AddParam("accountNo", accountNo);
        HttpController.Instance.AddParam("fullName", fullName);
        HttpController.Instance.AddParam("documentType", documentType);
        HttpController.Instance.AddParam("documentId", documentId);
        HttpController.Instance.AddParam("customWithdrawalAmount", customWithdrawalAmount);
        HttpController.Instance.AddParam("paymentProviderId", paymentProviderId);
        HttpController.Instance.AddParam("paymentProviderItemId", paymentProviderItemId);
        HttpController.Instance.RequestToServer<WithdrawalObtain>("withdrawal/obtain", OnWithdrawalObtainResponse);
    }

    public void OnWithdrawalObtainResponse(bool success, WithdrawalObtain obtain, string msg)
    {
        Debug.Log("OnWithdrawalObtainResponse ： "+ obtain.deductionAmount);
        EventModule.BroadCast(E_GameEvent.WithdrawalObtainResponse, success, obtain);
        Bizza.Analytics.Manager.AddParam("cashposition", 0);
        Bizza.Analytics.Manager.AddParam("cashrevenue", obtain.deductionAmount);
        Bizza.Analytics.Manager.AddParam("currencycode", obtain.currencyCode);
        Bizza.Analytics.Manager.AddParam("cashtrue", success);
        Bizza.Analytics.Manager.AddCommonParams();
        Bizza.Analytics.Manager.SendCustomEvent("Cash");
        GetUserInfo();
    }

    #endregion

    #region Withdrawal Order Query 提现订单查询

    [Serializable]
    public struct OrderInfo
    {
        public string orderId;
        public int status;
        public int coin;
        public string currencyCode;

        /// <summary>
        /// 手续费开关
        /// </summary>
        public int commissionSwitch;

        /// <summary>
        /// 手续费
        /// </summary>
        public float commissionAmount;

        /// <summary>
        /// 手续金币
        /// </summary>
        public int commissionCoin;

        public string countryCode;
        public int amount;
    }

    public void RequestWithdrawalOrderQuery(string orderId)
    {
        HttpController.Instance.AddParam("orderId", orderId);
        HttpController.Instance.RequestToServer<OrderInfo>("", OnWithdrawalQrderQueryResponse);
    }

    private void OnWithdrawalQrderQueryResponse(bool result, OrderInfo orderInfo, string msg)
    {
    }

    #endregion

    #region Withdrawal Records 提现记录

    [Serializable]
    public struct OrderRecordInfo
    {
        public float amount;
        public int coin;
        public int status;
        public string createTime;
        public string orderId;
        public string currencyCode;

        /// <summary>
        /// 手续费开关
        /// </summary>
        public int commissionSwitch;

        /// <summary>
        /// 手续费
        /// </summary>
        public float commissionAmount;

        /// <summary>
        /// 手续金币
        /// </summary>
        public int commissionCoin;

        public string countryCode;
    }

    [Serializable]
    public class WithdrawalRecord
    {
        public List<OrderRecordInfo> record = new();
    }

    public void RequestWithdrawalRecords(int pageNum, int pageSize, int status)
    {
        HttpController.Instance.AddParam("pageNum", pageNum);
        HttpController.Instance.AddParam("pageSize", pageSize);
        HttpController.Instance.AddParam("status", status);
        HttpController.Instance.RequestToServer<WithdrawalRecord>("withdrawal/record", OnWithdrawalRecordsResponse);
    }

    private void OnWithdrawalRecordsResponse(bool result, WithdrawalRecord orderInfo, string msg)
    {
        if (result)
        {
            EventModule.BroadCast(E_GameEvent.OrderRecordListResponse, orderInfo);
        }
    }

    #endregion

    #region ST 策略信息

    [Serializable]
    public class STInfo
    {
        /// <summary>
        /// ⽹赚功能 0关，1开，2: 等待后再次请求。其他：联系RD确认
        /// </summary>
        public int earningSwitch;
        /// <summary>
        /// directShowAdSwitch 是否有直接观看⼴告的⼊⼝
        /// 0关，1开
        /// </summary>
        public int dsaSw;
        public string interactiveAdUrl;
        public string interactiveAdNspc;
        public string groupLink;
        public float rrRatio;
        public AppUpgrade appUpgrade;
        public adConf adConf;
    }

    [Serializable]
    public class AppUpgrade
    {
        public string level;
        public string tips;
    }

    [Serializable]
    public class ContactUs
    {
        public string btnText;
        public string text;
        public string tg;
        public string email;
    }


    [Serializable]
    public class adConf
    {
        public MaxAd MAX;

        /// <summary>
        /// 图标
        /// </summary>
        public List<string> IRONSOURCE = new List<string>();

        public int rvLoadWaitTime;
        public int interLoadWaitTime;
        public int accInterval;
        public string maxAppKey;
        public string ironSourceAppKey;
        public string topOnAppKey;
        public string topOnAppId;
        public string bigoAppId;
        public string kwaiAppId;


        public adConfInfo MAX_ID;
        public adConfInfo TOPON_ID;
        public adConfInfo BIGO_ID;
        public adConfInfo KWAI_ID;

        //public Block blockAll;
        //public Block blockReward;

        //public BlockAdNetwork blockAdNetwork;
        //public BlockAdSource blockAdSource;
        //public string[] stopSubAdSrc;
    }

    [Serializable]
    public class MaxAd
    {
        public string BANNER;
        public string REWARD_VIDEO;
        public string INTERSTITIAL;
    }

    [Serializable]
    public class BidFactor
    {
        public string REWARDED_VIDEO;
        public string INTERSTITIAL;
    }

    [Serializable]
    public class adConfInfo
    {
        public string[] BANNER;
        public string[] REWARDED_VIDEO;
        public string[] INTERSTITIAL;
        public string[] SPLASH;
    }

    [Serializable]
    public class Block
    {
        public string[] MAX_ID;
        public string[] IRONSOURCE;
        public string[] TOPON_ID;
    }

    [Serializable]
    public class BlockAdNetwork
    {
        public string[] TOPON;
    }

    [Serializable]
    public class BlockAdSource
    {
        public string[] TOPONAdSourceIds;
    }

    private string curScene;

    public void RequestST(string scene)
    {
        curScene = scene;
        retryTimes = 3;
        HttpController.Instance.AddParam("scene", curScene);
        HttpController.Instance.RequestToServer<STInfo>("st", OnSTResponse);
    }

    private void OnSTResponse(bool result, STInfo stInfo, string msg)
    {
        
        if (result)
        {
            this.stInfo = stInfo;
            ////验证网赚模式开关
            //this.stInfo.earningSwitch = 0;
            if (!stTask.IsFinished)
            {
                stTask.CompleteTask(LoadingTask.CompleteStatus.Completed);

                #region Bizza初始化

                //获取服务器参数 & 登陆后初始化 & 获取策略信息后调用
                if (!Bizza.Channel.Platform.isInit)
                {
                    Bizza.Channel.Platform.InitPlatform();
                    Bizza.Channel.ThinkingDataLoader.ManualLoad();
                }
                

                #endregion
            }

            EventModule.BroadCast(E_GameEvent.STRefreshResponse);
        }
        else if (retryTimes > 0) //登录重试
        {
            retryTimes--;
            HttpController.Instance.AddParam("scene", curScene);
            HttpController.Instance.RequestToServer<STInfo>("st", OnSTResponse);
        }
        else
        {
            loginTask.CompleteTask(LoadingTask.CompleteStatus.Failed);
        }
    }

    #endregion
}

public class AccountLoginTask : LoadingTask
{
    public override void OnTaskActivated()
    {
        AccountController.Instance.Login();
    }
}

public class AccountSTTask : LoadingTask
{
    public override void OnTaskActivated()
    {
        AccountController.Instance.RequestST("START");
    }
}

public abstract class LoadingTask
{
    public bool IsActive { get; private set; }
    public bool IsFinished { get; private set; }

    public CompleteStatus Status { get; private set; }

    public event Action<CompleteStatus> OnTaskCompleted;

    public LoadingTask()
    {
        IsActive = false;
        IsFinished = false;
    }

    public void CompleteTask(CompleteStatus status)
    {
        if (IsFinished) return;

        Status = status;
        IsFinished = true;
        OnTaskCompleted?.Invoke(status);
    }

    public void Activate()
    {
        IsActive = true;

        try
        {
            OnTaskActivated();
        }
        catch
        {
            CompleteTask(CompleteStatus.Failed);
        }
    }

    public abstract void OnTaskActivated();

    public enum CompleteStatus { Skipped, Completed, Failed }
}