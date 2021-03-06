﻿using CommonNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    public class ControlGobal : MonoBehaviour
    {
        static Transform uiRoot;
        public static Camera uiRootCam;
        public static string controlRootPath = "Control/";
        private void Awake()
        {
            uiRoot = GameObject.Find("UI Root").transform;
            uiRootCam = uiRoot.Find("Camera").GetComponent<Camera>();
        }
        static public GameObject CreateCtrl(string ctrlName)
        {
            GameObject prefab = Resources.Load<GameObject>(controlRootPath + ctrlName);
            GameObject go = Instantiate(prefab, uiRoot);
            return go;
        }
    }
}
