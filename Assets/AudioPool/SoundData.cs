using UnityEngine;

//[System.Serializable]
[CreateAssetMenu(fileName = "newSound", menuName = "SoundData", order = 0)]
public class SoundData : ScriptableObject
{
    public AudioClip clip;
    public AudioType type = AudioType.Effect;
    public bool loop = false;
    public bool persistBetweenScenes = false;
}
