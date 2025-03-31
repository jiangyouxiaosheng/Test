using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using OPS.Obfuscator.Attribute;
using Unity.VisualScripting;

[DoNotRename]
public class HttpController : MonoBehaviour
{
    private static HttpController m_manger;
    public static HttpController Instance => m_manger;

    //https://bridge.havenflare.com
    public string domain = "";
    public string doc_version;
    public string bundle_name;
    
    public string aes_key = "311575c8f62aaf11c0fa41fe7c5d9ae4";
    public string salt = "ZjTinpENsk_WOhGZ";
    private long serverTime = 0;
    [HideInInspector] public string accountToken;

    private JsonSerializerSettings jsonSetting;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        m_manger = this;
        InitUrlInfo();
        jsonSetting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };
        IOSPluginAdapter.RequestToIDFA();
        Invoke(nameof(GetIDFAStringAsync), 1.5f);
    }

    private Dictionary<string, object> requestParams = new();

    public void AddParam(string key, object data)
    {
        requestParams[key] = data;
    }


    [DoNotRename]
    private void GetIDFAStringAsync()
    {
        IOSPluginAdapter.GetIDFAString((r, idfa) =>
        {
            infoData.ifa = idfa;
        });
    }

    #region Http Request

    //!!! T必须加[Serializable]标签 否则代码混淆插件会改变JSON的字段名导致反序列化异常
    public void RequestToServer<T>(string path, Action<bool, T, string> response, string method = "POST")
    {
        byte[] bodyRaw = null;
        if (requestParams.Count > 0)
        {
            string jsonStr = SerializeRequestParams();
            requestParams.Clear();
            Debug.Log(jsonStr);
            string bodyEncoding = Base64EncodeWithAes(jsonStr);
            bodyRaw = Encoding.UTF8.GetBytes(bodyEncoding);
        }

        StartCoroutine(RequestToServerCoroutine(path, bodyRaw, response, method));
    }

    private IEnumerator RequestToServerCoroutine<T>(string path, byte[] bodyRaw, Action<bool, T, string> response,
        string method)
    {
        string url = $"{domain.Trim('/')}/{path.Trim('/')}?{UrlInfo()}";
        Debug.Log("请求服务器: " + url);
        using UnityWebRequest request = new UnityWebRequest(url, method);
        {
            var uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.uploadHandler = uploadHandler;
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            if (!string.IsNullOrEmpty(accountToken))
                request.SetRequestHeader("access_token", accountToken);
            var operation = request.SendWebRequest();
            yield return operation;
            if (request.result == UnityWebRequest.Result.Success)
            {
                var k = Encoding.UTF8.GetBytes(aes_key);
                try
                {
                    Debug.Log("服务器返回数据解析之前的"+request.downloadHandler.text);
                    string jsonData = DecryptAes(request.downloadHandler.text, k);
                    Debug.Log("服务器返回: " + jsonData);
                    HttpResponse<T> httpResponse =
                        JsonConvert.DeserializeObject<HttpResponse<T>>(jsonData, jsonSetting);
                    Debug.Log("服务器解析参数"+httpResponse.code+";"+httpResponse.msg+";"+httpResponse.uid+";"+httpResponse.st+";"+httpResponse.data+";");
                    serverTime = httpResponse.st;
                    if (httpResponse.code == "2000")
                    {
                        LogUtil.Info($"HTTP Request Success");
                        response?.Invoke(true, httpResponse.data, "success");
                        
                    }
                    else
                    {
                        LogUtil.Error($"HTTP Request Error [{httpResponse.code}]:{httpResponse.msg}");
                        response?.Invoke(false, default, httpResponse.msg);
                    }
                }
                catch (Exception e)
                {
                    response?.Invoke(false, default, e.Message);
                }
            }
            else
            {
                UIDialog dialog = UIController.ShowPage<UIDialog>();
                Debug.Log("获取广告奖励失败");
                EventModule.BroadCast(E_GameEvent.ShowMask, false);
                dialog.Show("Error", "Ad is loading, please try again later.", null);
                LogUtil.Error(request.error);
                response?.Invoke(false, default, request.error);
            }
        }
    }


    public void RequestDownloadRawImage(string imgPath, Action<Texture> onDownloadFinish)
    {
        StartCoroutine(DownloadingRawImage(imgPath, onDownloadFinish));
    }

    public IEnumerator DownloadingRawImage(string imgPath, Action<Texture> onDownloadFinish)
    {
        using (UnityWebRequest request = new UnityWebRequest(imgPath))
        {
            DownloadHandlerTexture textDownload = new DownloadHandlerTexture(true);
            request.downloadHandler = textDownload;
            yield return request.SendWebRequest();
            onDownloadFinish?.Invoke(textDownload.texture);
        }
    }

    private string SerializeRequestParams()
    {
        return JsonConvert.SerializeObject(requestParams);
    }

    #endregion

    #region Http Response

    [Serializable]
    public struct HttpResponse<T>
    {
        public string code;
        public string msg;
        public string uid;
        public long st;
        public T data;
    }

    #endregion

    #region URL INFO

    [HideInInspector] public UrlInfoData infoData;

    private void InitUrlInfo()
    {
        infoData = new UrlInfoData();
        //TODO: 初始化基础参数
        infoData.w = UnityEngine.Device.Screen.width;
        infoData.h = UnityEngine.Device.Screen.height;
        infoData.dpi = Mathf.RoundToInt(UnityEngine.Device.Screen.dpi);
        infoData.bundle = bundle_name;
        infoData.vc = Convert.ToInt32(Application.version.Replace(".", ""));
        infoData.vn = Application.version;
#if UNITY_IOS
        infoData.os = "IOS";
        infoData.adb = "false";
        infoData.root = "false";
        infoData.ally = "false";
        infoData.mcc = IOSPluginAdapter.GetCountryCodeFromIOS();
        
        infoData.vpn = IOSPluginAdapter.IsVPNConnected();
        infoData.ue = IOSPluginAdapter.IsCallBackSimul();
        infoData.svm = IOSPluginAdapter.IsCallBackSysVNMatch();
        infoData.jbp = IOSPluginAdapter.IsCallBackIsDJbPath();
        infoData.jbu = IOSPluginAdapter.IsCallBackIsDJbUSche();
        infoData.jbl = IOSPluginAdapter.IsCallBackIsDJbLib();
        infoData.tam = IOSPluginAdapter.IsCallBackIdTamper();
        infoData.inj = IOSPluginAdapter.CheckInjectedLibraries();
        Debug.Log($"测试" + infoData.vpn +"-"+infoData.ue+"-"+infoData.ue+"-"+infoData.svm+"-"+infoData.jbp+"-"+infoData.jbu+"-"+infoData.jbl+"-"+infoData.tam+"-"+infoData.inj );
        
#else
        infoData.os = "Android";
        infoData.adb = "false";
        infoData.root = "false";
        infoData.vpn = "false";
        infoData.ally = "false";
        infoData.mcc = “”;
#endif
        infoData.osv = SystemInfo.operatingSystem;
        infoData.language = "en";
        infoData.brand = SystemInfo.deviceName;
        infoData.model = SystemInfo.deviceModel;
        infoData.ue = 0;
        infoData.idfv = UnityEngine.iOS.Device.vendorIdentifier;
        infoData.tz = TimeZoneInfo.Local.Id;
        infoData.ouid = IOSPluginAdapter.GetOpenUDIDFromiOS();
        Debug.Log(infoData.ouid);
    }

    private string UrlInfo()
    {
        //TODO: 处理url_Info实时数据
        var reachability = Application.internetReachability;
        switch (reachability)
        {
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                infoData.contype = "4G"; break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                infoData.contype = "WIFI"; break;
        }

        infoData.ts = TimeUtil.GetCurrentTime(ETimeUnit.MILLISECONDS);
        infoData.vs = GetVerifySign();
        infoData.st = serverTime;
        //加密返回
        string jsonStr = JsonConvert.SerializeObject(infoData);
        Debug.Log(jsonStr);
        string urlEncoding = UrlSafeBase64EncodeWithAes(jsonStr);
        return $"info={urlEncoding}&bundle={bundle_name}&svn={doc_version}";
    }

    private string GetVerifySign()
    {
        return MD5Encoding(infoData.bundle + doc_version + infoData.vn + infoData.ts + salt);
    }

    [Serializable]
    public class UrlInfoData
    {
        public int w;
        public int h;
        public int dpi;
        public string bundle;
        public int vc;
        public string vn;
        public string language;
        public string os;
        public string osv;
        public string contype;
        public long ts;
        public string brand;
        public string model;
        public string adb;
        public string root;
        public string ally;
        public string mcc;
        public string vs;
        public string referrer;
        public string ifa;
        public long st;
        public string tz;
        public string idfv;
        public string ouid;
        public int vpn;
        public int ue;
        public int svm;
        public int jbp;
        public int jbu;
        public int jbl;
        public int tam;
        public int inj;

    }

    #endregion

    #region Encoding Util

    public static string UrlSafeBase64EncodeWithAes(string jsonData)
    {
        LogUtil.Info(jsonData);
        var k = Encoding.UTF8.GetBytes(Instance.aes_key);
        byte[] aesData = AesEncode(jsonData, k);
        return UrlSafeBase64Encode(aesData);
    }

    public static string Base64EncodeWithAes(string jsonData)
    {
        LogUtil.Info(jsonData);
        var k = Encoding.UTF8.GetBytes(Instance.aes_key);
        byte[] aesData = AesEncode(jsonData, k);
        return Base64Encode(aesData);
    }

    private static string UrlSafeBase64Encode(byte[] data)
    {
        string base64EncodedString = Base64Encode(data).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        return base64EncodedString;
    }

    private static string Base64Encode(byte[] data)
    {
        string base64EncodedString = Convert.ToBase64String(data);
        return base64EncodedString;
    }


    private static byte[] AesEncode(string jsonData, byte[] key)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(jsonData);
        byte[] result;
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.Mode = CipherMode.ECB; // 设置为ECB模式
            aes.Padding = PaddingMode.PKCS7; // 设置填充模式
            ICryptoTransform encryptor = aes.CreateEncryptor();
            result = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
        }

        return result;
    }

    public static string DecryptAes(string ciphertext, byte[] key)
    {
        LogUtil.Info(ciphertext);
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] cipherTextBytes = Convert.FromBase64String(ciphertext);
            byte[] plainTextBytes = decryptor.TransformFinalBlock(cipherTextBytes, 0, cipherTextBytes.Length);
            return Encoding.UTF8.GetString(plainTextBytes);
        }
    }

    private static string MD5Encoding(string text)
    {
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes(text);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("x2"));
        }

        return sb.ToString();
    }

    #endregion
}