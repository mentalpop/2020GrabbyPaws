using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using PixelCrushers.DialogueSystem;

[CreateAssetMenu(fileName = "Secret", menuName = "Secret Data", order = 4)]
public class NotSecretData : ScriptableObject
{
    public Secrets secret;
    [TextArea(3, 10)]
    public string text;
}