// ----------------------------------------------------------------------------
// <copyright file="PhotonRigidbodyViewEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the RigidbodyView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Tim
{
	using UnityEditor;
	using UnityEngine;
    using Photon.Pun;

    [CustomEditor(typeof (MyPhotonRigidbodyView))]
    public class MyPhotonRigidbodyViewEditor : MonoBehaviourPunEditor
    {
        public override void OnInspectorGUI()
        {
            //Debug.Log("OnInspectorGUI");

            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Editing is disabled in play mode.", MessageType.Info);
                return;
            }

            MyPhotonRigidbodyView view = (MyPhotonRigidbodyView)target;

            view.m_TeleportEnabled = PhotonGUI.ContainerHeaderToggle("Enable teleport for large distances", view.m_TeleportEnabled);

            if (view.m_TeleportEnabled)
            {
                Rect rect = PhotonGUI.ContainerBody(20.0f);
                view.m_TeleportIfDistanceGreaterThan = EditorGUI.FloatField(rect, "Teleport if distance greater than", view.m_TeleportIfDistanceGreaterThan);
            }

            view.m_SynchronizeVelocity = PhotonGUI.ContainerHeaderToggle("Synchronize Velocity", view.m_SynchronizeVelocity);
            view.m_SynchronizeAngularVelocity = PhotonGUI.ContainerHeaderToggle("Synchronize Angular Velocity", view.m_SynchronizeAngularVelocity);
            
            if (GUI.changed)
            {
                //Debug.Log("GUI.changed = true");
                EditorUtility.SetDirty(view);
                /*
                https://blog.csdn.net/cui6864520fei000/article/details/86226078
                EditorUtility.SetDirty 设置已改变
                static function SetDirty (target : Object) : void

                Description描述

                Marks target object as dirty.

                标记目标物体已改变。

                Unity internally uses the dirty flag to find out when assets have changed and need to be saved to disk.

                当资源已改变并需要保存到磁盘，Unity内部使用dirty标识来查找。

                E.g. if you modify a prefab's MonoBehaviour or ScriptableObject variables, you must tell Unity that the value has changed. Unity builtin components internally call SetDirty whenever a property changes. MonoBehaviour or ScriptableObject don't do this automatically so if you want your value to be saved you need to call SetDirty.

                例如，如果修改一个prefab的MonoBehaviour或ScriptableObject变量，必须告诉Unity该值已经改变。每当一个属性发生变化，Unity内置组件在内部调用setDirty。MonoBehaviour或ScriptableObject不自动做这个，因此如果你想值被保存，必须调用SetDirty。
                */
            }
        }
    }
}