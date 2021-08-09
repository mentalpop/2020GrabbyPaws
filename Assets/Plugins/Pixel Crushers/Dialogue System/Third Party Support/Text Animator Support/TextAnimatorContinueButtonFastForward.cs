// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This script replaces the normal continue button functionality with
    /// a two-stage process. If Text Animator's typewriter is still playing, it
    /// simply fast forwards to the end. Otherwise it sends OnContinue to the UI.
    /// </summary>
    public class TextAnimatorContinueButtonFastForward : MonoBehaviour
    {

        [Tooltip("Dialogue UI that the continue button affects.")]
        public StandardDialogueUI dialogueUI;

        [Tooltip("Text Animator Player to fast forward if it's not done playing.")]
        public Febucci.UI.TextAnimatorPlayer textAnimatorPlayer;

        [Tooltip("Hide the continue button when continuing.")]
        public bool hideContinueButtonOnContinue = false;

        private UnityEngine.UI.Button continueButton;

        private AbstractDialogueUI m_runtimeDialogueUI;
        private AbstractDialogueUI runtimeDialogueUI
        {
            get
            {
                if (m_runtimeDialogueUI == null)
                {
                    m_runtimeDialogueUI = dialogueUI;
                    if (m_runtimeDialogueUI == null)
                    {
                        m_runtimeDialogueUI = GetComponentInParent<AbstractDialogueUI>();
                        if (m_runtimeDialogueUI == null)
                        {
                            m_runtimeDialogueUI = DialogueManager.dialogueUI as AbstractDialogueUI;
                        }
                    }
                }
                return m_runtimeDialogueUI;
            }
        }

        public virtual void Awake()
        {
            continueButton = GetComponent<UnityEngine.UI.Button>();
        }

        public virtual void OnFastForward()
        {
            if (textAnimatorPlayer != null && !textAnimatorPlayer.textAnimator.allLettersShown)
            {
                textAnimatorPlayer.SkipTypewriter();
            }
            else
            {
                if (hideContinueButtonOnContinue && continueButton != null) continueButton.gameObject.SetActive(false);
                if (runtimeDialogueUI != null) runtimeDialogueUI.OnContinue();
            }
        }

    }
}
