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
    #region Base
    public interface IBaseMoeModule
    {
        
    }

    public abstract class BaseMoeModule : IBaseMoeModule
    {

    }

    public abstract class BaseMoeModuleManager<TModule>
        where TModule : IBaseMoeModule
    {
        [SerializeField]
        List<TModule> list;
        public List<TModule> List { get { return list; } }

        public virtual void Add(TModule module)
        {
            list.Add(module);
        }

        public virtual void ForAll(Action<TModule> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    throw new NullReferenceException("Module At Index " + i + " Inside A " + GetType().Name + " Is Null");

                action(list[i]);
            }
        }

        public BaseMoeModuleManager()
        {
            list = new List<TModule>();
        }
    }
    #endregion

    #region Basic
    public interface IBasicMoeModule : IBaseMoeModule
    {
        void Init();
    }
    [Serializable]
    public abstract class BasicMoeModule : IBasicMoeModule
    {
        public virtual void Init()
        {

        }
    }

    [Serializable]
    public abstract class BasicMoeModuleManager<TModule> : BaseMoeModuleManager<TModule>
        where TModule : IBasicMoeModule
    {
        public virtual void Init()
        {
            ForAll(InitModule);
        }
        protected virtual void InitModule(TModule module)
        {
            module.Init();
        }
    }
    #endregion

    #region Linked
    public interface IMoeLinkedModule<TLink> : IBaseMoeModule
    {
        void Init(TLink link);
    }
    [Serializable]
    public abstract class MoeLinkedModule<TLink> : BaseMoeModule, IMoeLinkedModule<TLink>
    {
        public TLink Link { get; protected set; }

        public virtual void Init(TLink link)
        {
            this.Link = link;
        }
    }

    [Serializable]
    public abstract class MoeLinkedModuleManager<TModule, TLink> : BaseMoeModuleManager<TModule>
        where TModule : IMoeLinkedModule<TLink>
    {
        public TLink Link { get; protected set; }

        public virtual void Init(TLink link)
        {
            ForAll(InitModule);
        }
        protected virtual void InitModule(TModule module)
        {
            module.Init(Link);
        }

        public virtual T Add<T>(GameObject gameObject)
            where T : TModule
        {
            var module = gameObject.GetComponent<T>();

            if (module == null)
                throw new Exception("No Module Of Type " + typeof(T).Name + " Was Found On Gameobject " + gameObject.name);

            Add(module);

            return module;
        }
    }
    #endregion
}