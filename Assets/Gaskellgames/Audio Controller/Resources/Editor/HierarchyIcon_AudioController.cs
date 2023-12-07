#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.AudioController
{
    [InitializeOnLoad]
    public class HierarchyIcon_AudioController
    {
        #region Variables

        private static readonly Texture2D icon_SoundManager;
        private static readonly Texture2D icon_SoundController;
        private static readonly Texture2D icon_SoundEffect;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Editor Loop

        static HierarchyIcon_AudioController()
        {
            icon_SoundManager = AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Audio Controller/Resources/Icons/Icon_SoundManager.png", typeof(Texture2D)) as Texture2D;
            if (icon_SoundManager == null) { return; }
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon_SoundManager;
            
            icon_SoundController = AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Audio Controller/Resources/Icons/Icon_SoundController.png", typeof(Texture2D)) as Texture2D;
            if (icon_SoundController == null) { return; }
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon_SoundController;
            
            icon_SoundEffect = AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/Audio Controller/Resources/Icons/Icon_SoundEffect.png", typeof(Texture2D)) as Texture2D;
            if (icon_SoundEffect == null) { return; }
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon_SoundEffect;
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private static void DrawHierarchyIcon_SoundManager(int instanceID, Rect rect)
        {
            if (icon_SoundManager == null) { return; }
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) { return; }
            var componant = gameObject.GetComponent<SoundManager>();
            if (componant == null) { return; }
            float hierarchyPixelHeight = 16;
            EditorGUIUtility.SetIconSize(new Vector2(hierarchyPixelHeight, hierarchyPixelHeight));
            var iconPosition = new Rect(rect.xMax - hierarchyPixelHeight, rect.yMin, rect.width, rect.height);
            var iconGUIContent = new GUIContent(icon_SoundManager);
            EditorGUI.LabelField(iconPosition, iconGUIContent);
        }
        
        private static void DrawHierarchyIcon_SoundController(int instanceID, Rect rect)
        {
            if (icon_SoundController == null) { return; }
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) { return; }
            var componant = gameObject.GetComponent<SoundController>();
            if (componant == null) { return; }
            float hierarchyPixelHeight = 16;
            EditorGUIUtility.SetIconSize(new Vector2(hierarchyPixelHeight, hierarchyPixelHeight));
            var iconPosition = new Rect(rect.xMax - hierarchyPixelHeight, rect.yMin, rect.width, rect.height);
            var iconGUIContent = new GUIContent(icon_SoundController);
            EditorGUI.LabelField(iconPosition, iconGUIContent);
        }
        
        private static void DrawHierarchyIcon_SoundEffect(int instanceID, Rect rect)
        {
            if (icon_SoundEffect == null) { return; }
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) { return; }
            var componant = gameObject.GetComponent<SoundEffect>();
            if (componant == null) { return; }
            float hierarchyPixelHeight = 16;
            EditorGUIUtility.SetIconSize(new Vector2(hierarchyPixelHeight, hierarchyPixelHeight));
            var iconPosition = new Rect(rect.xMax - hierarchyPixelHeight, rect.yMin, rect.width, rect.height);
            var iconGUIContent = new GUIContent(icon_SoundEffect);
            EditorGUI.LabelField(iconPosition, iconGUIContent);
        }

        #endregion
        
    } // class end
}

#endif