using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Bizza.Editor
{
    /// <summary>
    /// 编辑器环境下资源工具类
    /// </summary>
    public static class EditorAssetUtil
    {
        public static List<T> FindAssetInFolder<T>(string folderPath, List<T> ret = null) where T : Object
        {
            ret ??= new();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T t = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (t != null) { ret.Add(t); }
            }

            return ret;
        }

        /// <summary>
        /// 获取一个资源的目录
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetAssetPath(Object asset)
        {
            string ret = AssetDatabase.GetAssetPath(asset);
            return AssetDatabase.IsValidFolder(ret) ? ret : Path.GetDirectoryName(ret);
        }
    }
}
#endif

//public static class SoCfgListUtil
//{
//#if UNITY_EDITOR
//    public static void AddByFolder<T>(List<T> list, Object asset) where T : Object
//    {
//        string selectedFolderPath = AssetDatabase.GetAssetPath(asset);
//        if (!AssetDatabase.IsValidFolder(selectedFolderPath))
//        {
//            selectedFolderPath = Path.GetDirectoryName(selectedFolderPath);
//        }
//        var dir = new DirectoryInfo(selectedFolderPath);
//        list.Clear();
//        AddByFolder(selectedFolderPath, dir, list);
//        AssetDatabase.Refresh();

//        Debug.Log($"asset at {selectedFolderPath} add to list");
//    }

//    public static void AddByFolder<T>(string basePath, DirectoryInfo dir, List<T> list) where T : Object
//    {
//        // Check if the source directory exists
//        if (!dir.Exists) { return; }

//        // Get the files in the source directory and copy to the destination directory
//        foreach (FileInfo file in dir.GetFiles())
//        {
//            string path = Path.Combine(basePath, file.Name);
//            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
//            if (asset == null) { continue; }
//            list.Add(asset);
//        }

//        foreach (var subDir in dir.GetDirectories())
//        {
//            string path = Path.Combine(basePath, subDir.Name);
//            AddByFolder(path, subDir, list);
//        }
//#endif
//    }
//}