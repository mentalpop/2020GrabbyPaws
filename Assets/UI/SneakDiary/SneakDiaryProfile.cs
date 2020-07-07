using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class SneakDiaryProfile : MonoBehaviour
{
    public MenuNode timeIntervalListNode;
    public NavButton navButton;
    public ListController listController;
    public ListElement listElement;
    public int faceRightCount = 3;
    public int numTimeIntervals = 8;
    public GameObject timeSlotPrefab;
    public float timeSlotWidth = 80f;
    public Transform timeSlotTransform;
    public RaccoonProfileImage raccoonProfileImage;

    public Image badgeImage;
    public Sprite spriteBadgeComplete;

    private MenuNodeSneakDiaryList NPCListNode;
    private NPCProfileUIData profileData;
    private SneakDiary sneakDiaryRef;
    private bool dataHasBeenUnpacked = false;

    public void Unpack(NPCProfileUIData _profileData, SneakDiary _sneakDiaryRef, MenuNode _NPCListNode) {
        dataHasBeenUnpacked = true;
        sneakDiaryRef = _sneakDiaryRef;
        profileData = _profileData;
        NPCListNode = _NPCListNode as MenuNodeSneakDiaryList;
        NPCListNode.listController.OnSelect += ListController_OnSelect;
        timeIntervalListNode.mCancel = _NPCListNode; //Pass along reference to Cancel node
//The profile Image
        raccoonProfileImage.Unpack(profileData, sneakDiaryRef);
//The Time Intervals
        PopulateTimeIntervals();
    //The Completion Badge
        EvaluateQuestCompletion();
    }

    private void OnEnable() {
        if (dataHasBeenUnpacked) {
            PopulateTimeIntervals();
    //The Completion Badge
            EvaluateQuestCompletion();
        }
        //navButton.OnSelect += NavButton_OnSelect;
        navButton.OnFocusGain += NavButton_OnFocusGain;
        navButton.OnFocusLost += NavButton_OnFocusLost;
        listController.OnSelect += ListController_OnSelect;
    }

    private void ListController_OnSelect(int index) {
        //Debug.Log("listElement.listIndex: "+listElement.listIndex);
        if (index == listElement.listIndex) {
            if (timeIntervalListNode.validSelection) {
                //NPCListNode.listController.Unfocus();
                MenuNavigator.Instance.MenuFocus(timeIntervalListNode);
            }
        }
    }

    private void OnDisable() {
        //navButton.OnSelect -= NavButton_OnSelect;
        navButton.OnFocusGain -= NavButton_OnFocusGain;
        navButton.OnFocusLost -= NavButton_OnFocusLost;
        NPCListNode.listController.OnSelect -= ListController_OnSelect;
        raccoonProfileImage.LoseFocus();
    }

    private void NavButton_OnFocusLost(ButtonStateData _buttonStateData) {
        raccoonProfileImage.LoseFocus();
    }

    private void NavButton_OnFocusGain(ButtonStateData _buttonStateData) {
        raccoonProfileImage.GainFocus();
    }

    /*
    private void NavButton_OnSelect(ButtonStateData _buttonStateData) {
        
    }
    //*/

    public void PopulateTimeIntervals() {
    //Clear all time intervals first, then populate new ones
        foreach(Transform child in timeSlotTransform)
			Destroy(child.gameObject);
        List<ListElement> _elements = new List<ListElement>();
        for (int i = 0; i < numTimeIntervals; i++) {
            TimeIntervalData timeIntervalData = profileData.nightPhases[(int)sneakDiaryRef.nightPhase].intervals[i];
    //If the flags set on the time interval match the current flags on the Quest
            if (timeIntervalData != null && (timeIntervalData.questState.HasFlag(QuestLog.GetQuestState(timeIntervalData.questName.ToString())))) {
                GameObject newGO = Instantiate(timeSlotPrefab, timeSlotTransform, false);
                newGO.transform.localPosition = new Vector2(newGO.transform.localPosition.x + i * timeSlotWidth, newGO.transform.localPosition.y);
                TimeInterval timeInterval = newGO.GetComponent<TimeInterval>();
                ListElement liEl = newGO.GetComponent<ListElement>();
                _elements.Add(liEl);
                timeInterval.Unpack(timeIntervalData, sneakDiaryRef, i > faceRightCount);
            }
        }
        listController.Elements = _elements;
    }

    public void EvaluateQuestCompletion() {
        bool allTrue = true;
        foreach (var quest in profileData.quests) {
            if (!FlagRepository.ReadQuestKey(quest.ToString())) {
                allTrue = false;
                break;
            }
        }
        if (allTrue) {
            badgeImage.sprite = spriteBadgeComplete;
        }
    }
}