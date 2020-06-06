using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class RaccoonProfileImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image myImage;
    public Vector2 tooltipOffset;

    [HideInInspector] public SneakDiary sneakDiaryRef;
    [HideInInspector] public NPCProfileUIData profileData;

    private TooltipSmall tooltip;

    public void Unpack(NPCProfileUIData _profileData, SneakDiary _sneakDiaryRef) {
        sneakDiaryRef = _sneakDiaryRef;
        profileData = _profileData;
        myImage.sprite = profileData.sprite;
    }

    private void FixedUpdate() {
//Match position of Tooltips
        if (tooltip != null) {
            CorrectTransformPosition(tooltip.transform, tooltip.myRect);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (tooltip == null) {
            tooltip = sneakDiaryRef.TooltipOpenSmall(profileData.rName, false);
            CorrectTransformPosition(tooltip.transform, tooltip.myRect);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (tooltip != null) {
            Destroy(tooltip.gameObject);
            tooltip = null;
        }
    }
    private void CorrectTransformPosition(Transform _tooltip, RectTransform _tRect) {
        _tooltip.SetParent(transform);
        _tRect.anchoredPosition = tooltipOffset;
        _tooltip.SetParent(sneakDiaryRef.transform);
    }
}