using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadPromptManager : MonoBehaviour
{
    public bool allowPrompts = false;
    public GameObject prefabPrompt;
    public Sprite promptIgnoredGamepad;
    public string promptTextIgnoredGamepad;
    public Sprite promptDetectedGamepad;
    public string promptTextDetectedGamepad;
    public Sprite promptDetectedMouse;
    public string promptTextDetectedMouse;

    private ControllerPrompt currentPrompt;

    public enum PromptType
    {
        PromptIgnoredGamepad,
        PromptDetectedGamepad,
        PromptDetectedMouse
    }

    public void CreatePrompt(PromptType promptType) {
        if (currentPrompt == null && allowPrompts) {
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
            currentPrompt = newGO.GetComponent<ControllerPrompt>();
            currentPrompt.Unpack(_promptSprite, _promptText);
        }
        if (!allowPrompts) {
            Debug.LogWarning("allowPrompts is disabled on GamepadPromptManager");
        }
    }
}