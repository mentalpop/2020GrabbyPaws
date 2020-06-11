using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FontHandler : MonoBehaviour
{
[Header("Fonts")]
    public bool handleFontChange = true;
    public FontChoicesData fontData;
    private TMP_FontAsset initialFont;
[Header("Text Size")]
    public bool handleFontResize = false;
    public float fSizeMax = 40f;
    private float fSizeInitial;

    private UI UIRef;
	private TextMeshProUGUI myTMPElement;

	private void Awake() {
        UIRef = UI.Instance;
        //UIRef = FindObjectOfType<UI>();
        myTMPElement = gameObject.GetComponent<TextMeshProUGUI>();
        initialFont = myTMPElement.font;
        fSizeInitial = myTMPElement.fontSize;
    }
        
    void OnEnable() {
        if (handleFontResize) {
            UIRef.OnTextScaled += UIRef_OnTextScaled;
            UIRef_OnTextScaled(UIRef.fontScale);
        }
        if (handleFontChange) {
            UIRef.OnFontChoice += UIRef_OnFontChoice;
            UIRef_OnFontChoice(UIRef.fontChoice);
        }
    }

    void OnDisable() {
        if (handleFontResize)
            UIRef.OnTextScaled -= UIRef_OnTextScaled;
        if (handleFontChange)
            UIRef.OnFontChoice -= UIRef_OnFontChoice;
    }

    private void UIRef_OnTextScaled(float _fontScale) {
       /*
        //float _scale = UIRef.fontScale;
        float _newFontSize = fSizeInitial;
    //The original number, plus the difference between them, with the difference multiplied by the percent below or above 1f
        if (_fontScale < 1f) {
            _newFontSize = fSizeMin + (fSizeInitial - fSizeMin) * (_fontScale / 1f);
        } else if (_fontScale > 1f) {
            _newFontSize = fSizeInitial + (fSizeMax - fSizeInitial) * ((_fontScale - 1f) / 1f);
        }
       //*/
        myTMPElement.fontSize = fSizeInitial + (fSizeMax - fSizeInitial) * ((_fontScale - 1f) / 1f); //_newFontSize
    }

    private void UIRef_OnFontChoice(int fontChoice) {
        myTMPElement.font = fontChoice == 0 ? initialFont : fontData.fonts[fontChoice - 1];
    }
}