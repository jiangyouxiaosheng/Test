using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class CountryCode : ScriptableObject
{
    [TableList]
    public List<CountryCodeInfo> countrys = new List<CountryCodeInfo>();

    public string GetCountryNum(string code)
    {
        for (int i = 0; i < countrys.Count; i++)
        {
            return countrys[i].num;
        }
        return "";
    }
}

[Serializable]
public class CountryCodeInfo
{
    public string code;
    public string num;
}
