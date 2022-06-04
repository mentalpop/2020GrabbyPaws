using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfirmationPromptData", menuName = "ConfirmationPromptData", order = 11)]
public class ConfirmationPromptData : ScriptableObject
{
    public ConfirmationPromptID promptID;
    public string header;
    [TextArea(3, 5)]
    public string description;
    public string buttonA = "OKAY";
    public string buttonB = "NOPE";
}

public enum ConfirmationPromptID
{
    Rewind,
    Hockster,
    Gadget,
    Save,
    Load,
    QuitToTitle,
    QuitGame,
    NewGame,
    RestoreDefaults,
    EraseSaveData,
    Buy
}