using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class LogUtil
{
    public static void Assert(bool condition, string text)
    {
        if (!condition)
        {
            Debug.LogError(text);
        }
    }

    [Conditional("DEBUG_MODE")]
    public static void Verbose(string text)
    {
        Debug.Log(text);
    }

    [Conditional("DEBUG_MODE")]
    public static void Verbose(string tag, string text)
    {
        Debug.Log($"[{tag}] {text}");
    }
    
    [Conditional("DEBUG_MODE")]
    public static void Info(string tag, string text)
    {
        Debug.Log($"[{tag}] {text}");
    }
    
    [Conditional("DEBUG_MODE")]
    public static void Info(string text)
    {
        Debug.Log(text);
    }
    
    public static void Error(string text)
    {
        Debug.LogError(text);
    }
}
