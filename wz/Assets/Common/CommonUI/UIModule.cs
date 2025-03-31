using Bizza.Library;
using Bizza.Unity;
using System;
using System.Collections.Generic;
using OPS.Obfuscator.Attribute;
using UniRx.Async;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

/// <summary>
/// UI模块
/// 管理UI打开关闭所有内容
/// 1. 打开/关闭
/// 2. UI预加载（结合Addressable）
/// 3. 简单的层级管理
/// </summary>
///
///
[DoNotRename]
public class UIModule : BaseGameModule<UIModule>
{
    public struct Param
    {
        RectTransform parent;
        UIPageBase page;
        UIConfigInfo config;
    }

    [SerializeField] private UIConfig m_uiConfig;
    [SerializeField] private RectTransform m_pageRoot;
    [SerializeField] private UIAnimationConfig m_animationConfig;

    public UIConfig UIConfig => m_uiConfig;

    #region UIManager

    /// <summary>
    /// 已加载
    /// </summary>
    private Dictionary<EUIPageType, UIPageBase> m_uiPagesLoadedDic;
    /// <summary>
    /// 实例化列表
    /// </summary>
    private List<UIPageBase> m_pagesOpened;
    /// <summary>
    /// 层级根节点
    /// </summary>
    private List<RectTransform> m_pageLayerRoots;

    private readonly LinkedList<Param> m_pages = new();

    private int openingCount = 0;

    public bool Opening => openingCount > 0;

    private EUIPageType curUIPageType = EUIPageType.Void;

    /*private EUIPageType InternalGetCurUIPageType()
    {
        for (int i = m_pageLayerRoots.Count - 1; i > -1; i--)
        {
            if (m_pageLayerRoots[i] == null) { continue; }
            foreach (Transform child in m_pageLayerRoots[i])
            {
                if (child.gameObject.activeSelf
                    && child.TryGetComponent<UIPageBase>(out var page)
                    && m_uiConfig.ConfigDic[page.PageType].IsPage)
                {
                    return page.PageType;
                }
            }
        }// 如果都没有返回GameMode的Type
        return World.Current.CurGameMode.PageType;
    }*/

    // 获取当前最上方的界面类型
    /*public EUIPageType CurUIPageType
    {
        get
        {
            if (Opening) { return EUIPageType.None; }
            if (curUIPageType == EUIPageType.Void)
            {
                curUIPageType = InternalGetCurUIPageType();
            }
            return curUIPageType;
        }
    }*/

    private List<UIPageBase> m_pagePool;

    private Canvas m_uiCanvas;

    public Camera UICamera
    {
        get
        {
            return GetUICanvas().GetComponentInChildren<Camera>();
        }
    }

    public override void InitGameModule()
    {
        // ui page
        m_uiPagesLoadedDic = new Dictionary<EUIPageType, UIPageBase>(new UIPageTypeComparer());
        m_pageLayerRoots = new List<RectTransform>();
        m_pagesOpened = new List<UIPageBase>();
        m_pagePool = new List<UIPageBase>();

        // loading
        m_loadingHandlerPool = new Pool<LoadingHandler>(CreateLoadingHandler);
        m_loadingShow = new Dictionary<EUIPageType, LoadingHandler>(new UIPageTypeComparer());
        m_fullScreenHandler = new LoadingHandler();
        m_fullScreenHandler.PoolReset();

        // ui config
        if (m_uiConfig != null)
        {
            m_uiConfig.Init();
            var layerDef = m_uiConfig.LayerDef;
            for (int i = 0; i < m_uiConfig.LayerDef.Count; i++)
            {
                Transform layer = m_pageRoot.Find(layerDef[i].LayerName);
                if (layer != null && layer.TryGetComponent<RectTransform>(out var rtf))
                {// 如果已经有层级节点 则直接使用
                    m_pageLayerRoots.Add(rtf);
                    continue;
                }
                GameObject layerRoot = new GameObject(m_uiConfig.LayerDef[i].LayerName);
                RectTransform rt = layerRoot.AddComponent<RectTransform>();
                rt.SetParent(m_pageRoot);
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.sizeDelta = new Vector2(0, 0);
                rt.SetAsLastSibling();
                m_pageLayerRoots.Add(rt);
            }
        }
    }

    public override void ReleaseGameModule()
    {
        ReleaseAllPage();
    }

    #region Load & Release

    public async UniTask LoadPageAsync(EUIPageType uiPageType, bool autoInstance = false)
    {
        if (!m_uiConfig || !m_uiConfig.ConfigDic.ContainsKey(uiPageType))
        {
            Debug.LogError("UIConfig does not have this page:" + uiPageType.ToString());
            return;
        }

        UIConfigInfo configInfo = m_uiConfig.ConfigDic[uiPageType];
        
        if (configInfo.PageReference.IsValid())
        {
            if (!configInfo.PageReference.IsDone)
            {
                await configInfo.PageReference.OperationHandle.Task;
            }
            // ------ 加载完成
            if (!m_uiPagesLoadedDic.ContainsKey(uiPageType))
            {
                UIPageBase pageBase = (configInfo.PageReference.Asset as GameObject).GetComponent<UIPageBase>();
                m_uiPagesLoadedDic.Add(pageBase.PageType, pageBase);
                if (autoInstance && configInfo.Cache)
                {
                    var pageBaseClone = Instantiate(pageBase, m_pageLayerRoots[configInfo.Layer]);
                    pageBaseClone.gameObject.SetActive(false);
                    m_pagePool.Add(pageBaseClone);
                }
            }
        }
        else
        {
            AsyncOperationHandle<GameObject> asyncOperation = configInfo.PageReference.LoadAssetAsync<GameObject>();
            // ------ 等待加载完成
            await asyncOperation.Task;
            if (asyncOperation.Status != AsyncOperationStatus.Succeeded)
            {
                return;
            }

            if (!m_uiPagesLoadedDic.ContainsKey(uiPageType))
            {
                // ------ 加载完成
                UIPageBase pageBase = asyncOperation.Result.GetComponent<UIPageBase>();
                m_uiPagesLoadedDic.Add(pageBase.PageType, pageBase);
                if (autoInstance && configInfo.Cache)
                {
                    var pageBaseClone = Instantiate(pageBase, m_pageLayerRoots[configInfo.Layer]);
                    pageBaseClone.gameObject.SetActive(false);
                    m_pagePool.Add(pageBaseClone);
                }
            }
        }
    }

    public void ReleasePage(IEnumerable<EUIPageType> pageTypes)
    {
        foreach (EUIPageType pageType in pageTypes)
        {
            ReleasePage(pageType);
        }
    }

    public void ReleasePage(EUIPageType pageType)
    {
        for (int i = m_pagesOpened.Count - 1; i > -1; i--)
        {
            if (m_pagesOpened[i].PageType == pageType)
            {
                Destroy(m_pagesOpened[i].gameObject);
                m_pagesOpened.RemoveAt(i);
            }
        }

        if (m_uiConfig && m_uiConfig.ConfigDic.ContainsKey(pageType))
        {
            m_uiConfig.ConfigDic[pageType].PageReference.ReleaseAsset();
            m_uiPagesLoadedDic.Remove(pageType);
        }

    }

    public void ReleaseAllPage()
    {

    }


    #endregion

    #region Open & Close

    public async UniTask<UIPageBase> OpenPage(EUIPageType uiPageType)
    {
        UIPageBase uiPage = await _OpenPage(uiPageType);
        uiPage.OpenPage_CallByFramework();
        return uiPage;
    }
    public async UniTask<UIPageBase<A>> OpenPage<A>(EUIPageType uiPageType, A a)
    {
        UIPageBase uiPage = await _OpenPage(uiPageType);
        if (uiPage == null || !(uiPage is UIPageBase<A>))
        {
            Debug.Log($"-------------------{typeof(A).FullName}---------------------");
            return null;
        }
        UIPageBase<A> uiPageA = uiPage as UIPageBase<A>;

        try
        {
            uiPageA.OpenPage_CallByFramework(a);
        }
        catch (System.Exception e)
        {

            Debug.LogError("打开界面内部报错: " + e.ToString());
        }

        return uiPageA;
    }
    public async UniTask<UIPageBase<A, B>> OpenPage<A, B>(EUIPageType uiPageType, A a, B b)
    {
        UIPageBase uiPage = await _OpenPage(uiPageType);
        if (uiPage == null || !(uiPage is UIPageBase<A, B>))
        {
            return null;
        }
        UIPageBase<A, B> uiPageA = uiPage as UIPageBase<A, B>;
        uiPageA.OpenPage_CallByFramework(a, b);
        return uiPageA;
    }
    public async UniTask<UIPageBase<A, B, C>> OpenPage<A, B, C>(EUIPageType uiPageType, A a, B b, C c)
    {
        UIPageBase uiPage = await _OpenPage(uiPageType);
        if (uiPage == null || !(uiPage is UIPageBase<A, B, C>))
        {
            return null;
        }
        UIPageBase<A, B, C> uiPageA = uiPage as UIPageBase<A, B, C>;
        uiPageA.OpenPage_CallByFramework(a, b, c);
        return uiPageA;
    }
    public async UniTask<UIPageBase<A, B, C, D>> OpenPage<A, B, C, D>(EUIPageType uiPageType, A a, B b, C c, D d)
    {
        UIPageBase uiPage = await _OpenPage(uiPageType);
        if (uiPage == null || !(uiPage is UIPageBase<A, B, C, D>))
        {
            return null;
        }
        UIPageBase<A, B, C, D> uiPageA = uiPage as UIPageBase<A, B, C, D>;
        uiPageA.OpenPage_CallByFramework(a, b, c, d);
        return uiPageA;
    }
    public async UniTask<UIPageBase<A, B, C, D, E>> OpenPage<A, B, C, D, E>(EUIPageType uiPageType, A a, B b, C c, D d, E e)
    {
        UIPageBase uiPage = await _OpenPage(uiPageType);
        if (uiPage == null || !(uiPage is UIPageBase<A, B, C, D, E>))
        {
            return null;
        }
        UIPageBase<A, B, C, D, E> uiPageA = uiPage as UIPageBase<A, B, C, D, E>;
        uiPageA.OpenPage_CallByFramework(a, b, c, d, e);
        return uiPageA;
    }

    private void InitPage(UIPageBase pageBaseClone, UIConfigInfo configInfo)
    {
        pageBaseClone.RectTransform.anchoredPosition = Vector2.zero;
        pageBaseClone.RectTransform.localScale = Vector3.one;
        if (configInfo.ZIndex >= 1000)
        {
            pageBaseClone.RectTransform.SetAsLastSibling();
        }
        else
        {
            var layer = m_pageLayerRoots[configInfo.Layer];
            bool over = true;
            bool min = true;
            for (int i = layer.childCount - 1; i > -1; i--)
            {
                var child = layer.GetChild(i);
                if (child.TryGetComponent<UIPageBase>(out var childPage))
                {
                    if (childPage == pageBaseClone)
                    {
                        over = false;
                        continue;
                    }
                    var childConfig = m_uiConfig.ConfigDic[childPage.PageType];
                    if (configInfo.ZIndex >= childConfig.ZIndex)
                    {
                        pageBaseClone.RectTransform.SetSiblingIndex(over ? i : i + 1);
                        min = false;
                        break;
                    }
                }
            }
            if (min)
            {
                pageBaseClone.RectTransform.SetAsFirstSibling();
            }
        }

        m_pagesOpened.Add(pageBaseClone);
        pageBaseClone.gameObject.SetActive(true);
        //EventModule.BroadCast(E_GameEvent.OpenPage, pageBaseClone.PageType);
    }

    private async UniTask<UIPageBase> _OpenPage(EUIPageType uiPageType)
    {
        if (!m_uiConfig || !m_uiConfig.ConfigDic.ContainsKey(uiPageType))
        {
            return null;
        }

        curUIPageType = EUIPageType.Void;

        UIConfigInfo configInfo = m_uiConfig.ConfigDic[uiPageType];
        UILayerSetting layerSetting = m_uiConfig.LayerDef[configInfo.Layer];
        if (!layerSetting.MultiPages || !configInfo.MultiPages) // 单例页面，如果已经打开了，就不打了
        {
            for (int i = m_pagesOpened.Count - 1; i > -1; i--)
            {
                var page = m_pagesOpened[i];
                if (page.PageType == uiPageType)
                {
                    if (!m_pagesOpened.Contains(page))
                    {
                        InitPage(page, configInfo);
                    }
                    // 找到相同页面
                    return page;
                }
            }
        }

        openingCount++;

        //看看池里有没有
        UIPageBase pageBaseClone = null;
        for (int i = 0; i < m_pagePool.Count; i++)
        {
            if (m_pagePool[i].PageType == uiPageType)
            {
                pageBaseClone = m_pagePool[i];
                m_pagePool.Remove(pageBaseClone);
                break;
            }
        }

        if (pageBaseClone == null)
        {
            //打开一个UI页面
            if (!m_uiPagesLoadedDic.ContainsKey(uiPageType))
            {
                Debug.Log("打开界面成功" + uiPageType);
                await LoadPageAsync(uiPageType);
                UIPageBase pageBasePrefab = m_uiPagesLoadedDic[uiPageType];
                pageBaseClone = Instantiate(pageBasePrefab, m_pageLayerRoots[configInfo.Layer]);
            }
            else
            {
                Debug.Log("打开界面失败" + uiPageType);
                UIPageBase pageBasePrefab = m_uiPagesLoadedDic[uiPageType];
                pageBaseClone = Instantiate(pageBasePrefab, m_pageLayerRoots[configInfo.Layer]);
            }
        }

    
        //处理多级管理
        if (!layerSetting.MultiPages)
        {
            for (int i = m_pagesOpened.Count - 1; i > -1; i--)
            {
                UIConfigInfo c = m_uiConfig.ConfigDic[m_pagesOpened[i].PageType];
                UILayerSetting s = m_uiConfig.LayerDef[c.Layer];

                if (configInfo.Layer == c.Layer)
                {
                    ClosePage(m_pagesOpened[i]);
                }
                else if (configInfo.Layer < c.Layer && !s.IgnoreLowLayerManage)// 关闭同级
                {
                    ClosePage(m_pagesOpened[i]);
                }
            }
        }

        InitPage(pageBaseClone, configInfo);

        openingCount--;

        return pageBaseClone;
    }

    public void ClosePage(UIPageBase pageBase)
    {
        if (pageBase == null)
        {
            Debug.LogError("试图关闭一个null的UIPage");
            return;
        }
        if (m_pagesOpened.Contains(pageBase))
        {
            m_pagesOpened.Remove(pageBase);
            curUIPageType = EUIPageType.Void;
        }

        pageBase.ClosePage_CallByFramework();

        /*EventModule.BroadCast(E_GameEvent.ClosePage, pageBase.PageType);
        EventModule.BroadCast(E_GameEvent.ScriptEndSuspend, ScriptUtil.GetPageCloseTag(pageBase.PageType));*/
    }

    public void ClosePage(EUIPageType uiPageType, bool onlyLast = false)
    {
        for (int i = m_pagesOpened.Count - 1; i > -1; i--)
        {
            if (m_pagesOpened[i].PageType == uiPageType)
            {
                ClosePage(m_pagesOpened[i]);

                //ClosePage有可能会删除m_pagesOpened中的元素，所以要重新判断一下边界
                i = (int)MathF.Min(i, m_pagesOpened.Count - 1);

                if (onlyLast)
                    break;
            }
        }
    }

    public void CloseLayerAllPage(int layer)
    {
        for (int i = m_pagesOpened.Count - 1; i > -1; i--)
        {
            var page = m_pagesOpened[i];
            var pageType = page.PageType;
            foreach (var config in m_uiConfig.Configs)
            {
                if (config.UIPageType == pageType && config.Layer == layer)
                {
                    ClosePage(page);
                    i = (int)MathF.Min(i, m_pagesOpened.Count - 1);
                    break;
                }
            }
        }
    }

    public void CloseAndDestroyLayerAllPage(int layer)
    {
        List<UIPageBase> temp1 = new();
        for (int i = m_pagesOpened.Count - 1; i > -1; i--)
        {
            var page = m_pagesOpened[i];
            var pageType = page.PageType;
            foreach (var config in m_uiConfig.Configs)
            {
                if (config.UIPageType == pageType && config.Layer == layer)
                {
                    temp1.Add(page);
                    break;
                }
            }
        }
        foreach(var page in temp1)
        {
            ClosePage(page);
        }
        List<int> temp2 = new();
        for (int i = 0; i < m_pagePool.Count; i++)
        {
            var page = m_pagePool[i];
            var pageType = page.PageType;
            foreach (var config in m_uiConfig.Configs)
            {
                if (config.UIPageType == pageType && config.Layer == layer)
                {
                    temp2.Add(i);
                    break;
                }
            }
        }
        m_pagePool.RemoveByIndexs(temp2);
        if (m_pageLayerRoots.TryGetItem(layer, out var root))
        {
            root.DestroyAllChildren();
        }
    }

    public void ClosePage_CallByFramework(UIPageBase uiPage)
    {
        if (!m_uiConfig.ConfigDic.ContainsKey(uiPage.PageType))
            return;
        uiPage.gameObject.SetActive(false);
        if (!m_uiConfig.ConfigDic[uiPage.PageType].Cache)
        {
            Destroy(uiPage.gameObject);
        }
        else
        {
            uiPage.PoolReset();
            m_pagePool.Add(uiPage);
        }
    }
    #endregion

    //查找打开了的界面
    public UIPageBase FindOpenedPage(EUIPageType uiPageType)
    {
        for (int i = m_pagesOpened.Count - 1; i > -1; i--)
        {
            if (m_pagesOpened[i].PageType == uiPageType)
            {
                return m_pagesOpened[i];
            }
        }
        return null;
    }
    //查找刚打开的界面
    public UIPageBase FindLastOpenedPage()
    {
        if (m_pagesOpened.Count > 0)
        {
            return m_pagesOpened[m_pagesOpened.Count - 1];
        }
        return null;
    }
    public Canvas GetUICanvas()
    {
        if (m_uiCanvas == null)
            m_uiCanvas = m_pageRoot.GetComponentInParent<Canvas>();
        return m_uiCanvas;
    }

    public float GetPixelSize(float SceneLenth, bool isVertical = false)
    {
        Vector3 screenPositionA = Camera.main.WorldToScreenPoint(Vector3.zero);
        Vector3 screenPositionB = Camera.main.WorldToScreenPoint(new Vector3(SceneLenth, 0, 0));
        return Vector3.Distance(screenPositionA, screenPositionB);
    }
    #endregion

    public UIAnimationConfig.UIAnimationConfigData GetUIAnimationConfigById(int Id)
    {
        foreach (var config in m_animationConfig.Datas)
        {
            if (config.Id == Id)
            {
                return config;
            }
        }

        return null;
    }

    // public int SubTabIndex = 0;
    // public void OpenMainPageTab(int MainTabIndex, int SubTabIndex = 0)
    // {
    //     if (SubTabIndex != 0)
    //         this.SubTabIndex = SubTabIndex;
    //     EventModule.BroadCast(E_GameEvent.OpenMainPage, MainTabIndex);
    // }
    #region Loading Module

    private Pool<LoadingHandler> m_loadingHandlerPool;

    private Dictionary<EUIPageType, LoadingHandler> m_loadingShow;

    private LoadingHandler m_fullScreenHandler;

    private LoadingHandler CreateLoadingHandler()
    {
        return new LoadingHandler();
    }

    // public async UniTask<LoadingHandler> ShowLoading(EUIPageType loadingType)
    // {
    //     if (loadingType < EUIPageType.Loading_None)
    //         return null;
    //     if (m_loadingShow.ContainsKey(loadingType))//证明之前同类型的已经在显示了。同类型只显示一个，loading不支持Multistage
    //         return m_loadingShow[loadingType];
    //
    //     UILoadingPageBase loadingPage = await OpenPage(loadingType) as UILoadingPageBase;
    //     LoadingHandler handler = m_loadingHandlerPool.Pop();
    //     m_loadingShow.Add(loadingType, handler);
    //     loadingPage.LoadingHandler = handler;
    //     handler.PageType = loadingType;
    //     return handler;
    // }

    public void HideLoading(EUIPageType loadingType)
    {
        if (!m_loadingShow.ContainsKey(loadingType))
            return;
        LoadingHandler handler = m_loadingShow[loadingType];
        ClosePage(loadingType, true);
        m_loadingShow.Remove(loadingType);
    }

    #endregion
}
[DoNotRename]
public class LoadingHandler : IPoolElement
{
    public EUIPageType PageType { get; set; }
    public float Process { get; set; }

    public void PoolReset()
    {
        Process = 0;
        PageType = EUIPageType.None;
    }
}