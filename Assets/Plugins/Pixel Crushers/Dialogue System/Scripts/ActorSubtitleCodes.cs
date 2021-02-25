using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Add this script to a character's GameObject to add rich text codes to the
/// front and back of the character's subtitle text -- for example to set a
/// color or alignment.
/// </summary>
public class ActorSubtitleCodes : MonoBehaviour
{
    public string frontCode;
    public string backCode;

    void OnConversationLine(Subtitle subtitle)
    {
        if (subtitle.speakerInfo.transform == this.transform)
        {
            subtitle.formattedText.text = frontCode + subtitle.formattedText.text + backCode;
        }
    }
}
