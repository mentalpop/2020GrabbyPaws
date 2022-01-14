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
    public Sprite promptDetectedGamepad;
    public Sprite promptDetectedMouse;

    public void CreatePrompt(PromptType promptType) {
        GameObject newGO = Instantiate(prefabPrompt, transform, false);
        Sprite _promptSprite = promptIgnoredGamepad;
        switch (promptType) {
            case PromptType.PromptIgnoredGamepad: _promptSprite = promptIgnoredGamepad; break;
            case PromptType.PromptDetectedGamepad: _promptSprite = promptDetectedGamepad; break;
            case PromptType.PromptDetectedMouse: _promptSprite = promptDetectedMouse; break;
        }
        newGO.GetComponent<ControllerPrompt>().Unpack(_promptSprite);
    }
}