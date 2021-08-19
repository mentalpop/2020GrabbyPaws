// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Use this subclass of StandardUISubtitlePanel if your subtitle panel uses
    /// Text Animator and Accumulate Text is ticked.
    /// </summary>
    public class TextAnimatorSubtitlePanel : StandardUISubtitlePanel
    {
        public bool clearTextOnOpen = false;

        public override void Open()
        {
            base.Open();
            if (clearTextOnOpen) ClearText();
        }

        public override void SetContent(Subtitle subtitle)
        {
            if (accumulateText)
            {
                var previousChars = subtitleText.textMeshProUGUI.textInfo.characterCount;
                StartCoroutine(SkipTypewriterAhead(previousChars));
            }
            base.SetContent(subtitle);
        }

        protected IEnumerator SkipTypewriterAhead(int numChars)
        {
            var textAnimator = subtitleText.gameObject.GetComponent<Febucci.UI.TextAnimator>();
            if (textAnimator != null)
            {
                yield return null;
                for (int i = 0; i < numChars; i++)
                {
                    textAnimator.IncreaseVisibleChars();
                }
            }
        }
    }
}
