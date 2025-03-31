using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventModule
{
    public delegate void UFEventDeliver();

    public delegate void UFEventDeliver<T>(T args1);

    public delegate void UFEventDeliver<T, U>(T args1, U args2);

    public delegate void UFEventDeliver<T, U, V>(T args1, U args2, V args3);

    public delegate void UFEventDeliver<T, U, V, X>(T args1, U args2, V args3, X args4);

    public delegate void UFEventDeliver<T, U, V, X, Y>(T args1, U args2, V args3, X args4, Y args5);

    /// <summary>
    /// 所有事件的集合
    /// </summary>
    private Dictionary<E_GameEvent, Delegate> m_gameEventsDic;

    private static EventModule m_instance;


    private static EventModule Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EventModule();
                m_instance.m_gameEventsDic = new Dictionary<E_GameEvent, Delegate>(new GameEventComparer());
            }

            return m_instance;
        }
    }

    /// <summary>
    /// 广播事件(无参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    public static void BroadCast(E_GameEvent eventType)
    {
        if (!Instance.m_gameEventsDic.TryGetValue(eventType, out Delegate del))
            return;
        UFEventDeliver e_del = del as UFEventDeliver;
        if (e_del != null)
            e_del();
    }

    /// <summary>
    /// 广播事件(1个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    public static void BroadCast<T>(E_GameEvent eventType, T args1)
    {
        if (!Instance.m_gameEventsDic.TryGetValue(eventType, out Delegate del))
            return;
        UFEventDeliver<T> e_del = del as UFEventDeliver<T>;
        if (e_del != null)
            e_del(args1);
    }

    /// <summary>
    /// 广播事件(2个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    public static void BroadCast<T, U>(E_GameEvent eventType, T args1, U args2)
    {
        if (!Instance.m_gameEventsDic.TryGetValue(eventType, out Delegate del))
            return;
        UFEventDeliver<T, U> e_del = del as UFEventDeliver<T, U>;
        if (e_del != null)
            e_del(args1, args2);
    }

    /// <summary>
    /// 广播事件(3个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    public static void BroadCast<T, U, V>(E_GameEvent eventType, T args1, U args2, V args3)
    {
        if (!Instance.m_gameEventsDic.TryGetValue(eventType, out Delegate del))
            return;
        UFEventDeliver<T, U, V> e_del = del as UFEventDeliver<T, U, V>;
        if (e_del != null)
            e_del(args1, args2, args3);
    }

    /// <summary>
    /// 广播事件(4个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    public static void BroadCast<T, U, V, X>(E_GameEvent eventType, T args1, U args2, V args3, X args4)
    {
        if (!Instance.m_gameEventsDic.TryGetValue(eventType, out Delegate del))
            return;
        UFEventDeliver<T, U, V, X> e_del = del as UFEventDeliver<T, U, V, X>;
        if (e_del != null)
            e_del(args1, args2, args3, args4);
    }

    /// <summary>
    /// 广播事件(5个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    public static void BroadCast<T, U, V, X, Y>(E_GameEvent eventType, T args1, U args2, V args3, X args4, Y args5)
    {
        if (!Instance.m_gameEventsDic.TryGetValue(eventType, out Delegate del))
            return;
        UFEventDeliver<T, U, V, X, Y> e_del = del as UFEventDeliver<T, U, V, X, Y>;
        if (e_del != null)
            e_del(args1, args2, args3, args4, args5);
    }

    /// <summary>
    /// 监听事件(无参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="listener">Listener.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static void AddListener(E_GameEvent eventType, UFEventDeliver callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            Instance.m_gameEventsDic.Add(eventType, null);
        }

        Instance.m_gameEventsDic[eventType] = (UFEventDeliver)Instance.m_gameEventsDic[eventType] + callback;
    }

    /// <summary>
    /// 监听事件(1个参数)
    /// </summary>
    public static void AddListener<T>(E_GameEvent eventType, UFEventDeliver<T> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            Instance.m_gameEventsDic.Add(eventType, null);
        }

        Instance.m_gameEventsDic[eventType] = (UFEventDeliver<T>)Instance.m_gameEventsDic[eventType] + callback;
    }

    /// <summary>
    /// 监听事件(2个参数)
    /// </summary>
    public static void AddListener<T, U>(E_GameEvent eventType, UFEventDeliver<T, U> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            Instance.m_gameEventsDic.Add(eventType, null);
        }

        Instance.m_gameEventsDic[eventType] = (UFEventDeliver<T, U>)Instance.m_gameEventsDic[eventType] + callback;
    }

    /// <summary>
    /// 监听事件(3个参数)
    /// </summary>
    public static void AddListener<T, U, V>(E_GameEvent eventType, UFEventDeliver<T, U, V> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            Instance.m_gameEventsDic.Add(eventType, null);
        }

        Instance.m_gameEventsDic[eventType] = (UFEventDeliver<T, U, V>)Instance.m_gameEventsDic[eventType] + callback;
    }

    /// <summary>
    /// 监听事件(4个参数)
    /// </summary>
    public static void AddListener<T, U, V, X>(E_GameEvent eventType, UFEventDeliver<T, U, V, X> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            Instance.m_gameEventsDic.Add(eventType, null);
        }

        Instance.m_gameEventsDic[eventType] =
            (UFEventDeliver<T, U, V, X>)Instance.m_gameEventsDic[eventType] + callback;
    }


    /// <summary>
    /// 监听事件(4个参数)
    /// </summary>
    public static void AddListener<T, U, V, X, Y>(E_GameEvent eventType, UFEventDeliver<T, U, V, X, Y> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            Instance.m_gameEventsDic.Add(eventType, null);
        }

        Instance.m_gameEventsDic[eventType] =
            (UFEventDeliver<T, U, V, X, Y>)Instance.m_gameEventsDic[eventType] + callback;
    }

    /// <summary>
    /// 移除监听(无参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="listener">Listener.</param>
    public static void RemoveListener(E_GameEvent eventType, UFEventDeliver callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            return;
        }

        Instance.m_gameEventsDic[eventType] = (UFEventDeliver)Instance.m_gameEventsDic[eventType] - callback;
    }

    /// <summary>
    /// 移除监听(1个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="listener">Listener.</param>
    public static void RemoveListener<T>(E_GameEvent eventType, UFEventDeliver<T> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            return;
        }

        Instance.m_gameEventsDic[eventType] = (UFEventDeliver<T>)Instance.m_gameEventsDic[eventType] - callback;
    }

    /// <summary>
    /// 移除监听(2个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="listener">Listener.</param>
    public static void RemoveListener<T, U>(E_GameEvent eventType, UFEventDeliver<T, U> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            return;
        }

        Instance.m_gameEventsDic[eventType] = (UFEventDeliver<T, U>)Instance.m_gameEventsDic[eventType] - callback;
    }

    /// <summary>
    /// 移除监听(3个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="listener">Listener.</param>
    public static void RemoveListener<T, U, V>(E_GameEvent eventType, UFEventDeliver<T, U, V> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            return;
        }

        Instance.m_gameEventsDic[eventType] = (UFEventDeliver<T, U, V>)Instance.m_gameEventsDic[eventType] - callback;
    }

    /// <summary>
    /// 移除监听(4个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="listener">Listener.</param>
    public static void RemoveListener<T, U, V, X>(E_GameEvent eventType, UFEventDeliver<T, U, V, X> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            return;
        }

        Instance.m_gameEventsDic[eventType] =
            (UFEventDeliver<T, U, V, X>)Instance.m_gameEventsDic[eventType] - callback;
    }

    /// <summary>
    /// 移除监听(4个参数)
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="listener">Listener.</param>
    public static void RemoveListener<T, U, V, X, Y>(E_GameEvent eventType, UFEventDeliver<T, U, V, X, Y> callback)
    {
        if (!Instance.m_gameEventsDic.ContainsKey(eventType))
        {
            return;
        }

        Instance.m_gameEventsDic[eventType] =
            (UFEventDeliver<T, U, V, X, Y>)Instance.m_gameEventsDic[eventType] - callback;
    }
}