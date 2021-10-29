using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DSCanvasManager : MonoBehaviour
{
    public DialogueSystemController dialogueSystemController;
    public Canvas dialogueCanvas;

    public void SetUICamera(Camera _camera) {
        dialogueCanvas.worldCamera = _camera;
    }

}