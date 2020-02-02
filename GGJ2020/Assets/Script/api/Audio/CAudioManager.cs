﻿using System.Collections;
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

    public const float SignalTracehold = .3f;
    public AudioMixer Mixer;
    public AudioMixerGroup NoiseMixerGroup;
    public AudioMixerGroup VoiceMixerGroup;
    public AudioMixerGroup MachineOverall;
    public AudioMixerGroup GrannyGroup;
    public AudioMixerGroup TurnOnGroup;
    public AudioSource TurnOn;
    public AudioSource TurnOff;
    public List<AudioSource> NoiseComponents;
    public List<AudioSource> VoiceComponents;
    public AudioSource grannySource;
    public List<CAudio> GrannyResources = new List<CAudio>();
    public int grannyIndex = 0;
    public List<CAudio> NoiseResources;
    public List<CAudio> VoiceResources;
    public List<CAudioStatistics> VoiceStatistics;

    public float mAudioPreviousPercentage = 0;


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

    public void LoadGranny()
    {
        GrannyResources = CAudioLoader.Inst.getGrannyAudios();
        GrannyResources.Shuffle();

        // for (int i = 0; i < GrannyResources.Count; i++)
        // {
        //     AudioSource source = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        //     NoiseComponents.Add(source);
        //     source.outputAudioMixerGroup = NoiseMixerGroup;
        //     source.clip = NoiseResources[i].mClip;
        //     source.volume = 0;
        //     source.loop = true;
        // }
    }

    public void LoadNoises(int count)
    {
        var noises = CAudioLoader.Inst.getNoises();
        Debug.Assert(noises.TrueForAll(n => n.mClip != null && n.mNoise && !n.isGranny));
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
        var mainAudios = CAudioLoader.Inst.getBaseAudios();
        Debug.Assert(0 < mainAudios.Count);
        Debug.Assert(mainAudios.TrueForAll(n => n.mClip != null && !n.mNoise));// && n.mMain));
        mainAudios.Shuffle();
        VoiceResources.Add(mainAudios[0]);

        if (1 < count)
        {
            var baseAudios = CAudioLoader.Inst.getBaseAudios();
            Debug.Assert(count - 1 <= baseAudios.Count);
            Debug.Assert(baseAudios.TrueForAll(n => n.mClip != null && !n.mNoise));// && !n.mMain));
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
        ResetStatistics();
    }
    private void ResetStatistics()
    {
        VoiceStatistics = new List<CAudioStatistics>();
        foreach (var voice in VoiceResources)
        {
            VoiceStatistics.Add(new CAudioStatistics { mAudio = voice });
        }
    }

    // Noice
    public void UpdateNoiseVolume(int noise, float volume)
    {
        Debug.Assert(0 <= noise && noise < NoiseComponents.Count);
        Debug.Assert(0 <= volume && volume <= 1);
        NoiseComponents[noise].volume = volume;
    }

    // Voices
    public void SetVoice(int voice)
    {
        Debug.Assert(voice == -1 || 0 <= voice && voice < VoiceResources.Count);
        if (currentVoice == voice)
            return;

        if (voice != -1)
        {
            PlayVoice(voice);
        }
        else
        {
            StopVoice();
        }
    }
    private int currentVoice = -1;
    private float currentVoiceTraceholdInPoint = -1; // In seconds
    private void PlayVoice(int voice)
    {
        currentVoice = voice;
    }

    private void StopVoice()
    {
        VoiceComponents[currentVoice].volume = 0;
    }

    public void UpdateVoiceVolume(float volume)
    {
        mAudioPreviousPercentage = volume;
        if (currentVoice != -1)
        {
            var audioSource = VoiceComponents[currentVoice % VoiceResources.Count];
            if (currentVoiceTraceholdInPoint != -1)
            {
                // In signal
                if (audioSource.time < currentVoiceTraceholdInPoint)
                {
                    // Looped
                    VoiceStatistics[currentVoice].AddSegment(currentVoiceTraceholdInPoint, audioSource.clip.length);
                    currentVoiceTraceholdInPoint = 0;
                    Debug.Log("Loop signal");
                }
            }
            if (audioSource.volume < SignalTracehold &&
                volume > SignalTracehold)
            {
                // Signal on
                currentVoiceTraceholdInPoint = audioSource.time;
                Debug.Log("Signaled");
            }
            if (audioSource.volume > SignalTracehold &&
                volume < SignalTracehold)
            {
                // Signal off
                VoiceStatistics[currentVoice].AddSegment(currentVoiceTraceholdInPoint, audioSource.clip.length);
                currentVoiceTraceholdInPoint = -1;
                Debug.Log("Lost signal");
            }
            audioSource.volume = volume;

            Debug.Log("*** current voice percentage: " + VoiceStatistics[currentVoice].GetPercentage());

            if (VoiceStatistics[currentVoice].GetPercentage() >= 0.5f && !VoiceStatistics[currentVoice].mAudio.hasGrannyPlayed)
            {
                VoiceStatistics[currentVoice].mAudio.hasGrannyPlayed = true;
                CAudioManager.Inst.playGranny();
            }
        }
        SetMainNoiseVolume(1 - volume);
    }

    private void SetMainNoiseVolume(float volume)
    {
        Debug.Assert(0 <= volume && volume <= 1);
        Mixer.SetFloat(string.Format("NoiseVol", NoiseMixerGroup), Mathf.Log(volume) * 20);
    }

    public void stopAllFrequencies()
    {
        Debug.Log("stopAllFrequencies");
        //UpdateVoiceVolume(0);
        //SetMainNoiseVolume(0);

        Mixer.SetFloat("MachineVol", float.MinValue);
    }

    public void restartAllFrequencies()
    {
        Debug.Log("restart them!!");
        //UpdateVoiceVolume(mAudioPreviousPercentage);

        Mixer.SetFloat("MachineVol", 0);
    }

    public List<CAudioStatistics> getAudiosListened()
    {
        List<CAudioStatistics> aList = new List<CAudioStatistics>();

        for (int i = 0; i < VoiceStatistics.Count; i++)
        {
            if (VoiceStatistics[i].GetPercentage() > 0.5f)
            {
                aList.Add(VoiceStatistics[i]);
            }
        }

        return aList;
    }

    public float getTotalPoints()
    {
        float aTotal = 0;

        float aCurrent = 0;

        List<CAudioStatistics> aList = getAudiosListened();

        for (int i = 0; i < VoiceStatistics.Count; i++)
        {
            if (aList.Contains(VoiceStatistics[i]))
            {
                Debug.Log("add to current!");
                aCurrent += VoiceStatistics[i].mAudio.mPuntaje;
            }

            aTotal += VoiceStatistics[i].mAudio.mPuntaje;
        }
        Debug.Log("current: " + aCurrent + " total: " + aTotal);

        float aPercent = aCurrent / aTotal;

        Debug.Log("aCurrent/aTotal: " + (aPercent));

        return aPercent;
    }

    public void playGranny()
    {
        Debug.Log("*** play GRANNY!");

        grannyIndex += 1;

        if (grannyIndex < 0 || grannyIndex >= GrannyResources.Count)
        {
            grannyIndex = 0;
        }

        grannySource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        grannySource.outputAudioMixerGroup = GrannyGroup;
        grannySource.clip = GrannyResources[grannyIndex].mClip;
        grannySource.loop = false;
        grannySource.Play();
    }

    public void playCachivache()
    {
        CAudio cachivache = CAudioLoader.Inst.mCachivache;

        grannySource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        grannySource.outputAudioMixerGroup = GrannyGroup;
        grannySource.clip = cachivache.mClip;
        grannySource.loop = false;
        grannySource.Play();
    }

    public void loadTurnOnGroup()
    {
        AudioClip aTurnOn = Resources.Load<AudioClip>("Audio/InicioMaquina");
        AudioClip aTurnOff = Resources.Load<AudioClip>("Audio/StopMaquina");

        TurnOn = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        TurnOn.clip = aTurnOn;
        TurnOn.outputAudioMixerGroup = TurnOnGroup;
        TurnOn.loop = false;

        TurnOff = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        TurnOff.clip = aTurnOff;
        TurnOff.outputAudioMixerGroup = TurnOnGroup;
        TurnOff.loop = false;

    }

    public void turnOn()
    {
        if (TurnOn != null)
        {
            TurnOn.Play();
        }
    }

    public void turnOff()
    {
        if (TurnOff != null)
        {
            TurnOff.Play();
        }
    }


}
