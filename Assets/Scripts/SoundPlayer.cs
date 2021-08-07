using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;

public class SoundPlayer : SingletonBehaviour<SoundPlayer>
{
    [SerializeField] AudioMixerGroup mixerGroup;
    [SerializeField] AnimationCurve audioVolumeCurve;

    List<PlayingAudio> currentlyPlaying = new List<PlayingAudio>();

    private void Start()
    {
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        mixerGroup.audioMixer.SetFloat("MasterVolume", audioVolumeCurve.Evaluate(Settings.Instance.MasterVolume));
    }

    public void ChangeVolume(Single volume)
    {
        Settings.Instance.MasterVolume = volume;
        UpdateVolume();
    }

    public void Play(AudioClip clip, GameObject toPlayFrom = null, Transform source3D = null, float volume = 1, float randomPitchRange = 0, bool playOnlyIfFinished = false)
    {
        float pitch = 1f + UnityEngine.Random.Range(-randomPitchRange, randomPitchRange);
        PlayClipWithPitch(clip, pitch, toPlayFrom, source3D, volume, playOnlyIfFinished);
    }

    public void PlayClipWithPitch(AudioClip clip, float pitch, GameObject toPlayFrom = null, Transform source3D = null, float volume = 1, bool playOnlyIfFinished = false)
    {
        if (clip == null)
            return;

        if (playOnlyIfFinished && ClipIsBeingPlayed(clip))
            return;

        PlayingAudio newAudio = new PlayingAudio();

        if (toPlayFrom != null)
        {
            StopAllSoundsFromSource(toPlayFrom);
            newAudio.origin = toPlayFrom;
        }


        newAudio.source = gameObject.AddComponent<AudioSource>();
        newAudio.source.clip = clip;
        newAudio.source.volume = volume;
        newAudio.source.pitch = pitch;
        newAudio.source.outputAudioMixerGroup = mixerGroup;
        newAudio.source.Play();
        newAudio.coroutine = StartCoroutine(WaitForEndOfClipRoutine(newAudio));
        currentlyPlaying.Add(newAudio);

        if (source3D != null)
        {
            newAudio.source.transform.position = source3D.position;
            newAudio.source.spatialBlend = 1f;
        }
    }

    private bool ClipIsBeingPlayed(AudioClip clip)
    {
        foreach (var playing in currentlyPlaying)
        {
            if (playing != null && playing.source != null && playing.source.clip == clip)
                return true;
        }

        return false;
    }

    private void StopAllSoundsFromSource(GameObject toPlayFrom)
    {
        foreach (var playing in currentlyPlaying.Where(g => g.origin == toPlayFrom))
        {
            Destroy(playing.source);
            StopCoroutine(playing.coroutine);
        }
    }

    private IEnumerator WaitForEndOfClipRoutine(PlayingAudio playingAudio)
    {
        yield return new WaitForSeconds(playingAudio.source.clip.length);
        Destroy(playingAudio.source);
        currentlyPlaying.Remove(playingAudio);
    }
}

public class PlayingAudio
{
    public GameObject origin;
    public Coroutine coroutine;
    public AudioSource source;
}
