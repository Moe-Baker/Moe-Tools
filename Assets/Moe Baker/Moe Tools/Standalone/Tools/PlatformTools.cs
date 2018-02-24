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
	public static partial class MoeTools
	{
        public static class Platform
        {
            public static GameTargetPlatform Current
            {
                get
                {
                    if (PC.IsCurrent)
                        return GameTargetPlatform.PC;

                    else if (Mobile.IsCurrent)
                        return GameTargetPlatform.Mobile;

                    else if (Console.IsCurrent)
                        return GameTargetPlatform.Console;

                    else if (web.IsCurrent)
                        return GameTargetPlatform.Web;

                    return GameTargetPlatform.Unknown;
                }
            }

            static PCPlatform _PC = new PCPlatform();
            public static PCPlatform PC { get { return _PC; } }
            public class PCPlatform : Data
            {
                public PCPlatform()
                {
                    Platforms = new RuntimePlatform[]
                    {
                        RuntimePlatform.WindowsEditor,
                        RuntimePlatform.WindowsPlayer,

                        RuntimePlatform.OSXEditor,
                        RuntimePlatform.OSXPlayer,

                        RuntimePlatform.LinuxEditor,
                        RuntimePlatform.LinuxPlayer,
                    };
                }
            }

            static MobilePlatform mobile = new MobilePlatform();
            public static MobilePlatform Mobile { get { return mobile; } }
            public class MobilePlatform : Data
            {
                public MobilePlatform()
                {
                    Platforms = new RuntimePlatform[]
                    {
                        RuntimePlatform.Android,
                        RuntimePlatform.IPhonePlayer,

                        RuntimePlatform.TizenPlayer,
                    };
                }
            }

            static ConsolePlatform console = new ConsolePlatform();
            public static ConsolePlatform Console { get { return console; } }
            public class ConsolePlatform : Data
            {
                public ConsolePlatform()
                {
                    Platforms = new RuntimePlatform[]
                    {
                        RuntimePlatform.PS4,
                        RuntimePlatform.XboxOne,
                    };
                }
            }

            static WebPlatform web = new WebPlatform();
            public static WebPlatform Web { get { return web; } }
            public class WebPlatform : Data
            {
                public WebPlatform()
                {
                    Platforms = new RuntimePlatform[]
                    {
                        RuntimePlatform.WebGLPlayer,
                    };
                }
            }

            public static Data GetData(GameTargetPlatform targetPlatform)
            {
                switch (targetPlatform)
                {
                    case GameTargetPlatform.PC:
                        return _PC;

                    case GameTargetPlatform.Mobile:
                        return mobile;

                    case GameTargetPlatform.Console:
                        return console;
                    case GameTargetPlatform.Web:
                        return web;

                    case GameTargetPlatform.Unknown:
                        throw new ArgumentException("No Platform Data Specified For The Unknow Platform");
                }

                throw new ArgumentOutOfRangeException("No Platform Data Is Defined For " + targetPlatform.ToString());
            }
            public static RuntimePlatform[] GetRuntimePlatforms(GameTargetPlatform targetPlatform)
            {
                return GetData(targetPlatform).Platforms;
            }

            public abstract class Data
            {
                public bool IsCurrent { get { return CompareToPlatforms(Application.platform); } }

                public virtual bool CompareToPlatforms(RuntimePlatform runtimePlatform)
                {
                    return Platforms.Contains(runtimePlatform);
                }

                public RuntimePlatform[] Platforms { get; protected set; }
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static MoeTools.Platform.Data GetData(this GameTargetPlatform targetPlatform)
        {
            return MoeTools.Platform.GetData(targetPlatform);
        }
        public static RuntimePlatform[] GetRuntimePlatforms(this GameTargetPlatform targetPlatform)
        {
            return MoeTools.Platform.GetRuntimePlatforms(targetPlatform);
        }
    }

    public enum GameTargetPlatform
    {
        PC, Mobile, Console, Web, Unknown
    }

    public enum MobileTargetPlatform
    {
        Android, IPhone, Tizen
    }

    public enum ConsoleTargetPlatform
    {
        XBOX, PlayStation
    }

    public enum PCTargetPlatform
    {
        Windows, Linux, OSX
    }
}