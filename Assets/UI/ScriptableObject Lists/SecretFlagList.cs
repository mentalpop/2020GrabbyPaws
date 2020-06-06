using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SecretFlags", menuName = "Lists/Not Secret List", order = 1)]
public class SecretFlagList : ScriptableObject
{
    public List<NotSecretData> secrets = new List<NotSecretData>();
}