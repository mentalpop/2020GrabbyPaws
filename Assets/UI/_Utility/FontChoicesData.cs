using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "FontChoicesData", menuName = "Lists/FontChoicesData", order = 8)]
public class FontChoicesData : ScriptableObject
{
    public List<TMP_FontAsset> fonts = new List<TMP_FontAsset>();
}