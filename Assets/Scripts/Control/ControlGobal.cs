using UnityEngine;

namespace ControlNS
{
    public class ControlGobal: MonoBehaviour
    {
        public static Transform uiRoot;
        public static string controlRootPath = "Control/";
        private void Awake()
        {
            uiRoot = GameObject.Find("UI Root").transform;
        }
        static public GameObject CreateCtrl(string ctrlName)
        {

            GameObject prefab = Resources.Load<GameObject>(controlRootPath + ctrlName);
            GameObject go = Instantiate(prefab, uiRoot);
            return go;
        }
    }
}
