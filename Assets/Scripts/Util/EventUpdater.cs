using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CommonNS
{
    public class EventUpdater<T>
    {
        public delegate void UpdaterDelegate();
        Dictionary<T, UpdaterDelegate> updaterDict = new Dictionary<T, UpdaterDelegate>();

        List<T> removeElemList = new List<T>();

        public void Update()
        {
            var varIter = updaterDict.GetEnumerator();
            while (varIter.MoveNext())
            {
                varIter.Current.Value();
            }

            if(removeElemList.Count > 0)
            {
                for (int i = 0; i < removeElemList.Count; i++)
                    updaterDict.Remove(removeElemList[i]);
                removeElemList.Clear();
            }
        }

        public void UnAllReg()
        {
            updaterDict.Clear();
            removeElemList.Clear();
        }

        public void Reg(T key, UpdaterDelegate action)
        {
            if (!updaterDict.ContainsKey(key))
            {
                updaterDict[key] = action;
            }
        }

        public void UnReg(T key)
        {
            removeElemList.Add(key);    
        }
    }
}