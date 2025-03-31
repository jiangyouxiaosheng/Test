using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<Button> m_TabButtons = new List<Button>();
    public List<GameObject> m_splits = new List<GameObject>();
    public List<EUIPageType> m_TabPages = new List<EUIPageType>();

    private List<bool> m_TabState = new List<bool>();

    public bool AllowClose = false;


    public void InitTabs<T>(bool isAllowClose, Action<bool, Button, UIPageBase, T> callBack, List<T> datas)
    {
        AllowClose = isAllowClose;
        if (datas != null && datas.Count != m_TabButtons.Count)
        {
            Debug.LogError("tab 页签初始化异常");
            return;
        }

        for (int i = 0; i < m_TabButtons.Count; i++)
        {
            int curInitTabIndex = i;
            m_TabState.Add(false);

            var tabButton = m_TabButtons[curInitTabIndex];
            tabButton.onClick.AddListener(() =>
            {
                List<bool> tabState = m_TabState;
                List<EUIPageType> tabPages = m_TabPages;
                T customData = datas == null ? default(T) : datas[curInitTabIndex];
                Action<bool, Button, UIPageBase, T> theCallBack = callBack;

                OnTabClick(isAllowClose, curInitTabIndex, theCallBack, tabButton, customData, datas, tabState, tabPages);
            });
        }

    }

    public void SelectTab(int idx)
    {
        m_TabButtons[idx].onClick.Invoke();
    }

    //按钮按下
    public void OnTabClick<T>
    (
        bool isAllowClose, int curInitTabIndex, Action<bool, Button, UIPageBase, T> callBack,
        Button tabButton, T customData, List<T> datas,
        List<bool> tabState, List<EUIPageType> tabPages
    )
    {

        bool originState = tabState[curInitTabIndex];

        //点击下方的关闭
        if (originState && isAllowClose)
        {
            Debug.Log("关闭页签" + this.gameObject.name);

            //关闭页面
            tabState[curInitTabIndex] = false;
            var pageId = tabPages[curInitTabIndex];

            if (callBack != null)
            {
                callBack(false, tabButton, UIModule.Instance.FindOpenedPage(pageId), customData);
            }

            if
            (
                this == null || this.gameObject == null || curInitTabIndex < 0 ||
                tabPages == null || curInitTabIndex >= tabPages.Count
            )
            {
                return;
            }

            Debug.Log("TabGroup 组件关闭页签Index: " + curInitTabIndex + " this:" + this.transform.parent + "tabPagesLeng:" + tabPages.Count);
            UIModule.Instance.ClosePage(tabPages[curInitTabIndex]);


            return;
        }

        //切换
        if (!originState)
        {
            //1 关闭其他页面
            for (int k = 0; k < tabState.Count; k++)
            {
                if (tabState[k] == true)
                {
                    tabState[k] = false;
                    var pageId = m_TabPages[k];

                    T customDataToClose = datas == null ? default(T) : datas[k];
                    if (callBack != null)
                    {
                        callBack(false, m_TabButtons[k], UIModule.Instance.FindOpenedPage(pageId), customDataToClose);
                    }

                    if (this == null || this.gameObject == null)
                    {
                        return;
                    }

                    UIModule.Instance.ClosePage(pageId);
                }
            }

            //2 打开当前页签
            tabState[curInitTabIndex] = true;

            for (int i = 0; i < m_splits.Count; i++)
            {
                if (i == curInitTabIndex || i == curInitTabIndex - 1)
                {
                    if (m_splits[i]) 
                        m_splits[i].SetActive(false);
                }
                else
                {
                    if (m_splits[i])
                        m_splits[i].SetActive(true);
                }
            }


            OpenTabPage(tabPages[curInitTabIndex], tabButton, callBack);
            // todo send event
            //EventModule.BroadCast(E_GameEvent.SelectMainPage, curInitTabIndex);
            const string eventName = Bizza.Channel.Analytics.EventName.SelectMainPage;
            bool unlock = tabPages[curInitTabIndex] != EUIPageType.None;
            var @params = new Dictionary<string, object>()
            {
                {"index",curInitTabIndex.ToString() },
                {"unlock",unlock.ToString() },
            };
            Bizza.Channel.Analytics.Manager.SendCustomEvent(eventName, @params);
            return;
        }
    }



    public async void OpenTabPage<T>(EUIPageType uiPageType, Button tabButton, Action<bool, Button, UIPageBase, T> callBack)
    {
        var task = UIModule.Instance.OpenPage(uiPageType);
        await task;
        if (this == null || this.gameObject == null)
        {
            return;
        }
        if (task.Result.gameObject.activeInHierarchy)
        {
            callBack(true, tabButton, task.Result, default(T));
        }

    }

    public void PageClose()
    {
        for (int i = 0; i < m_TabState.Count; i++)
        {
            m_TabState[i] = false;
        }

        for (int i = 0; i < m_TabButtons.Count; i++)
        {
            m_TabButtons[i].transform.Find("SelectionBg").gameObject.SetActive(false);
        }
    }
}
