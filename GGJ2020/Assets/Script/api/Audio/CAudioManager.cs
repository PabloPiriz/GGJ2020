using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;

public class CAudioManager : MonoBehaviour
{
    private static CAudioManager _inst;
    public static CAudioManager Inst
    {
        get
        {
            if (_inst == null)
            {
                return Instantiate<GameObject>(
                            Resources.Load<GameObject>("Audio/AudioManager"))
                            .GetComponent<CAudioManager>();
            }
            else
            {
                return _inst;
            }
        }
    }

    public AudioMixer Mixer;
    public AudioMixerGroup NoiseMixerGroup;
    public AudioMixerGroup VoiceMixerGroup;
    public List <AudioSource> NoiseComponents;
    public List <AudioSource> VoiceComponents;
    public List<CAudio> NoiseResources;
    public List<CAudio> VoiceResources;
    
    void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _inst = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadNoises(int count)
    {
        var noises = CAudioLoader.Inst.getNoises();
        Debug.Assert(noises.TrueForAll(n => n.mClip != null && n.mNoise));
        noises.Shuffle();
        NoiseResources = noises.GetRange(0, count);

        // TODO: reuse components
        if (NoiseComponents != null)
        {
            foreach (var cmp in NoiseComponents)
            {
                cmp.Pause();
                DestroyImmediate(cmp);
            }
            NoiseComponents.Clear();
        }

        NoiseComponents = new List<AudioSource>(count);
        for (int i = 0; i < count; ++i)
        {
            AudioSource source = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            NoiseComponents.Add(source);
            source.outputAudioMixerGroup = NoiseMixerGroup;
            source.clip = NoiseResources[i].mClip;
            source.volume = 0;
            source.loop = true;
            source.Play();
        }
    }
    public void LoadVoices(int count)
    {
        Debug.Assert(0 < count);
        Debug.Log(string.Format("Creating {0} voice audio sources", count));
        if (VoiceComponents != null)
        {
            foreach (var cmp in VoiceComponents)
            {
                cmp.Pause();
                DestroyImmediate(cmp);
            }
            VoiceComponents.Clear();
        }

        VoiceResources = new List<CAudio>(count);
        var mainAudios = CAudioLoader.Inst.getMainAudios();
        Debug.Assert(0 < mainAudios.Count);
        Debug.Assert(mainAudios.TrueForAll(n => n.mClip != null && !n.mNoise && n.mMain));
        mainAudios.Shuffle();
        VoiceResources.Add(mainAudios[0]);

        if (1 < count)
        {
            var baseAudios = CAudioLoader.Inst.getBaseAudios();
            Debug.Assert(count - 1 <= baseAudios.Count);
            Debug.Assert(baseAudios.TrueForAll(n => n.mClip != null && !n.mNoise && !n.mMain));
            baseAudios.Shuffle();
            VoiceResources.AddRange(baseAudios.GetRange(0, count - 1));
        }

        VoiceComponents = new List<AudioSource>(count);
        for (int i = 0; i < count; ++i)
        {
            AudioSource source = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            VoiceComponents.Add(source);
            source.outputAudioMixerGroup = VoiceMixerGroup;
            source.clip = VoiceResources[i].mClip;
            source.volume = 0;
            source.loop = true;
            source.Play();
        }
    }

    public void UpdateNoiseVolume(int noise, float volume)
    {
        Debug.Assert(0 <= noise && noise < NoiseComponents.Count);
        Debug.Assert(0 <= volume && volume <= 1);
        NoiseComponents[noise].volume = volume;
    }
    
    private int currentVoice = -1;
    public void SetVoice(int voice)
    {
        Debug.Assert(-1 <= voice && voice < VoiceResources.Count);
        Debug.Log(string.Format("Play clip {0}", voice == -1 ? "silence" : VoiceResources[voice % VoiceResources.Count].mId));
        currentVoice = voice;
        VoiceComponents.ForEach(c => c.volume = 0);
    }
    public void UpdateVoiceVolume(float volume)
    {
        Debug.Log(string.Format("Update voice volume: {0}", volume));
        if (currentVoice != -1)
        {
            VoiceComponents[currentVoice % VoiceResources.Count].volume = volume;
        }
        SetMainNoiseVolume(1 - volume);
    }

    private void SetMainNoiseVolume(float volume)
    {
        Debug.Assert(0 <= volume && volume <= 1);
        Mixer.SetFloat(string.Format("NoiseVol", NoiseMixerGroup), Mathf.Log(volume) * 20);
    }
}
