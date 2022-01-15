using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadPromptManager : MonoBehaviour
{
    public GameObject prefabPrompt;
    public enum PromptType
    {
        PromptIgnoredGamepad,
        PromptDetectedGamepad,
        PromptDetectedMouse
    }
    public Sprite promptIgnoredGamepad;
    public string promptTextIgnoredGamepad;
    public Sprite promptDetectedGamepad;
    public string promptTextDetectedGamepad;
    public Sprite promptDetectedMouse;
    public string promptTextDetectedMouse;

    public void CreatePrompt(PromptType promptType) {
        GameObject newGO = Instantiate(prefabPrompt, transform, false);
        Sprite _promptSprite = promptIgnoredGamepad;
        string _promptText = "";
        switch (promptType) {
            case PromptType.PromptIgnoredGamepad:
                _promptSprite = promptIgnoredGamepad;
                _promptText = promptTextIgnoredGamepad; 
                break;
            case PromptType.PromptDetectedGamepad:
                _promptSprite = promptDetectedGamepad;
                _promptText = promptTextDetectedGamepad;
                break;
            case PromptType.PromptDetectedMouse:
                _promptSprite = promptDetectedMouse;
                _promptText = promptTextDetectedMouse;
                break;
        }
        newGO.GetComponent<ControllerPrompt>().Unpack(_promptSprite, _promptText);
    }
}