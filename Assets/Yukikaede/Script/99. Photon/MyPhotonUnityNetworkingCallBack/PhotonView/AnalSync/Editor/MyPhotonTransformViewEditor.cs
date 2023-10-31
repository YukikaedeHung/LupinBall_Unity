// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the TransformView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------
/*★此腳本因使用UnityEditor，務必放在Editor資料夾底下，否則無法輸出*/

namespace Tim
{
    using UnityEditor;
    using UnityEngine;
    using Photon.Pun;

    [CustomEditor(typeof(MyPhotonTransformView))]
    public class MyPhotonTransformViewEditor : Editor
    {
        private bool helpToggle = false;

        SerializedProperty pos, rot, scl, lcl, obj;//★修改


        public void OnEnable()
        {
            pos = serializedObject.FindProperty("m_SynchronizePosition");
            rot = serializedObject.FindProperty("m_SynchronizeRotation");
            scl = serializedObject.FindProperty("m_SynchronizeScale");
            lcl = serializedObject.FindProperty("m_UseLocal");
            obj = serializedObject.FindProperty("obj");//★自加
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Editing is disabled in play mode.", MessageType.Info);
                return;
            }

            MyPhotonTransformView view = (MyPhotonTransformView)target;//★修改


            EditorGUILayout.LabelField("Synchronize Options");


            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginVertical("HelpBox");
                {
                    EditorGUILayout.PropertyField(pos, new GUIContent("Position", pos.tooltip));
                    EditorGUILayout.PropertyField(rot, new GUIContent("Rotation", rot.tooltip));
                    EditorGUILayout.PropertyField(scl, new GUIContent("Scale", scl.tooltip));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(lcl, new GUIContent("Use Local", lcl.tooltip));
                EditorGUILayout.PropertyField(obj, new GUIContent("obj", obj.tooltip));//★自加
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            this.helpToggle = EditorGUILayout.Foldout(this.helpToggle, "Info");
            if (this.helpToggle)
            {
                EditorGUILayout.HelpBox("The Photon Transform View of PUN 2 is simple by design.\nReplace it with the Photon Transform View Classic if you want the old options.\nThe best solution is a custom IPunObservable implementation.", MessageType.Info, true);
            }
        }
    }
}