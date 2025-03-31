using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    public List<UIPage> UIPages = new List<UIPage>();

    public static UIController Instance;

    public GameObject LoadingMask;

    private void Awake()
    {
        Instance = this;
        UIPages = new List<UIPage>(GetComponentsInChildren<UIPage>(true));

        //脚本激活注册下事件
        LoadingMask.gameObject.SetActive(true);
    }

    private void Start()
    {
        foreach (var item in UIPages)
        {
            item.gameObject.SetActive(false);
        }

        LoadingMask.gameObject.SetActive(false);
    }

    public static T ShowPage<T>() where T : UIPage
    {
        foreach (var item in Instance.UIPages)
        {
            Debug.Log(item.GetType() + " --- " + typeof(T));
            if (item.GetType() == typeof(T))
            {
                item.gameObject.SetActive(true);
                return (T)item;
            }
        }

        return null;
    }

    public static void HidePage(UIPage uipage)
    {
        uipage.gameObject.SetActive(false);
    }

    public static void HidePage<T>() where T : UIPage
    {
        foreach (var item in Instance.UIPages)
        {
            if (item.GetType() == typeof(T))
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
