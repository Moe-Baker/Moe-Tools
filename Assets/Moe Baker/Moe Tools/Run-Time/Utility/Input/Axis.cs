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
    [Serializable]
    public class Axis
    {
        [SerializeField]
        protected string name;
        public string Name { get { return name; } }

        public float RawValue
        {
            get
            {
                if (name == string.Empty) return 0f;

                return Input.GetAxisRaw(name);
            }
        }
        public float Value
        {
            get
            {
                if (name == string.Empty) return 0f;

                return Input.GetAxis(name);
            }
        }

        public Axis(string name)
        {
            this.name = name;
        }
    }

    [Serializable]
    public class DirectionalAxis
    {
        [SerializeField]
        protected Axis x;
        public Axis X { get { return x; } }
        public const string XPrefix = "X";

        [SerializeField]
        protected Axis y;
        public Axis Y { get { return y; } }
        public const string YPrefix = "Y";

        public virtual Vector2 Value
        {
            get
            {
                return new Vector2(x.Value, y.Value);
            }
        }
        public virtual Vector2 RawValue
        {
            get
            {
                return new Vector2(x.RawValue, y.RawValue);
            }
        }

        public static string Format(string axis, string prefix)
        {
            return axis + " " + prefix;
        }

        public DirectionalAxis(string axis) : this(Format(axis, XPrefix), Format(axis, YPrefix))
        {

        }
        public DirectionalAxis(string x, string y)
        {
            this.x = new Axis(x);
            this.y = new Axis(y);
        }
    }
}