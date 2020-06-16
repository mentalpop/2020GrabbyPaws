using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReadablePCData", menuName = "Readable PC Data", order = 1)]
public class ReadablePCData : ScriptableObject {
    
    public string title;
    public string readFlag;
    [TextArea(0, 10)] public string contents;
}