using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public float timeUntilSourceFinishes = 0f;
    public SoundData data;
    [HideInInspector] public AudioSource source;
    [HideInInspector] public Sonos audioManager;

    private bool isPaused = false;
    private bool isPlayingAudio = false;
    private Transform followTransform;

    public delegate void AudioEvent ();
	public event AudioEvent OnSourceFinishPlaying = delegate { };

    void Awake() {
        source = gameObject.AddComponent<AudioSource>();
    }

    public void Unpack(Sonos _audioManager) {
        audioManager = _audioManager;
        audioManager.OnVolumeChanged += AudioSetVolume;
    }

    private void OnDisable() {
        audioManager.OnVolumeChanged -= AudioSetVolume;
    }

    public void Play(SoundData _data, Transform _followTransform) {
        data = _data;
        if (_followTransform == null) {
            source.spatialBlend = 0f;
        } else {
    //Set Source Spatial Blend
            source.spatialBlend = 1f;
            followTransform = _followTransform;
        }
        gameObject.name = data.clip.name;
        source.clip = data.clip;
        //source.loop = data.loop;
        AudioSetVolume();
        isPlayingAudio = true;
        timeUntilSourceFinishes = source.clip.length;
    //Play the clip
        source.Play();
    }
    
    public void AudioSetVolume() {
    //Set audio when the sound starts playing, but also if an audio event changes it
        if (data != null)
            source.volume = audioManager.GetAudioLevel(data.type); 
    }

    void Update() {
        if (isPlayingAudio && !isPaused) {
    //Count down if you are playing audio
            if (timeUntilSourceFinishes > Mathf.Epsilon) {
                timeUntilSourceFinishes -= Time.deltaTime;
            } else {
                if (OnSourceFinishPlaying != null)
                    OnSourceFinishPlaying.Invoke(); //If anyone is listening to this event, fire it
                if (data.loop) {
            //Loop by passing the data back to yourself, don't rely on Unity's audio looping
                    Play(data, followTransform);
                } else {
                    Stop();
                }
            }
            if (followTransform != null) {
                transform.position = followTransform.position;
            }
        }
    }

    public void Stop() {
        source.Stop();
        isPlayingAudio = false;
    //Return to pool of available sources
        audioManager.OnAudioFinishedPlaying(this);
    }
    public void Toggle() {
        isPaused = !isPaused;
        if (isPaused) {
            source.Pause();
        } else {
            source.UnPause();
        }
    }

    public void Pause() {
        isPaused = true;
        source.Pause();
    }

    public void Resume() {
        isPaused = false;
        source.UnPause();
    }
}