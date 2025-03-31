using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza
{
    public static class GPUInfo
    {
        public enum Performance
        {
            Low,
            Middle,
            High,
        }

        public static Performance performance { get; private set; }

        public static void Refresh()
        {
#if UNITY_EDITOR
            performance = Performance.High;
#elif UNITY_ANDROID
            var table = DataTableModule.Instance.Tables.TblGPUInfo;
            string vendorID = SystemInfo.graphicsDeviceVendorID.ToString("X");
            string deviceID = SystemInfo.graphicsDeviceID.ToString("X");
            string id = string.Concat(vendorID, ":", deviceID);
            bool enable = table.DataMap.ContainsKey(id);
            performance = enable ? Performance.High : Performance.Low;
            Debug.Log("gpu vendor id ::: " + vendorID);
            Debug.Log("gpu device id ::: " + deviceID);
            Debug.Log("gpu info id ::: " + id);
            Debug.Log("enable high quality ::: " + enable);
#else
            performance = Performance.High;
#endif
        }
    }
}

