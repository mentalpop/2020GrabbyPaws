using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WishListGadget : MonoBehaviour
{
    public TextMeshProUGUI gadgetName;
    public TextMeshProUGUI description;
	public Image bluePrintImage;
	public ButtonOmni buildButton;
	public WishListScrapList wishListScrapList;
	public GameObject checkMark;
	public GameObject scrapPartsContainer;
    public TextMeshProUGUI textUnavailable;

	private int gadgetIndex;
	private GadgetData gadgetData;
	private WishListWindow wishListWindow;
	private Inventory inventory;
	private bool canBuildGadget = true;
	private bool unlocked = false;
    
	public ConfirmationPromptData promptGadget;
	private ConfirmationWindow confirmationWindow;
	private bool awaitingConfirmation = false;
	
	public void Unpack(int _gadgetIndex, WishListWindow _wishListWindow) {
		gadgetIndex = _gadgetIndex;
		inventory = Inventory.instance;
		gadgetData = inventory.gadgetList.gadgets[gadgetIndex];
		EvaluateCanBuild();
		if (unlocked) {
			scrapPartsContainer.SetActive(false); //Hide List
			checkMark.SetActive(true); //Show Checkmark
			textUnavailable.text = "BUILT"; //Set button to built
			gadgetName.fontStyle = FontStyles.Strikethrough; //Strike out header
		} else if (canBuildGadget) {
			buildButton.available = true;
			buildButton.UpdateGO();
		}
		wishListWindow = _wishListWindow;
		gadgetName.text = gadgetData.gadgetName;
		description.text = gadgetData.description;
		bluePrintImage.sprite = gadgetData.blueprintSprite;
		wishListScrapList.Unpack(gadgetData.items);
	}

	private void OnEnable() {
		buildButton.OnClick += BuildButton_OnClick;
		if (wishListWindow != null)
			EvaluateCanBuild();
	}

	public void EvaluateCanBuild() {
//Evaluate canBuildGadget; Iterate throught required scrap items to see if the player has all they need
		unlocked = inventory.gadgetUnlocked[gadgetIndex];
		if (unlocked) {
			canBuildGadget = false;
		} else {
			foreach (var item in gadgetData.items) {
				if ((int)inventory.InventoryCount(item.item.name) <= item.quantity) {
					canBuildGadget = false;
					break;
				}
			}
		}
		if (!canBuildGadget) {
			buildButton.available = false;
			buildButton.UpdateGO();
		}
	}

	private void OnDisable() {
		buildButton.OnClick -= BuildButton_OnClick;
		if (awaitingConfirmation) {
			awaitingConfirmation = false;
			confirmationWindow.OnChoiceMade -= OnConfirm;
		}
	}

	private void BuildButton_OnClick(bool _stateActive) {
		if (canBuildGadget) {
	//Make a confirmation request
			confirmationWindow = UI.RequestConfirmation(promptGadget, null);
			confirmationWindow.OnChoiceMade += OnConfirm;
			awaitingConfirmation = true;
		}
	}

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
		}
	}
}