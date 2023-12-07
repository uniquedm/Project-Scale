using UnityEngine;
using UnityEngine.Audio;

using Gaskellgames;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.AudioController
{
    [System.Serializable]
    public class SoundData
    {
        public string ID;
        public string name;
        public string artist;
        public AudioClip clip;

        public AudioMixerGroup outputAudioMixerGroup;

        public bool mute;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;
        public bool playOnAwake;
        public bool loop;

        [Range(0f, 256f)] public int priority;
        [Range(0f, 1f)] public float volume;
        [Range(-3f, 3f)] public float pitch;
        [Range(-1f, 1f)] public float panStereo;
        [Range(0f, 1f)] public float spatialBlend;
        [Range(0f, 1.1f)] public float reverbZoneMix;

        [HideInInspector] public AudioSource source;
    }
}