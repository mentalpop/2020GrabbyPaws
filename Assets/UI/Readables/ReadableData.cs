using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Readable", menuName = "Readable", order = 1)]
public class ReadableData : ReadableDataParent
{
    [TextArea(0, 10)] public string contents;
}