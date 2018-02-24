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
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScaleController : PlatformSpecificDataModifier<CanvasScaleProfile>
	{
        public virtual CanvasScaler GetScaler()
        {
            return GetComponent<CanvasScaler>();
        }

        protected override void InitPlatform(GameTargetPlatform platform, CanvasScaleProfile data)
        {
            base.InitPlatform(platform, data);

            data.Apply(GetScaler());
        }
    }

    [Serializable]
    public abstract class BasePlatformDataController
    {
        [Serializable]
        public class PlatformOverrideValue : OverrideValue<RuntimePlatform>
        {

        }
    }
    [Serializable]
    public abstract class PlatformDataController<TPlatformData, TValue> : BasePlatformDataController
        where TPlatformData : PlatformData<TValue>
    {
        [SerializeField]
        protected TPlatformData[] platforms;
        public TPlatformData[] Platforms { get { return platforms; } }

        [SerializeField]
        protected PlatformOverrideValue overridePlatform;
        public PlatformOverrideValue OverridePlatform { get { return overridePlatform; } }
        
        public RuntimePlatform GetCurrentPlatform()
        {
#if UNITY_EDITOR
            if (overridePlatform.Enabled)
                return overridePlatform.Value;
#endif

            return Application.platform;
        }

        public virtual TValue GetValue()
        {
            return GetValue(GetCurrentPlatform());
        }
        public virtual TValue GetValue(RuntimePlatform platform)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i].ContainsPlatform(platform))
                    return platforms[i].Data;
            }

            throw new ArgumentException("Platform " + platform + " Has No Corresponding Platform Specified In The Platforms Array");
        }

        public PlatformDataController()
        {
            platforms = GetDefaultPlatforms();
        }

        public static TPlatformData[] GetDefaultPlatforms()
        {
            return new TPlatformData[]
            {
                CreatePlatformData(GameTargetPlatform.PC),
                CreatePlatformData(GameTargetPlatform.Mobile),
                CreatePlatformData(GameTargetPlatform.Console)
            };
        }

        public static TPlatformData CreatePlatformData(GameTargetPlatform targetPlatform)
        {
            return CreatePlatformData(targetPlatform, default(TValue));
        }
        public static TPlatformData CreatePlatformData(GameTargetPlatform targetPlatform, TValue value)
        {
            var platform = Activator.CreateInstance<TPlatformData>();
            platform.Configure(targetPlatform, value);

            return platform;
        }
    }

    [Serializable]
    public abstract class BasePlatformData
    {
        
    }
    [Serializable]
    public abstract class PlatformData<TValue> : BasePlatformData
    {
        [SerializeField]
        protected string name;
        public string Name { get { return name; } }

        [SerializeField]
        protected TValue value;
        public TValue Data { get { return value; } }

        [SerializeField]
        protected RuntimePlatform[] platforms;
        public RuntimePlatform[] Platforms { get { return platforms; } }

        public bool IsCurrent()
        {
            return ContainsPlatform(Application.platform);
        }
        public bool ContainsPlatform(RuntimePlatform platform)
        {
            return platforms.Contains(platform);
        }

        public PlatformData()
        {
            
        }

        public virtual void Configure(GameTargetPlatform targetPlatform, TValue value)
        {
            Configure(targetPlatform.ToFriendlyString(), targetPlatform.GetRuntimePlatforms(), value);
        }
        protected virtual void Configure(string name, RuntimePlatform[] platforms, TValue value)
        {
            this.name = name;
            this.platforms = platforms;
            this.value = value;
        }
    }
}