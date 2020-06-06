using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class TabSortItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler 
{
    //public CanvasGroup canvasGroup;
	public Image myIconImage;
	public Image background;
	//public Image myHighlightImage;
    public TextMeshProUGUI title;
    public bool ignoreContentActive = false;
    //public bool doSwapSprite = true;
//Set by Menu
	[HideInInspector] public bool contentActive = false;
    [HideInInspector] public TabSortMenu myParentMenu; 
    [HideInInspector] public int tabID;
    private TabData tabData;

    /*
    public void InsertSeparator(GameObject separator) {
//Instantiate a separator as 1 index ahead of yourself
        GameObject newSeperator = Instantiate(separator, transform.parent, false);
        newSeperator.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
    }
    //*/

    public void UnpackData(TabData _tabData, TabSortMenu _TabSortMenu, int _tabID) {
        tabData = _tabData;
        myIconImage.sprite = _tabData.icon;
        background.color = _tabData.bgColor;
        myIconImage.SetNativeSize();
        if (title != null) {
            title.text = _tabData.text;
        }
        myParentMenu = _TabSortMenu;
        tabID = _tabID;
    }
    
    /*
    public void SetOpacity() {
        if (mouseOver) {
            canvasGroup.alpha = 1f;
        } else if (!contentActive) {
            canvasGroup.alpha = 0.5f;
        }
        if (doSwapSprite) {
    //Swap the active sprite
            myHighlightImage.sprite = contentActive ? myParentMenu.spriteActive : mouseOver ? myParentMenu.spriteMOver : null;
            myHighlightImage.color = myHighlightImage.sprite == null ? new Color(0f, 0f, 0f, 0f) : Color.white;
        } else {
    //Toggle the alpha of the sprite
            myHighlightImage.sprite = contentActive ? myParentMenu.spriteActive : null;
            //myHighlightImage.color = contentActive ? new Color(0f, 0f, 0f, 0f) : Color.white;
        }
    }
    //*/

    public void OnPointerEnter(PointerEventData evd) {
		if (ignoreContentActive || !contentActive)
            background.color = tabData.bgColorHighlight;
	}

	public void OnPointerExit (PointerEventData evd) {
		if (ignoreContentActive || !contentActive)
            background.color = tabData.bgColor;
	}
		
	public void OnPointerClick (PointerEventData evd) {
		if (ignoreContentActive || !contentActive) {
		    background.color = tabData.bgColor;
            myParentMenu.TabSelect(tabID);
        }
	}
}