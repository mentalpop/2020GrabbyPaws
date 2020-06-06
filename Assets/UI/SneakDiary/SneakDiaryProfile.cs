using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class SneakDiaryProfile : MonoBehaviour
{
    public int faceRightCount = 3;
    public int numTimeIntervals = 8;
    public GameObject timeSlotPrefab;
    public float timeSlotWidth = 80f;
    public Transform timeSlotTransform;
    public RaccoonProfileImage raccoonProfileImage;

    public Image badgeImage;
    public Sprite spriteBadgeComplete;
    
    private NPCProfileUIData profileData;
    private SneakDiary sneakDiaryRef;
    private bool dataHasBeenUnpacked = false;

    public void Unpack(NPCProfileUIData _profileData, SneakDiary _sneakDiaryRef) {
        dataHasBeenUnpacked = true;
        sneakDiaryRef = _sneakDiaryRef;
        profileData = _profileData;
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
    }

    public void PopulateTimeIntervals() {
    //Clear all time intervals first, then populate new ones
        foreach(Transform child in timeSlotTransform)
			Destroy(child.gameObject);
        for (int i = 0; i < numTimeIntervals; i++) {
            TimeIntervalData timeIntervalData = profileData.nightPhases[(int)sneakDiaryRef.nightPhase].intervals[i];
    //If the flags set on the time interval match the current flags on the Quest
            if (timeIntervalData != null && (timeIntervalData.questState.HasFlag(QuestLog.GetQuestState(timeIntervalData.questName.ToString())))) {
                GameObject newGO = Instantiate(timeSlotPrefab, timeSlotTransform, false);
                newGO.transform.localPosition = new Vector2(newGO.transform.localPosition.x + i * timeSlotWidth, newGO.transform.localPosition.y);
                TimeInterval timeInterval = newGO.GetComponent<TimeInterval>();
                timeInterval.Unpack(timeIntervalData, sneakDiaryRef, i > faceRightCount);
            }
        }
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