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
    [CreateAssetMenu(menuName = MoeTools.Constants.Paths.Tools + "Sound Set")]
    public partial class SoundSet : ScriptableObject
    {
        [SerializeField]
        AudioClip[] clips;
        public AudioClip[] Clips { get { return clips; } }

        public AudioClip RandomClip { get { return clips.GetRandom(); } }

#if UNITY_EDITOR
        [CustomEditor(typeof(SoundSet))]
        public class Inspector : MoeInspector<SoundSet>
        {
            InspectorList clips;

            protected override void OnEnable()
            {
                base.OnEnable();

                clips = new InspectorList(serializedObject.FindProperty("clips"));

                CustomGUI.Overrides.Add(clips.serializedProperty.name, DrawClips);
            }

            protected virtual void DrawClips()
            {
                clips.Draw();
            }
        }
#endif
    }
}