using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThreeStateButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerClickHandler
{
	public bool hasToggleState = false;
    public bool stateActive = false; //If this button has a toggled state, should it use that sprite set?
    public Image myImage;
    public GameObject onHover; //If this isn't null, it will activate / deactivate the component to show / hide the image
    public Sprite spriteDefaultNeutral;
    public Sprite spriteDefaultHover;
    public Sprite spriteDefaultSelected;
    public Sprite spriteToggleNeutral;
    public Sprite spriteToggleHover;
    public Sprite spriteToggleSelected;
    public Sprite spriteUnavailable;
    public bool doSetNativeSize = true;
    public bool available = true;
    public bool doChangeColor = false;
    public bool onUnavailableChangeColor = false;
    public Color colorNeutral = new Color(1f, 1f, 1f, 0.7f);
    public Color colorHover = new Color(1f, 1f, 1f, 1f);
    public Color colorSelected = new Color(1f, 1f, 1f, 1f);
    public Color colorAvailable = new Color(1f, 1f, 1f, 1f);
    public Color colorUnavailable = new Color(1f, 1f, 1f, 0.3f);
    [HideInInspector] public bool alwaysHover = false;
    private bool mouseClick = false;
    private bool mouseOver = false;

    public void Start() {
//Overwrite null sprites
        if (spriteDefaultNeutral == null) 
            spriteDefaultNeutral = myImage.sprite; //If no sprites are given, pull the sprite from the image component
        if (spriteDefaultHover == null)
            spriteDefaultHover = spriteDefaultNeutral;
        if (spriteDefaultSelected == null)
            spriteDefaultSelected = spriteDefaultNeutral;
    //Overwrite Toggle Sprites
        if (spriteToggleNeutral == null) 
            spriteToggleNeutral = spriteDefaultNeutral;
        if (spriteToggleHover == null)
            spriteToggleHover = spriteToggleNeutral;
        if (spriteToggleSelected == null)
            spriteToggleSelected = spriteToggleNeutral;
        if (spriteUnavailable == null)
            spriteUnavailable = spriteDefaultNeutral;
    //Set Default Color
        if (doChangeColor)
            myImage.color = colorNeutral;
        if (onUnavailableChangeColor)
            myImage.color = available ? colorAvailable : colorUnavailable;
    //Set default Hover Image state
        if (onHover != null)
            onHover.SetActive(false);
    }

    private void OnDisable() {
//Hide Hover GO onDisable
        if (onHover != null)
            onHover.SetActive(false);
    }

    public void SpriteUpdate() {
		Sprite newSprite;
        Color changeTo;
        if (available) {
            if (mouseClick) {
                newSprite = stateActive ? spriteToggleSelected : spriteDefaultSelected; //Selected
                changeTo = colorSelected;
                if (onHover != null)
                    onHover.SetActive(false);
            } else {
                if (mouseOver || alwaysHover) {
                    newSprite = stateActive ? spriteToggleHover : spriteDefaultHover; //Hover
                    changeTo = colorHover;
                    if (onHover != null)
                        onHover.SetActive(true);
                } else {
                    newSprite = stateActive ? spriteToggleNeutral : spriteDefaultNeutral; //Neutral
                    changeTo = colorNeutral;
                    if (onHover != null)
                        onHover.SetActive(false);
                }
            }
        } else {
            newSprite = spriteUnavailable;
            changeTo = colorUnavailable;
        }
        myImage.sprite = null; //REMOVE work-around Unity 2019.3
        myImage.sprite = newSprite;
        if (doChangeColor)
            myImage.color = changeTo;
        if (doSetNativeSize)
            myImage.SetNativeSize();
	}
		
	public void OnPointerEnter(PointerEventData evd) {
        mouseOver = true;
        SpriteUpdate();
        //Debug.Log ("OnPointerEnter");
    }

	public void OnPointerExit (PointerEventData evd) {
        mouseOver = false;
        SpriteUpdate();
        //Debug.Log ("OnPointerExit");
    }
		
    public void OnPress() {
        mouseClick = true;
        SpriteUpdate();
    }

    public void OnRelease() {
        mouseClick = false;
        SpriteUpdate();
    }
    public void SetActiveState(bool _state) {
        stateActive = _state;
        SpriteUpdate();
    }

    public void ToggleActiveState() {
        stateActive = !stateActive;
        SpriteUpdate();
    }

    public void SetAvailable(bool isAvailable) {
        available = isAvailable;
        if (onUnavailableChangeColor)
            myImage.color = available ? colorAvailable : colorUnavailable;
        SpriteUpdate();
    }
    /*
	public void OnPointerClick (PointerEventData evd) {
		//Debug.Log ("OnPointerClick");
	}
    */
}