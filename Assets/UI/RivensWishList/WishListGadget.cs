using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WishListGadget : MonoBehaviour
{
    public MenuNode listNode;
    public MenuNode buttonNode;
	public ListElement listElement;
    public TextMeshProUGUI gadgetName;
    public TextMeshProUGUI gadgetNameFocus;
    public TextMeshProUGUI description;
	public Image bluePrintImage;
	public NavButton buildButton;
	public WishListScrapList wishListScrapList;
	public GameObject checkMark;
	public GameObject scrapPartsContainer;
    public TextMeshProUGUI textUnavailable;
	public TextMeshProUGUI textUnavailableFocus;

	private int gadgetIndex;
	private GadgetData gadgetData;
	private WishListWindow wishListWindow;
	private Inventory inventory;
	private bool canBuildGadget = true;
	private bool unlocked = false;
    
	public ConfirmationPromptData promptGadget;
	private ConfirmationWindow confirmationWindow;
	private bool awaitingConfirmation = false;
	private RivensWishListMenuNodeList wishList;
	
	public void Unpack(int _gadgetIndex, WishListWindow _wishListWindow, MenuNode rivensWishListMenuNodeList) {
		gadgetIndex = _gadgetIndex;
		wishList = rivensWishListMenuNodeList as RivensWishListMenuNodeList;
        wishList.listController.OnSelect += ListController_OnSelect;
        listNode.mCancel = rivensWishListMenuNodeList; //Pass along reference to Cancel node
		buttonNode.mCancel = rivensWishListMenuNodeList; //Pass along reference to Cancel node
		inventory = Inventory.instance;
		gadgetData = inventory.gadgetList.gadgets[gadgetIndex];
		EvaluateCanBuild();
		wishListWindow = _wishListWindow;
		gadgetName.text = gadgetData.gadgetName;
		gadgetNameFocus.text = gadgetData.gadgetName;
		description.text = gadgetData.description;
		bluePrintImage.sprite = gadgetData.blueprintSprite;
		wishListScrapList.Unpack(gadgetData.items);
	}

	private void OnEnable() {
        buildButton.OnSelect += BuildButton_OnSelect;
		/*
		if (wishListWindow != null)
			EvaluateCanBuild();
		//*/
	}

	private void OnDisable() {
		buildButton.OnSelect -= BuildButton_OnSelect;
		if (awaitingConfirmation) {
			awaitingConfirmation = false;
			confirmationWindow.OnChoiceMade -= OnConfirm;
		}
		wishList.listController.OnSelect -= ListController_OnSelect;
	}

	private void ListController_OnSelect(int index) {
        //Debug.Log("listElement.listIndex: "+listElement.listIndex);
        if (index == listElement.listIndex && listElement.navButton.Available) {
            if (listNode.validSelection) {
                //NPCListNode.listController.Unfocus();
                MenuNavigator.Instance.MenuFocus(listNode);
            }
        }
    }

    private void BuildButton_OnSelect(ButtonStateData _buttonStateData) {
        if (canBuildGadget) {
	//Make a confirmation request
			confirmationWindow = UI.RequestConfirmation(promptGadget, null); //This is a hack to leave this null, so control is manually reset in OnConfirm
			confirmationWindow.OnChoiceMade += OnConfirm;
			awaitingConfirmation = true;
		}
    }

    public void EvaluateCanBuild() {
//Evaluate canBuildGadget; Iterate throught required scrap items to see if the player has all they need
		unlocked = inventory.gadgetUnlocked[gadgetIndex];
		if (unlocked) {
			canBuildGadget = false;
	//Already Built
			scrapPartsContainer.SetActive(false); //Hide List
			checkMark.SetActive(true); //Show Checkmark
			buildButton.SetAvailable(false); //Set button to Unavailable
			textUnavailable.text = "BUILT"; //Change text to "Built" instead of "Missing Parts"
			textUnavailableFocus.text = "BUILT"; //Change text to "Built" instead of "Missing Parts"
			gadgetName.fontStyle = FontStyles.Strikethrough; //Strike out header
			gadgetNameFocus.fontStyle = FontStyles.Strikethrough; //Strike out header
			listElement.navButton.SetAvailable(false); //Prevent the user from selecting this line item if the item has already been built
		} else {
	//Have the parts to build?
			foreach (var item in gadgetData.items) {
				if ((int)inventory.InventoryCount(item.item.name) < item.quantity) { //Strictly less than = NOT ENOUGH
					canBuildGadget = false;
					break;
				}
			}
			buildButton.SetAvailable(canBuildGadget);
		}
	}

	/*
	private void BuildButton_OnClick(bool _stateActive) {
		if (canBuildGadget) {
	//Make a confirmation request
			confirmationWindow = UI.RequestConfirmation(promptGadget, null);
			confirmationWindow.OnChoiceMade += OnConfirm;
			awaitingConfirmation = true;
		}
	}
	//*/

	private void OnConfirm(bool _choice) {
		awaitingConfirmation = false;
		confirmationWindow.OnChoiceMade -= OnConfirm;
		if (_choice) {
			inventory.gadgetUnlocked[gadgetIndex] = true;
	//Subtract items from Inventory
			foreach (var item in gadgetData.items) {
				inventory.Remove(item.item, item.quantity);
			}
			wishListWindow.Unpack();
		} else {
			Debug.Log("Did not build Gadget");
			MenuNavigator.Instance.MenuFocus(buttonNode); //Reset focus on Button
		}
	}
}