using CommonNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    public class ControlGobal: MonoBehaviour
    {
        static Transform uiRoot;
        public static string controlRootPath = "UI/Control/";
        private void Awake()
        {
            uiRoot = GameObject.Find("UI/UI Root").transform;
        }
        static public GameObject CreateCtrl(string ctrlName)
        {
            GameObject prefab = MyTools.GetItemPrefab(controlRootPath + ctrlName);
            GameObject go = Instantiate(prefab, uiRoot);
            return go;
        }
    }
}
