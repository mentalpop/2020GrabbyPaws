using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PCTitles", menuName = "UniNav/NavButtonData PCTitles", order = 2)]
public class NavButtonDataPCTitles : NavButtonData
{
    public string title;
    [TextArea(0, 10)] public string contents;
}