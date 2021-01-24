using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReadablePCData", menuName = "Readable PC Data", order = 1)]
public class ReadablePCData : ReadableDataParent
{
    public List<NavButtonDataPCTitles> data = new List<NavButtonDataPCTitles>();
}