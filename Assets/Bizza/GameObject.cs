using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza.Unity
{
    public static class GameObjUitl
    {
        public static void SetObjActive(this GameObject self, bool active)
        {
            if (self.activeSelf != active)
            {
                self.SetActive(active);
            }
        }

        public static void DestroyAllChildren(this Transform self)
        {
            foreach (Transform child in self)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static GameObject FindObj(string path, Transform parent = null)
        {
            if (parent != null)
            {
                var target = parent.Find(path);
                return target != null ? target.gameObject : null;
            }
            return GameObject.Find(path);
        }
    }
}

