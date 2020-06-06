using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestPlayAudio : MonoBehaviour
{
    //public AudioManager audioManager;
    public SoundData bgm;
    public SoundData sfx;
    public SoundData missileLoop;

    void Start() {
        Sonos.Play(bgm);
        Sonos.PlayAt(missileLoop, transform);
        Sonos.VolumeMaster = 0.5f;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Sonos.PlayAt(sfx, transform);
        }
        if (Input.GetMouseButtonDown(1)) {
            Sonos.Play(sfx);
        }
        if (Input.GetMouseButtonDown(2)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.mouseScrollDelta.y > 0f )  {// forward
            //Sonos.VolumeMaster += 0.1f;
            //Sonos.instance.Volume[(int)AudioType.Music] += 0.1f;
            Sonos.ChangeVolume(AudioType.Effect, 0.1f);
        } else if (Input.mouseScrollDelta.y <  0f )  {// backwards
            //Sonos.VolumeMaster -= 0.1f;
            //Sonos.instance.Volume[(int)AudioType.Music] -= 0.1f;
            Sonos.ChangeVolume(AudioType.Effect, -0.1f);
        }

    }
}