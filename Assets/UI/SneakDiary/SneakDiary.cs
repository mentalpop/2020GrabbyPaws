using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SneakDiary : MonoBehaviour
{
    public ListController listController;
	public MenuHub menuHub;
	public MenuNode NPCListMenuNode;
    public ButtonGeneric closeButton;
    public ClickToClose clickToClose;

	public ListControllerDropDown timeSelector;
	public Image timeOfNightIndicator;
	public List<Sprite> nightPhaseSprites = new List<Sprite>();
	
    public GameObject toolTipPrefab;
    public GameObject toolTipExpandedPrefab;

	[Range(0, 7)]
	public int timeInterval = 0;
	public float timelineSegmentWidth = 80f;
	public GameObject timelineStick;
	
    public NightPhases nightPhase = NightPhases.p1Twilight;
	public GameObject npcProfilePrefab;
    public Transform profileListTransform;
	public NPCListData npcList;

	//[HideInInspector] public string[] quests;
	private Vector2 timelineStickOrigin;

	private void OnEnable() {
        clickToClose.OnClick += Close;
		closeButton.OnClick += Close;
		timeSelector.OnSelect += TimeSelector_OnChoiceMade;
        menuHub.OnMenuClose += MenuHub_OnMenuClose;
		Unpack();
	}

	private void OnDisable() {
        clickToClose.OnClick -= Close;
		closeButton.OnClick -= Close;
		timeSelector.OnSelect -= TimeSelector_OnChoiceMade;
        menuHub.OnMenuClose -= MenuHub_OnMenuClose;
	}

    private void MenuHub_OnMenuClose() {
        Close();
    }

    private void TimeSelector_OnChoiceMade(int choiceMade) {
		timeOfNightIndicator.sprite = nightPhaseSprites[choiceMade];
		nightPhase = (NightPhases)choiceMade;
		Unpack();
	}

	private void Close() {
		gameObject.SetActive(false);
	}

	//private void Start() {
	//Debug
		//FlagRepository.WriteQuestKey(QuestNames.q001TwilightCottonCandyEndFlag.ToString(), true);
		
	//Profiles will need access to the list of Quests, so collect them here for cached reference
        /*
        quests = QuestLog.GetAllQuests(QuestState.Abandoned | QuestState.Active | QuestState.Failure | QuestState.Success | QuestState.Unassigned, false);
        Debug.Log("# quests found by Sneak Diary: "+quests.Length);
		foreach (var quest in quests) {
            Debug.Log("quest: "+quest);
        }
		//*/
	//}

	public void Unpack() {
		Debug.Log("Sneak Diary List Repopulated");
		foreach (Transform child in profileListTransform) {
			Destroy(child.gameObject);
		}
	//Spawn list of NPCs
        List<ListElement> _elements = new List<ListElement>();
		foreach (var npc in npcList.npcList) {
			GameObject npcProfile = Instantiate(npcProfilePrefab, profileListTransform, false);
			SneakDiaryProfile sneakDiaryProfile = npcProfile.GetComponent<SneakDiaryProfile>();
			ListElement liEl = npcProfile.GetComponent<ListElement>();
            _elements.Add(liEl);
			sneakDiaryProfile.Unpack(npc, this, NPCListMenuNode);
		}
		listController.Elements = _elements;
		menuHub.menuOnEnable = NPCListMenuNode;
		MenuNavigator.Instance.MenuFocus(NPCListMenuNode);
	//Timeline Stick
		timelineStickOrigin = timelineStick.transform.position;
	}

	private void Update() {
		timelineStick.transform.position = new Vector2(timelineStickOrigin.x + ScreenSpace.Convert(timelineSegmentWidth) * timeInterval, timelineStickOrigin.y);
	}

	public TooltipSmall TooltipOpenSmall(string text, bool faceLeft) {
		GameObject newTooltip = Instantiate(toolTipPrefab, transform, false);
		TooltipSmall ttS = newTooltip.GetComponent<TooltipSmall>();
		ttS.Unpack(text, faceLeft);
		return ttS;
	}

	public TooltipTextContainer TooltipOpenLarge(TimeIntervalData timeIntervalData, bool faceLeft) {
		GameObject newTooltip = Instantiate(toolTipExpandedPrefab, transform, false);
		TooltipTextContainer ttC = newTooltip.GetComponent<TooltipTextContainer>();
		ttC.Unpack(timeIntervalData, faceLeft);
		return ttC;
	}
}