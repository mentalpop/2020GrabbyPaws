using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AudioType { Effect, Music, Voice}
public class Sonos : MonoBehaviour
{
    private string saveString = "volumeLevels";
    private string saveString2 = "volumeMaster";
    public static float VolumeMaster { get { return _volumeMaster; } set {
            _volumeMaster = value;
            if (instance.OnVolumeChanged != null)
                instance.OnVolumeChanged.Invoke();
            } }
    private static float _volumeMaster;

    public List<float> Volume { get {return _volume; } set {
            _volume = value;
            Debug.Log("OnVolumeChanged" + ": " + OnVolumeChanged);
            if (OnVolumeChanged != null) {
                OnVolumeChanged.Invoke();
            }
        } }
    private List<float> _volume = new List<float>();

    public int playerPoolPreferredSize = 5;
    public bool cullOnSceneChange = false;

    public static Sonos instance;

    private List<AudioPlayer> availablePlayers = new List<AudioPlayer>();
    private List<AudioPlayer> usedPlayers = new List<AudioPlayer>();

    public delegate void VolumeChangeEvent ();
	public event VolumeChangeEvent OnVolumeChanged = delegate { };

    void Awake() {
    //Singleton Pattern
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    //Volume Types setup as 1f each
        _volumeMaster = 1f;
        foreach (var aType in (AudioType[]) Enum.GetValues(typeof(AudioType))) {
            Volume.Add(1f);
        }
    //Set up the initial pool
        for (int i = 0; i < playerPoolPreferredSize; i++) {
            AddPlayerToPool();
        }
    //Subscribe to Scene management event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable() {
        UI.Instance.OnSave += Save;
        UI.Instance.OnLoad += Load;
    }
    private void OnDisable() {
        UI.Instance.OnSave -= Save;
        UI.Instance.OnLoad -= Load;
    }

    public void Save(int fileIndex) {
        List<float> volumeLevels = new List<float>();
        foreach (var volume in _volume) {
            //Debug.Log("Saving volume: "+volume);
            volumeLevels.Add(volume);
        }
        ES3.Save<List<float>>(saveString, volumeLevels);
        ES3.Save<float>(saveString2, _volumeMaster);
    }

    public void Load(int fileIndex) {
        List<float> volumeLevels = ES3.Load(saveString, new List<float>());
        for (int i = 0; i < volumeLevels.Count; i++) {
            //Debug.Log("Loading volume: "+volumeLevels[i]);
            _volume[i] = volumeLevels[i];
        }
        _volumeMaster = ES3.Load<float>(saveString2);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
//Halt all sounds which are not supposed to persist
        List<AudioPlayer> playersToStop = new List<AudioPlayer>();
        foreach (var player in usedPlayers) {
            if (!player.data.persistBetweenScenes) {
        //Add to the list of Players to Stop
                playersToStop.Add(player);
            }
        }
    //Actually call Stop on the sounds
        foreach (var player in playersToStop) {
            player.Stop();
        }
        if (cullOnSceneChange) {
            CullExtraPlayers();
        }
    }

    public static void SetVolume(AudioType aType, float _value) {
        instance.VolumeSet(aType, _value, false);
    }

    public static void ChangeVolume(AudioType aType, float _value) {
        instance.VolumeSet(aType, _value, true);
    }

    public static float GetVolume(AudioType aType) {
        return instance.Volume[(int)aType];
    }

    private void VolumeSet(AudioType aType, float _value, bool _relative) {
//Modify the volume of the requested type
        float newValue = Volume[(int)aType];
        if (_relative) {
            newValue += _value;
        } else {
            newValue = _value;
        }
        Volume[(int)aType] = Mathf.Clamp(newValue, 0f, 1f);
        if (OnVolumeChanged != null)
            OnVolumeChanged.Invoke();
    }

    public void CullExtraPlayers() {
//Pop extra players out of the stack
        while (availablePlayers.Count > playerPoolPreferredSize) {
            AudioPlayer _player = PopPlayer();
            Destroy(_player.gameObject);
        }
    }

    public void OnAudioFinishedPlaying(AudioPlayer _player) {
//Reclaim AudioPlayer
        int _index = 0;
        for (int i = 0; i < usedPlayers.Count; i++) {
            if (usedPlayers[i] == _player) {
                _index = i;
            }
        }
        _player.gameObject.name = "AvailablePlayer";
        _player.transform.position = transform.position; //Reset any possible changes to transform
        usedPlayers.RemoveAt(_index);
        availablePlayers.Add(_player);
    }

    public static AudioPlayer Play(SoundData data) {
        return instance.PlayAudio(data, null);
    }

    public static AudioPlayer PlayAt(SoundData data, Vector3 position) {
        AudioPlayer _player = PlayAt(data, null);
        _player.transform.position = position;
        return _player;
    }

    public static AudioPlayer PlayAt(SoundData data, Transform _transform) {
        return instance.PlayAudio(data, _transform);
    }

    private AudioPlayer PlayAudio(SoundData data, Transform _transform) {
        AudioPlayer _player = null;
        if (data.persistBetweenScenes && data.loop) {
    //First check if anyone is already playing this sound
            foreach (var player in instance.usedPlayers) {
                if (player.data == data) {
                    _player = player;
                    break;
                }
            }
        } 
        if (_player == null) {
            _player = instance.PopPlayer();
            instance.usedPlayers.Add(_player);
            _player.Play(data, _transform);
        }
        return _player;
    }

    public float GetAudioLevel(AudioType audioType) {
        /*float _volume = 0f;
        
        switch (audioType) {
    //Set Audio Levels based on type of Audio
            case AudioType.Effect: _volume = volumeEffects; break;
            case AudioType.Music: _volume = volumeMusic; break;
            case AudioType.Voice: _volume = volumeVoice; break;
        }
        //*/
        return _volume[(int)audioType] * _volumeMaster; //Multiply source audio by Master audio to set true level
    }

    public void StopAll() {
        foreach (var player in usedPlayers) {
    //Stop all sources of audio, reclaim all players
            player.Stop();
        }
    }

    private AudioPlayer PopPlayer() {
        if (availablePlayers.Count < 1) {
    //Have you run out? Add a new one
            AddPlayerToPool();
        }
//Grab one from the top of the list, add it to list of "Used" sources
        AudioPlayer _player = availablePlayers[0];
        availablePlayers.RemoveAt(0);
        if (_player == null) {
            Debug.LogWarning("_source is null: " + _player);
        }
        return _player;
    }

    private void AddPlayerToPool() {
        GameObject newAudioSource = new GameObject("AvailablePlayer");
        newAudioSource.transform.SetParent(transform); //Child Audio Sources to this Instance
    //Add audio Source for each sound in the game and supply it's Clip
        AudioPlayer newPlayer = newAudioSource.AddComponent<AudioPlayer>();
        newPlayer.Unpack(this);
        availablePlayers.Add(newPlayer);
    }

    /*
    void FixedUpdate() {
        if (OnVolumeChanged != null)
            OnVolumeChanged.Invoke();
    }
    //*/
}