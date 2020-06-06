using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCList", menuName = "Lists/NPC List", order = 1)]
public class NPCListData : ScriptableObject
{
    public List<NPCProfileUIData> npcList = new List<NPCProfileUIData>();
}