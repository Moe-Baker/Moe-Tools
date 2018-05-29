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
	public abstract partial class OverrideValue
    {
        [SerializeField]
        protected bool enabled;
        public bool Enabled { get { return enabled; } }

        public OverrideValue() : this(false)
        {

        }
        public OverrideValue(bool enabled)
        {
            this.enabled = enabled;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(OverrideValue), true)]
        public class Drawer : PropertyDrawer
        {
            SerializedProperty enabled;
            public bool EnabledValue { get { return enabled.boolValue; } set { enabled.boolValue = value; } }

            public DisectalPropertyDrawer ValueDrawer { get; protected set; }
            public SerializedProperty Value { get { return ValueDrawer.Property; } }
            public float ValueHeight { get { return ValueDrawer.ChildernHeight; } }

            public const float EndSpace = 10f;

            protected virtual void InitProperty(SerializedProperty property)
            {
                if (enabled == null)
                    enabled = property.FindPropertyRelative("enabled");

                if (ValueDrawer == null || ValueDrawer.Property == null)
                    ValueDrawer = new DisectalPropertyDrawer(property.FindPropertyRelative("value"));
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                InitProperty(property);

                var height = 0f;

                height += EditorGUIUtility.singleLineHeight;

                if (EnabledValue)
                    height += GetValueHeight() + EndSpace;

                return height;
            }
            protected virtual float GetValueHeight()
            {
                if (ValueDrawer.HasChildern)
                    return ValueDrawer.ChildernHeight;

                return EditorGUIUtility.singleLineHeight;
            }

            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                InitProperty(property);

                label.text = property.displayName;

                EnabledValue = EditorGUI.ToggleLeft(GUIArea.ProgressLine(ref rect), label, EnabledValue);

                if (EnabledValue)
                    DrawValue(ref rect, property, label);
            }

            public const float ValuePadding = 20f;
            protected virtual void DrawValue(ref Rect rect, SerializedProperty property, GUIContent label)
            {
                rect.x += ValuePadding;
                rect.width -= ValuePadding;

                if (ValueDrawer.HasChildern)
                    ValueDrawer.DrawChildern(ref rect);
                else
                    ValueDrawer.Draw(ref rect);

                rect.x -= ValuePadding;
                rect.width += ValuePadding;
            }
        }
#endif
    }

    [Serializable]
	public abstract class OverrideValue<TData> : OverrideValue
    {
        [SerializeField]
        protected TData value;
        public TData Value { get { return value; } }

        public virtual TData GetEnabledOrDefault(TData defaultValue)
        {
            if (enabled)
                return value;

            return defaultValue;
        }

        public OverrideValue() : this(false, default(TData))
        {

        }
        public OverrideValue(bool enabled) : this(enabled, default(TData))
        {

        }
        public OverrideValue(TData value) : this(false, value)
        {

        }
        public OverrideValue(bool enabled, TData value) : base(enabled)
        {
            this.value = value;
        }
    }

    #region Sample Values
    [Serializable]
    public class IntOverrideValue : OverrideValue<int>
    {
        public IntOverrideValue(int value) : base(value) { }
        public IntOverrideValue(bool enabled, int value) : base(enabled, value) { }
    }

    [Serializable]
    public class FloatOverrideValue : OverrideValue<float>
    {
        public FloatOverrideValue(float value) : base(value) { }
        public FloatOverrideValue(bool enabled, float value) : base(enabled, value) { }
    }

    [Serializable]
    public class BoolOverrideValue : OverrideValue<bool>
    {
        public BoolOverrideValue(bool value) : base(value) { }
        public BoolOverrideValue(bool enabled, bool value) : base(enabled, value) { }
    }

    [Serializable]
    public class StringOverrideValue : OverrideValue<string>
    {
        public StringOverrideValue(string value) : base(value) { }
        public StringOverrideValue(bool enabled, string value) : base(enabled, value) { }
    }

    [Serializable]
    public class PlatformOverrideValue : OverrideValue<GameTargetPlatform>
    {

    }
    #endregion

#if UNITY_EDITOR
    [Serializable]
    public class DisectalPropertyDrawer
    {
        public SerializedProperty Property { get; protected set; }
        public string Label { get { return Property.displayName; } }
        public bool PropertyExpanded { get { return Property.isExpanded; } set { Property.isExpanded = value; } }
        public float PropertyHeight { get; protected set; }
        public virtual float UpdatePropertyHeight()
        {
            if (HasChildern)
                PropertyHeight = EditorGUIUtility.singleLineHeight;
            else
                PropertyHeight = EditorGUI.GetPropertyHeight(Property);

            return PropertyHeight;
        }

        public bool HasChildern { get { return Property.hasVisibleChildren; } }

        public List<SerializedProperty> Childern { get; protected set; }
        public float ChildernHeight { get; protected set; }
        public virtual float UpdateChildernHeight()
        {
            ChildernHeight = 0f;

            for (int i = 0; i < Childern.Count; i++)
                ChildernHeight += EditorGUI.GetPropertyHeight(Childern[i], true);

            return ChildernHeight;
        }

        public float Height { get { return PropertyHeight + ChildernHeight; } }
        public float UpdateHeight()
        {
            UpdatePropertyHeight();
            UpdateChildernHeight();

            return Height;
        }

        public virtual void Draw(ref Rect rect)
        {
            if (HasChildern)
            {
                PropertyExpanded = EditorGUI.Foldout(GUIArea.ProgressLine(ref rect), PropertyExpanded, Label);

                if (PropertyExpanded)
                    DrawChildern(ref rect);
            }
            else
            {
                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), Property);
            }
        }
        public virtual void DrawChildern(ref Rect rect)
        {
            if (!HasChildern)
                throw new Exception("Cannot Draw " + Property.name + "'s Childern, Because It Has Non");

            for (int i = 0; i < Childern.Count; i++)
                DrawChild(ref rect, Childern[i]);
        }
        protected virtual void DrawChild(ref Rect rect, SerializedProperty property)
        {
            var height = EditorGUI.GetPropertyHeight(property, true);

            EditorGUI.PropertyField(GUIArea.Progress(ref rect, height), property);
        }

        public DisectalPropertyDrawer(SerializedProperty property)
        {
            this.Property = property;

            Childern = MoeTools.Inspector.GetChildern(property);

            UpdateHeight();
        }
    }
#endif
}