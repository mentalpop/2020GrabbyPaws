using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivenChatWindow : MonoBehaviour
{
    public DialogueSystemTrigger dsTrigger;

    // Some of this code saves the chat text between uses so you can restore it 
    // when the conversation starts again.

    // Set this true if you want to restore the previous chat text when restarting
    // the conversation:
    public bool accumulateText = false;

    // We'll save the old chat text in this string variable:
    private string accumulatedText = string.Empty;

    private StandardDialogueUI dialogueUI; // Handy reference to the chat dialogue UI.

    private void Awake()
    {
        dialogueUI = GetComponent<StandardDialogueUI>();
    }

    private void OnEnable()
    {
        dialogueUI.conversationUIElements.mainPanel.onOpen.AddListener(ConversationStarted);
        dialogueUI.conversationUIElements.mainPanel.onClose.AddListener(ConversationEnded);
    }

    private void OnDisable()
    {
        dialogueUI.conversationUIElements.mainPanel.onOpen.RemoveListener(ConversationStarted);
        dialogueUI.conversationUIElements.mainPanel.onClose.RemoveListener(ConversationEnded);
    }

    private void ConversationStarted()
    {
        if (accumulateText)
        {
            // When conversation starts, restore the previously-accumulated text:
            dialogueUI.conversationUIElements.defaultNPCSubtitlePanel.accumulatedText = accumulatedText;
        }
    }

    private void ConversationEnded()
    {
        // When conversation ends, make sure this GameObject gets deactivated:
        gameObject.SetActive(false);
    }

    private void Close()
    {

        if (accumulateText)
        {
            // When conversation ends, record the accumulated text:
            accumulatedText = dialogueUI.conversationUIElements.defaultNPCSubtitlePanel.accumulatedText;
        }

        // When this GameObject is deactivated, make sure the conversation is stopped:
        DialogueManager.StopConversation();

        gameObject.SetActive(false);
    }

    public void TriggerConversation(string conversationID) {
        dsTrigger.conversation = conversationID;
        dsTrigger.OnUse();
    }
}