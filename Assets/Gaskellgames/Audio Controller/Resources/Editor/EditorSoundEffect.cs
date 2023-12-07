using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.AudioController
{
    [CustomEditor(typeof(SoundEffect))][CanEditMultipleObjects]
    public class EditorSoundEffect : Editor
    {
        #region Serialized Properties / OnEnable

        SerializedProperty mixerMaster;
        SerializedProperty mixerMusic;
        SerializedProperty mixerSoundFX;
        SerializedProperty mixerEnvironment;
        SerializedProperty mixerMenuUI;
        
        SerializedProperty soundManager;
        SerializedProperty sphereCollider;
        SerializedProperty audioSource;
        SerializedProperty audioMixerGroup;
        SerializedProperty gizmoColor;
        SerializedProperty alwaysShowZone;
        SerializedProperty disableAfterSetup;
        SerializedProperty soundLibrary;
        SerializedProperty soundID;

        bool mixerGroup = false;
        bool referenceGroup = false;

        private void OnEnable()
        {
            mixerMaster = serializedObject.FindProperty("mixerMaster");
            mixerMusic = serializedObject.FindProperty("mixerMusic");
            mixerSoundFX = serializedObject.FindProperty("mixerSoundFX");
            mixerEnvironment = serializedObject.FindProperty("mixerEnvironment");
            mixerMenuUI = serializedObject.FindProperty("mixerMenuUI");
            
            soundManager = serializedObject.FindProperty("soundManager");
            sphereCollider = serializedObject.FindProperty("sphereCollider");
            audioSource = serializedObject.FindProperty("audioSource");
            audioMixerGroup = serializedObject.FindProperty("audioMixerGroup");
            gizmoColor = serializedObject.FindProperty("gizmoColor");
            alwaysShowZone = serializedObject.FindProperty("alwaysShowZone");
            disableAfterSetup = serializedObject.FindProperty("disableAfterSetup");
            soundLibrary = serializedObject.FindProperty("soundLibrary");
            soundID = serializedObject.FindProperty("soundID");
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            SoundEffect soundEffect = (SoundEffect)target;
            serializedObject.Update();

            // banner
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Audio Controller/Resources/Icons/inspectorBanner_AudioController.png", typeof(Texture));
            GUILayout.Box(banner, GUILayout.ExpandWidth(true), GUILayout.Height(Screen.width / 7.5f));

            // custom inspector:
            mixerGroup = EditorGUILayout.BeginFoldoutHeaderGroup(mixerGroup, "Audio Mix Channels");
            if (mixerGroup)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(mixerMaster);
                EditorGUILayout.PropertyField(mixerMusic);
                EditorGUILayout.PropertyField(mixerSoundFX);
                EditorGUILayout.PropertyField(mixerEnvironment);
                EditorGUILayout.PropertyField(mixerMenuUI);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            referenceGroup = EditorGUILayout.BeginFoldoutHeaderGroup(referenceGroup, "References");
            if (referenceGroup)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(soundManager);
                EditorGUILayout.PropertyField(sphereCollider);
                EditorGUILayout.PropertyField(audioSource);
                EditorGUILayout.PropertyField(audioMixerGroup);
                EditorGUILayout.PropertyField(gizmoColor);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(alwaysShowZone);
            EditorGUILayout.PropertyField(disableAfterSetup);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(soundLibrary);
            EditorGUILayout.PropertyField(soundID);

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

    #endregion

    } // class end
}
