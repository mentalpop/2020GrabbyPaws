using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GadgetList", menuName = "Lists/GadgetList", order = 4)]
public class GadgetList : ScriptableObject
{
    public List<GadgetData> gadgets = new List<GadgetData>();
}