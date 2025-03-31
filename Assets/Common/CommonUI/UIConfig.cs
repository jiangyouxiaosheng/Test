using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using OPS.Obfuscator.Attribute;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
[DoNotRename]
[CreateAssetMenu(menuName = "BizzaGame/UIConfig")]
public class UIConfig : ScriptableObject
{
    [FolderPath]
    public string uiPageAssetFolderPath = string.Empty;

    [SerializeField, TableList]
    private List<UIConfigInfo> m_configs = new List<UIConfigInfo>();

    [Space, SerializeField, TableList]
    private List<UILayerSetting> m_layerDef = new List<UILayerSetting>();

    public List<UIConfigInfo> Configs => m_configs;

    public List<UILayerSetting> LayerDef => m_layerDef;

    public Dictionary<EUIPageType, UIConfigInfo> ConfigDic { get; private set; }

    public void Init()
    {
        InitConfigDic();
    }

    private void InitConfigDic()
    {
        if (ConfigDic == null)
            ConfigDic = new Dictionary<EUIPageType, UIConfigInfo>(new UIPageTypeComparer());
        ConfigDic.Clear();
        int infoLength = Configs.Count;
        for (int i = 0; i < infoLength; i++)
        {
            ConfigDic.Add(Configs[i].UIPageType, Configs[i]);
        }
    }

    public int GetLayerByName(string name)
    {
        for (int i = 0; i < LayerDef.Count; i++)
        {
            if (LayerDef[i].LayerName == name)
            {
                return i;
            }
        }
        return -1;
    }

#if UNITY_EDITOR

    public const string UIPageAssetFolderPath = "Assets/GameRes/Prefabs/UIPrefab";
    public const string UIConfigAssetPath = "Assets/UI/UIConfig.asset";

#endif


}

[System.Serializable]
[DoNotRename]
public class UIConfigInfo
{
    [SerializeField, ReadOnly]
    private EUIPageType m_uiPageType;
    [SerializeField, OnValueChanged("OnPageRefrenceChange")]
    private AssetReferenceGameObject m_pageReference;
    [SerializeField, ValueDropdown("GetLayerSelection")]
    private int m_layer;
    [SerializeField]
    private bool m_multiPages = true;
    [SerializeField]
    private bool m_cache = false;
    [SerializeField]
    private bool m_isPage = true;
    [SerializeField]
    private int m_zIndex = 1000;
    
    [DoNotRename]
    public AssetReferenceGameObject PageReference
    {
        get
        {
            return m_pageReference;
        }
        set
        {
            m_pageReference = value;
        }
    }

    public int Layer
    {
        get
        {
            return m_layer;
        }
        set
        {
            m_layer = value;
        }
    }

    public EUIPageType UIPageType
    {
        get
        {
            return m_uiPageType;
        }
        set
        {
            m_uiPageType = value;
        }
    }

    public bool MultiPages => m_multiPages;

    public bool Cache => m_cache;

    public bool IsPage => m_isPage;

    public int ZIndex => m_zIndex;

#if UNITY_EDITOR

    private void OnPageRefrenceChange()
    {
        if (m_pageReference == null || m_pageReference.editorAsset == null)
            m_uiPageType = EUIPageType.None;
        UIPageBase pageBase = m_pageReference.editorAsset.GetComponent<UIPageBase>();
        m_uiPageType = pageBase.PageType;
    }

    private IEnumerable GetLayerSelection()
    {
        UIConfig config = AssetDatabase.LoadAssetAtPath<UIConfig>(UIConfig.UIConfigAssetPath);
        var valueDropList = new ValueDropdownList<int>();
        for (int i = 0; i < config.LayerDef.Count; i++)
        {
            valueDropList.Add(config.LayerDef[i].LayerName, i);
        }
        return valueDropList;
    }

#endif
}

[System.Serializable]
[DoNotRename]
public class UILayerSetting
{
    [SerializeField]
    private string m_layerName;
    [SerializeField]
    private bool m_multiPages = false;
    [SerializeField]
    private bool m_ignoreLowLayerManage = false;

    public string LayerName => m_layerName;
    public bool MultiPages => m_multiPages;

    public bool IgnoreLowLayerManage => m_ignoreLowLayerManage;
}