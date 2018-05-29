using System;
using System.IO;
using System.Linq;
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
    public class ScriptableObjectResource<TObject> : ScriptableObject
        where TObject : ScriptableObject
    {
        static TObject _instance;
        public static TObject Instance
        {
            get
            {
                if (_instance == null || Application.isPlaying == false)
                    _instance = GetInstance();

                return _instance;
            }
        }
        public static bool HasInstance { get { return Instance != null; } }

        public static TObject GetInstance()
        {
            TObject[] objects = Resources.LoadAll<TObject>("");

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name.ToLower().Contains("override"))
                    return objects[i];
            }

            if (objects.Length > 0)
                return objects.First();
            else
                return null;
        }
    }
}