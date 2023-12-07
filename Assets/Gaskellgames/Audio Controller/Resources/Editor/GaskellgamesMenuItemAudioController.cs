#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

using Gaskellgames;

namespace Gaskellgames.AudioController
{
    public class GaskellgamesMenuItemAudioController : GaskellgamesMenuItem
    {
        #region Tools Menu

        private const string AudioControllerToolsMenu = GaskellgamesToolsMenu + "/Audio Controller";

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region GameObject Menu
        
        private const string AudioControllerGameobjectMenu = GaskellgamesGameobjectMenu + "/Audio Controller";

        [MenuItem(AudioControllerGameobjectMenu + "/Sound Manager", false, 10)]
        private static void Gaskellgames_GameobjectMenu_SoundManager()
        {
            // Create a custom game object
            GameObject go = new GameObject("SoundManager [DDoL]");
            GameObject goChild1 = new GameObject("AudioSource_Music");
            GameObject goChild2 = new GameObject("AudioSource_SoundFX");
            GameObject goChild3 = new GameObject("AudioSource_Environment");
            GameObject goChild4 = new GameObject("AudioSource_MenuUI");
            goChild1.transform.SetParent(go.transform);
            goChild2.transform.SetParent(go.transform);
            goChild3.transform.SetParent(go.transform);
            goChild4.transform.SetParent(go.transform);
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            SoundManager soundManager = go.AddComponent<SoundManager>();
            AudioSource source1 = goChild1.AddComponent<AudioSource>();
            AudioSource source2 = goChild2.AddComponent<AudioSource>();
            AudioSource source3 = goChild3.AddComponent<AudioSource>();
            AudioSource source4 = goChild4.AddComponent<AudioSource>();
            soundManager.AudioSourceMusic = source1;
            soundManager.AudioSourceSoundFX = source2;
            soundManager.AudioSourceEnvironment = source3;
            soundManager.AudioSourceMenuUI = source4;
            AudioMixer audioMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Gaskellgames/Audio Controller/Content/MainMixer.mixer");
            AudioMixerGroup[] audioMixGroup = audioMixer.FindMatchingGroups("Music");
            source1.outputAudioMixerGroup = audioMixGroup[0];
            audioMixGroup = audioMixer.FindMatchingGroups("SoundFX");
            source2.outputAudioMixerGroup = audioMixGroup[0];
            audioMixGroup = audioMixer.FindMatchingGroups("Environment");
            source3.outputAudioMixerGroup = audioMixGroup[0];
            audioMixGroup = audioMixer.FindMatchingGroups("UI");
            source4.outputAudioMixerGroup = audioMixGroup[0];
        }
        
        [MenuItem(AudioControllerGameobjectMenu + "/Sound Manager", true, 10)]
        private static bool Gaskellgames_GameobjectMenu_SoundManagerValidate()
        {
            SoundManager exists = GameObject.FindObjectOfType<SoundManager>();
            if (exists) { return false; } else { return true; }
        }
        
        [MenuItem(AudioControllerGameobjectMenu + "/Sound Controller", false, 10)]
        private static void Gaskellgames_GameobjectMenu_SoundController()
        {
            // Create a custom game object
            GameObject go = new GameObject("Sound Controller");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            go.AddComponent<SoundController>();
        }
        
        [MenuItem(AudioControllerGameobjectMenu + "/Sound Controller", true, 10)]
        private static bool Gaskellgames_GameobjectMenu_SoundControllerValidate()
        {
            SoundController exists = GameObject.FindObjectOfType<SoundController>();
            if (exists) { return false; } else { return true; }
        }
        
        [MenuItem(AudioControllerGameobjectMenu + "/3D Sound Effect", false, 25)]
        private static void Gaskellgames_GameobjectMenu_SoundEffect()
        {
            // Create a custom game object
            GameObject go = new GameObject("Sound Effect");
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            
            // Add scripts & components
            go.transform.position = Vector3.zero;
            go.AddComponent<SoundEffect>();
        }
        
        #endregion
        
    } // class end
}

#endif