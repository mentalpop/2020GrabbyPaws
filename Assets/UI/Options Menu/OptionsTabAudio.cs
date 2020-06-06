using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsTabAudio : MonoBehaviour
{
    public Slider volumeMaster;
    public Slider volumeBGM;
    public Slider volumeSFX;

    private void OnEnable() {
        volumeMaster.value = Sonos.VolumeMaster;
        //Debug.Log("volumeMaster.value: "+volumeMaster.value);
        volumeMaster.onValueChanged.AddListener (delegate {volumeMaster_onValueChanged ();});
        volumeBGM.value = Sonos.GetVolume(AudioType.Music);
        //Debug.Log("volumeBGM.value: "+volumeBGM.value);
        volumeBGM.onValueChanged.AddListener (delegate {volumeBGM_onValueChanged ();});
        volumeSFX.value = Sonos.GetVolume(AudioType.Effect);
        //Debug.Log("volumeSFX.value: "+volumeSFX.value);
        volumeSFX.onValueChanged.AddListener (delegate {volumeSFX_onValueChanged ();});
    }

    private void OnDisable() {
        volumeMaster.onValueChanged.RemoveListener (delegate {volumeMaster_onValueChanged ();});
        volumeBGM.onValueChanged.RemoveListener (delegate {volumeBGM_onValueChanged ();});
        volumeSFX.onValueChanged.RemoveListener (delegate {volumeSFX_onValueChanged ();});
    }

    public void volumeMaster_onValueChanged() {
        Sonos.VolumeMaster = volumeMaster.value;
    }

    public void volumeBGM_onValueChanged() {
        Sonos.SetVolume(AudioType.Music, volumeBGM.value);
    }
    public void volumeSFX_onValueChanged() {
        Sonos.SetVolume(AudioType.Effect, volumeSFX.value);
    }
}