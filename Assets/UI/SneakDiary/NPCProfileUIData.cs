using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCProfile", menuName = "SneakDiary/NPC Profile Data", order = 1)]
public class NPCProfileUIData : ScriptableObject
{
    public string rName;
    public Sprite sprite;
    public List<NightPhaseData> nightPhases = new List<NightPhaseData>();
    public List<QuestNames> quests = new List<QuestNames>();
}

[System.Serializable]
public class NightPhaseData
{
    public List<TimeIntervalData> intervals = new List<TimeIntervalData>();
}