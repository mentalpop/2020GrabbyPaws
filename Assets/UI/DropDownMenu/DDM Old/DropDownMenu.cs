//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
//using System;

public class DropDownMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	//public Image myIcon;
	public TextMeshProUGUI myTitle;
    //public RectTransform arrowRect;
    //public RectTransform bgImageRect;
    //public ScrollRect scrollRect;
    //public GameObject scrollBar; //Hide the Bar graphic while opening / closing
    public GameObject background;
    public Transform contentTransform;
    public Transform parentTransform;
    public Transform masterContainer;
    public GameObject ddOptionPrefab;
    public GameObject ddSeparatorPrefab;
    public Sine mySine;
    public List<LineItemData> optionDataList = new List<LineItemData>();
    public int chosenIndex = 0;
    //public float widthDelta = 3f; //The width desparity between the DDM and its content (due to drop shadow)
    public float elementHeight = 35f;
    public ThreeStateButton buttonState;
    public VerticalLayoutGroup layoutGroup;
    //public float magicHeightOffset = 17f; //I honestly don't know why this needs to be here, but it has something to do with the offset from the top / bottom of the screen

	private bool isExpanded = false;
    private RectTransform myRect;
    private Vector2 originPosition;
    private RectTransform backgroundRect;
    private float widthRetracted;
    private Vector2 sizeRetracted;
    private Vector2 sizeExpanded;
    private bool doChangePosition = false;
    private float deltaHeight = 0f;
    private List<DropDownOption> ddOptionList = new List<DropDownOption>();
    private bool mouseIsOver = false;
        
	public delegate void DropDownEvent (int choiceMade);
	public event DropDownEvent OnChoiceMade = delegate { };

	void Start() {
    //Populate Header Button
        SetHeader(chosenIndex);
        float myWidth = GetComponent<RectTransform>().sizeDelta.x;// - widthDelta;
    //Instantiate Options
        if (optionDataList.Count == 0) {
            throw new System.Exception("optionDataList.Count is zero!");
        }
        int i = 0;
    //Instantite Option elements
        foreach (LineItemData option in optionDataList) {
            GameObject newOption = Instantiate(ddOptionPrefab, contentTransform, false);
        //Set width of child element
            RectTransform nORect = newOption.GetComponent<RectTransform>();
            nORect.sizeDelta = new Vector2(myWidth, nORect.sizeDelta.y);
        //Pass on Option data
            DropDownOption newDDO = newOption.GetComponent<DropDownOption>();
            newDDO.optionID = i++;
            newDDO.Unpack(option);
            newDDO.dropDownMenu = this;
        //Give each Option a Separator to manage
            if (ddSeparatorPrefab != null) {
                GameObject newSeparator = Instantiate(ddSeparatorPrefab, contentTransform, false);
            //Set width of child element
                RectTransform nOSep = newSeparator.GetComponent<RectTransform>();
                nOSep.sizeDelta = new Vector2(myWidth, nOSep.sizeDelta.y);
                newDDO.mySeparator = newSeparator;
            }
            ddOptionList.Add(newDDO);
        }
    //Handle Background height
        backgroundRect = background.GetComponent<RectTransform>();
        sizeRetracted = backgroundRect.rect.size;
        widthRetracted = backgroundRect.sizeDelta.x;
        myRect = GetComponent<RectTransform>();
        originPosition = myRect.anchoredPosition;
        SetDeltaHeight();
        background.SetActive(false);
    //layoutGroup
        layoutGroup.padding.top = (int)elementHeight;
	}

    private void SetDeltaHeight() {
        //Debug.Log("initialHeight" + ": " + initialHeight);
        float heightOfListElements = elementHeight * (optionDataList.Count - 1);
        float heightOfSeparators = optionDataList.Count - 1;
        float expandedHeight = heightOfListElements + heightOfSeparators + backgroundRect.sizeDelta.y; //backgroundRect.sizeDelta.y
        //Debug.Log("expandedHeight" + ": " + expandedHeight);
        float screenHeight = ScreenSpace.Height;// + magicHeightOffset; //ScreenSpace.Inverse(Screen.height) * (1/UIRef.UIScale);
        //Debug.Log("screenHeight" + ": " + screenHeight);
        if (expandedHeight > screenHeight) {
            expandedHeight = screenHeight;
            //scrollRect.vertical = true; //Enable vertical scrolling if Rect is less than Screen.height
            //Debug.Log("scrollRect.vertical = true");
        } else {
    //Disable the scrollbar
            //scrollRect.verticalScrollbar = null; //Don't need it
            //scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            //scrollRect.viewport.sizeDelta = new Vector2(backgroundRect.sizeDelta.x, scrollRect.viewport.sizeDelta.y);
        }
        sizeExpanded = new Vector2(widthRetracted, expandedHeight);
        //return expandedHeight;
        /*
        Debug.Log("transform.position.y" + ": " + ScreenSpace.Inverse(transform.position.y) * (1f/UIRef.UIScale));
        Debug.Log("sizeExpanded.y" + ": " + sizeExpanded.y);
        Debug.Log("backgroundRect.sizeDelta.y" + ": " + backgroundRect.sizeDelta.y);
        Debug.Log("Bool" + ": " + (ScreenSpace.Inverse(transform.position.y) * (1f/UIRef.UIScale) - sizeExpanded.y + 17f < 0f));
        //*/
        //Debug.Log("transform.position.y" + ": " + ScreenSpace.Inverse(transform.position.y) * (1f/UIRef.UIScale));
        doChangePosition = false;
        deltaHeight = 0f;
        /* Do not resposition
        float yOffset = ScreenSpace.Convert(transform.position.y) - sizeExpanded.y;// + magicHeightOffset;
        if (yOffset < 0f) { //sizeExpanded.y is its total height, if its position - its height is < 0, it would be off the bottom of the screen
            doChangePosition = true;
            deltaHeight = yOffset;
        } else {
            doChangePosition = false;
            deltaHeight = 0f;
        }
        //*/
    }

    private void Update() {
    //Handle click events
        if (isExpanded && !mouseIsOver && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))) {
            Toggle(-1);
        }
    //Effect handling
        if (!mySine.durationElapsed) {
            if (isExpanded) {
                mySine.Increment();
            } else {
                mySine.Decrement();
            }
            if (mySine.durationElapsed) {
                //arrowRect.rotation = Quaternion.Euler(new Vector3(0f, 0f, isExpanded ? 90f : 270f));
                background.SetActive(isExpanded);
                //scrollBar.SetActive(isExpanded); //Unhide the Bar graphic
                backgroundRect.sizeDelta = isExpanded ? sizeExpanded : sizeRetracted;
                if (doChangePosition)
                    myRect.anchoredPosition = isExpanded ? new Vector2(originPosition.x, originPosition.y - deltaHeight) : originPosition;
                if (!isExpanded) {
                    if (parentTransform != null) {
                        transform.SetParent(parentTransform);
                        myRect.anchoredPosition = originPosition;
                    }
                }
            } else {
                if (doChangePosition)
                    myRect.anchoredPosition = new Vector2(originPosition.x, originPosition.y - deltaHeight * mySine.GetSine()); //Handle sizing
                backgroundRect.sizeDelta = new Vector2(sizeExpanded.x, sizeRetracted.y + (sizeExpanded.y - sizeRetracted.y) * mySine.GetSine());
            }
        }
    }

    public void Toggle(int _newIndex) {
        isExpanded = !isExpanded;
        background.SetActive(true);
        //scrollBar.SetActive(false); //Hide the Bar graphic
        if (isExpanded) {
    //OnOpen
            mySine.Reset();
            foreach (DropDownOption option in ddOptionList) {
                if (option.optionID == chosenIndex)
                    option.gameObject.SetActive(false);
                else
                    option.gameObject.SetActive(true);
            }
            SetDeltaHeight();
            buttonState.SetActiveState(true);
            if (masterContainer != null) {
                transform.SetParent(masterContainer);
            }
        } else {
    //OnClose
            mySine.Max();
            if (_newIndex > -1) {
                chosenIndex = _newIndex;
                OnChoiceMade(chosenIndex);
                SetHeader(chosenIndex);
            }
            buttonState.SetActiveState(false);
        }
    }

    public void SetHeader(int _index) {
		myTitle.text = optionDataList[_index].text;
    }

	public void OnPointerEnter(PointerEventData evd) {
		mouseIsOver = true;
	}

	public void OnPointerExit (PointerEventData evd) {
		mouseIsOver = false;
	}
}