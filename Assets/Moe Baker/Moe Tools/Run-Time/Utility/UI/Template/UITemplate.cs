using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    public class UITemplate<TData> : MonoBehaviour
    {
        public TData Data { get; protected set; }

        public virtual void SetData(TData data)
        {
            this.Data = data;

            gameObject.name = ToString();
        }

        public override string ToString()
        {
            return Label.Get(Data);
        }

        public static class Label
        {
            public static string Prefix { get { return "-UI Template"; } }

            public static string Get(TData data)
            {
                if (data == null)
                    return typeof(TData).Name + Prefix;
                else
                    return data.ToString() + Prefix;
            }
        }
    }
}