using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListControllerDropDown : ListController
{
    public NavButton header;
    public GameObject background;
    public Transform parentTransform;
    public Transform masterContainer;
    public Sine mySine;
    public float elementHeight = 35f;
    public VerticalLayoutGroup layoutGroup;

    public Transform targetTransform;
    public GameObject listObject;
    public List<NavButtonData> buttonData = new List<NavButtonData>();

    private bool isExpanded = false;
    private RectTransform myRect;
    private Vector2 originPosition;
    private RectTransform backgroundRect;
    private float widthRetracted;
    private Vector2 sizeRetracted;
    private Vector2 sizeExpanded;
    private bool doChangePosition = false;
    private float deltaHeight = 0f;

    private void OnEnable() {
        header.OnSelect += Header_OnSelect;
    }

    private void OnDisable() {
        header.OnSelect -= Header_OnSelect;
    }

    void Start() {
    //Populate Header Button
        myRect = GetComponent<RectTransform>();
    //Instantiate Options
        if (buttonData.Count == 0) {
            throw new System.Exception("optionDataList.Count is zero!");
        }
    //Instantite Option elements
        List<ListElement> _elements = new List<ListElement>();
        for (int i = 0; i < buttonData.Count; i++) {
            var buttonClone = Instantiate(listObject, targetTransform, false);
            RectTransform nORect = buttonClone.GetComponent<RectTransform>();
            nORect.sizeDelta = new Vector2(myRect.sizeDelta.x, nORect.sizeDelta.y);
            ListElement liEl = buttonClone.GetComponent<ListElement>();
            _elements.Add(liEl);
            liEl.navButton.Unpack(buttonData[i]);
        }
        Elements = _elements;
        SetHeader(activeIndex);
    //Handle Background height
        backgroundRect = background.GetComponent<RectTransform>();
        sizeRetracted = backgroundRect.rect.size;
        widthRetracted = backgroundRect.sizeDelta.x;
        originPosition = myRect.anchoredPosition;
        SetDeltaHeight();
        background.SetActive(false);
    //layoutGroup
        layoutGroup.padding.top = (int)elementHeight;
	}

    private void Update() {
    //Handle click events
        /* Close the list without making a selection
        if (isExpanded && !mouseIsOver && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))) {
            Toggle(-1);
        }
        //*/
    //Effect handling
        if (!mySine.durationElapsed) {
            if (isExpanded) {
                mySine.Increment();
            } else {
                mySine.Decrement();
            }
            if (mySine.durationElapsed) {
                background.SetActive(isExpanded);
                //scrollBar.SetActive(isExpanded); //Unhide the Bar graphic
                backgroundRect.sizeDelta = isExpanded ? sizeExpanded : sizeRetracted;
                if (doChangePosition)
                    myRect.anchoredPosition = isExpanded ? new Vector2(originPosition.x, originPosition.y - deltaHeight) : originPosition;
            } else {
                if (doChangePosition)
                    myRect.anchoredPosition = new Vector2(originPosition.x, originPosition.y - deltaHeight * mySine.GetSine()); //Handle sizing
                backgroundRect.sizeDelta = new Vector2(sizeExpanded.x, sizeRetracted.y + (sizeExpanded.y - sizeRetracted.y) * mySine.GetSine());
            }
        }
    }

    private void Header_OnSelect(ButtonStateData _buttonStateData) {
        Toggle(-1); //Open
    }

    public override void SetActiveIndex(int _index) {
        base.SetActiveIndex(_index);
        Toggle(_index);
    }

    public void SetHeader(int _index) {
		header.Unpack(buttonData[_index]);
    }

    private void SetDeltaHeight() {
        //Debug.Log("initialHeight" + ": " + initialHeight);
        float heightOfListElements = elementHeight * (buttonData.Count - 1);
        float expandedHeight = heightOfListElements + backgroundRect.sizeDelta.y;
        //Debug.Log("expandedHeight" + ": " + expandedHeight);
        float screenHeight = ScreenSpace.Height;
        if (expandedHeight > screenHeight) {
            expandedHeight = screenHeight;
        }
        sizeExpanded = new Vector2(widthRetracted, expandedHeight);
        doChangePosition = false;
        deltaHeight = 0f;
    }

    public void Toggle(int _newIndex) {
        Debug.Log("_newIndex: "+_newIndex);
        isExpanded = !isExpanded;
        background.SetActive(true);
        //scrollBar.SetActive(false); //Hide the Bar graphic
        if (isExpanded) {
    //OnOpen
            mySine.Reset();
            foreach (ListElement liEl in Elements) {
                if (liEl.listIndex == activeIndex)
                    liEl.gameObject.SetActive(false);
                else
                    liEl.gameObject.SetActive(true);
            }
            SetDeltaHeight();
            header.SetActive(true);
            if (masterContainer != null) {
                transform.SetParent(masterContainer);
            }
        } else {
    //OnClose
            mySine.Max();
            if (_newIndex > -1) {
                activeIndex = _newIndex;
                //OnSelectEvent(activeIndex);
                SetHeader(activeIndex);
            }
            header.SetActive(false);
            if (parentTransform != null) {
                transform.SetParent(parentTransform);
                myRect.anchoredPosition = originPosition;
            }
        }
    }
}
