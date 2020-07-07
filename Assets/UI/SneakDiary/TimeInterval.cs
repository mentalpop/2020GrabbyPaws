using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TimeInterval : MonoBehaviour
{
    public NavButton navButton;
    public Sprite xSmall;
    public Sprite xLarge;
    public Image xImage;
    public Sine xImageSine;
    public Vector2 tooltipOffset;
    public RectTransform myRect;

    private bool faceLeft = true;
    private SneakDiary sneakDiaryRef;
    private TimeIntervalData timeIntervalData;
    private TooltipSmall tooltip;
    private TooltipTextContainer tooltipLarge;

    private void OnEnable() {
        navButton.OnFocusGain += NavButton_OnFocusGain;
        navButton.OnFocusLost += NavButton_OnFocusLost;
        navButton.OnSelect += NavButton_OnSelect;
    }

    private void OnDisable() {
        navButton.OnFocusGain -= NavButton_OnFocusGain;
        navButton.OnFocusLost -= NavButton_OnFocusLost;
        navButton.OnSelect -= NavButton_OnSelect;
    }

    private void NavButton_OnFocusGain(ButtonStateData _buttonStateData) {
        if (tooltipLarge == null && tooltip == null) {
            tooltip = sneakDiaryRef.TooltipOpenSmall(timeIntervalData.title, faceLeft);
            CorrectTransformPosition(tooltip.transform, tooltip.myRect);
            xImageSine.Reset();
        }
        if (!MenuNavigator.MouseIsUsing()) {
            TooltipExpand();
        }
    }

    private void NavButton_OnFocusLost(ButtonStateData _buttonStateData) {
        if (tooltipLarge != null) {
            Destroy(tooltipLarge.gameObject);
            tooltipLarge = null;
        }
        if (tooltip != null) {
            Destroy(tooltip.gameObject);
            tooltip = null;
        }
    }

    private void NavButton_OnSelect(ButtonStateData _buttonStateData) {
        TooltipExpand();
    }

    public void TooltipExpand() {
        if (tooltipLarge == null) {
            tooltipLarge = sneakDiaryRef.TooltipOpenLarge(timeIntervalData, faceLeft);
            CorrectTransformPosition(tooltipLarge.transform, tooltipLarge.myRect);
            if (tooltip != null) {
                Destroy(tooltip.gameObject);
                tooltip = null;
            }
        }
    }

    public void Unpack(TimeIntervalData _timeIntervalData, SneakDiary _sneakDiaryRef, bool _faceLeft) {
        faceLeft = _faceLeft;
        sneakDiaryRef = _sneakDiaryRef;
        timeIntervalData = _timeIntervalData;
        xImage.sprite = timeIntervalData.isMajorEvent ? xLarge : xSmall; //Set size of X sprite
        xImage.SetNativeSize();
    }
    /*
    
    private void Awake() {
//Convert offset to screen space
        tooltipOffset = new Vector2(ScreenSpace.Convert(tooltipOffset.x), ScreenSpace.Convert(tooltipOffset.y));
    }
    //*/

    private void Update() {
        if (!xImageSine.durationElapsed) {
            xImageSine.Increment();
            if (xImageSine.durationElapsed) {
                xImage.rectTransform.localScale = Vector2.one;
            } else {
                float _val = 1f + xImageSine.GetSineDuration();
                xImage.rectTransform.localScale = new Vector2(_val, _val);
            }
        }
    }

    private void FixedUpdate() {
//Match position of Tooltips
        if (tooltip != null) {
            CorrectTransformPosition(tooltip.transform, tooltip.myRect);
        }
        if (tooltipLarge != null) {
            CorrectTransformPosition(tooltipLarge.transform, tooltipLarge.myRect);
        }
    }

    private void CorrectTransformPosition(Transform _tooltip, RectTransform _tRect) {
        _tooltip.SetParent(transform);
        _tRect.anchoredPosition = tooltipOffset;
        _tooltip.SetParent(sneakDiaryRef.transform);
    }

    /*
    public void OnPointerClick(PointerEventData eventData) {
        if (tooltipLarge == null) {
            tooltipLarge = sneakDiaryRef.TooltipOpenLarge(timeIntervalData, faceLeft);
            CorrectTransformPosition(tooltipLarge.transform, tooltipLarge.myRect);
            if (tooltip != null) {
                Destroy(tooltip.gameObject);
                tooltip = null;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (tooltipLarge == null && tooltip == null) {
            tooltip = sneakDiaryRef.TooltipOpenSmall(timeIntervalData.title, faceLeft);
            CorrectTransformPosition(tooltip.transform, tooltip.myRect);
            xImageSine.Reset();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (tooltipLarge != null) {
            Destroy(tooltipLarge.gameObject);
            tooltipLarge = null;
        }
        if (tooltip != null) {
            Destroy(tooltip.gameObject);
            tooltip = null;
        }
    }
    //*/
}