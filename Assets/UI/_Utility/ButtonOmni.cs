using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOmni : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject goDefaultNeutral;
    public GameObject goDefaultHover;
    public GameObject goDefaultSelected;
	public bool hasToggleState = false;
    public bool stateActive = false; //If this button has a toggled state, should it use that sprite set?
    public GameObject goToggleNeutral;
    public GameObject goToggleHover;
    public GameObject goToggleSelected;
    public bool available = true;
    public GameObject goUnavailable;
    [HideInInspector] public bool alwaysHover = false;

    private bool mouseClick = false;
    private bool mouseOver = false;

    public delegate void ButtonEvent (bool _stateActive);
	public event ButtonEvent OnClick = delegate { };

    public void UpdateGO() {
		GameObject setThisActive;
        if (available) {
            if (mouseClick) {
                setThisActive = stateActive ? goToggleSelected : goDefaultSelected; //Selected
            } else {
                if (mouseOver || alwaysHover) {
                    setThisActive = stateActive ? goToggleHover : goDefaultHover; //Hover
                } else {
                    setThisActive = stateActive ? goToggleNeutral : goDefaultNeutral; //Neutral
                }
            }
        } else {
            setThisActive = goUnavailable;
        }
    //Disable all GameObjects except the right one
        if (goDefaultNeutral != null) goDefaultNeutral.SetActive(false);
        if (goDefaultHover != null) goDefaultHover.SetActive(false);
        if (goDefaultSelected != null) goDefaultSelected.SetActive(false);
        if (goToggleNeutral != null) goToggleNeutral.SetActive(false);
        if (goToggleHover != null) goToggleHover.SetActive(false);
        if (goToggleSelected != null) goToggleSelected.SetActive(false);
        if (goUnavailable != null) goUnavailable.SetActive(false);
        if (setThisActive != null) setThisActive.SetActive(true);
	}
		
	public void OnPointerEnter(PointerEventData evd) {
        mouseOver = true;
        UpdateGO();
    }

	public void OnPointerExit (PointerEventData evd) {
        mouseOver = false;
        UpdateGO();
    }
		
	public void OnPointerClick (PointerEventData evd) {
        if (hasToggleState)
            stateActive = !stateActive;
        UpdateGO();
		OnClick?.Invoke(stateActive);
	}

	public void OnPointerDown(PointerEventData eventData) {
        mouseClick = true;
        UpdateGO();
    }

    public void OnPointerUp(PointerEventData eventData) {
        mouseClick = false;
        UpdateGO();
    }
}