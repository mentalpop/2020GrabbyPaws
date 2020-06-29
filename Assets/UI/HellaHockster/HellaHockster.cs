using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HellaHockster : MonoBehaviour
{
    public Inventory inventory;
	public MenuHub menuHub;
	public MenuNode confirmationMenu;

    public ButtonGeneric closeButton;
    public ClickToClose clickToClose;
    public NavButton hocksterCallButton;
	public int availableHocksters = 2;
	public List<GameObject> hocksterImages = new List<GameObject>();
	public HocksterScrollRect inventoryRect;
	public HocksterScrollRect hocksterRect;
	public NavButton dumpTrash;
	public NavButton recoverTrash;
    public TextMeshProUGUI playerFunds;
    public TextMeshProUGUI estimatedValue;

	private List<InventoryItem> playerInventory = new List<InventoryItem>();
	private List<InventoryItem> hocksterInventory = new List<InventoryItem>();

	public ConfirmationPromptData promptData;
	private ConfirmationWindow confirmationWindow;
	private bool awaitingConfirmation = false;

	private void OnEnable() {
		hocksterCallButton.OnSelect += HocksterCallButton_OnClick;
		clickToClose.OnClick += Close;
		closeButton.OnClick += Close;
		dumpTrash.OnSelect += DumpAllTrash;
		recoverTrash.OnSelect += RecoverAllTrash;
        //inventory.OnItemChanged += UpdateDisplay;
		UpdateHockstersAvailable();
	//Player Funds
		Currency.instance.OnCashChanged += CurrencyCashUpdate;
		CurrencyCashUpdate();
	//List setup
		playerInventory.Clear();
		hocksterInventory.Clear();
		foreach (var iItem in inventory.items) {
	//Clone the inventory into a local list
			playerInventory.Add(new InventoryItem(iItem.item, iItem.quantity));
		}
		UpdateDisplay();
        menuHub.OnMenuClose += MenuNavigator_OnClose;
	}

    private void MenuNavigator_OnClose() {
        Close();
    }

    private void CurrencyCashUpdate() {
		playerFunds.text = Currency.instance.Cash.ToString();
	}

	private void OnDisable() {
		hocksterCallButton.OnSelect -= HocksterCallButton_OnClick;
        clickToClose.OnClick -= Close;
        //inventory.OnItemChanged -= UpdateDisplay;
		closeButton.OnClick -= Close;
		dumpTrash.OnSelect -= DumpAllTrash;
		recoverTrash.OnSelect -= RecoverAllTrash;
		Currency.instance.OnCashChanged -= CurrencyCashUpdate;
		if (awaitingConfirmation) {
			awaitingConfirmation = false;
			confirmationWindow.OnChoiceMade -= OnConfirm;
		}
        menuHub.OnMenuClose -= MenuNavigator_OnClose;
	}

	private void Close() {
		gameObject.SetActive(false);
	}

	private void HocksterCallButton_OnClick(ButtonStateData _buttonStateData) {
//SELL ITEMS
		if (_buttonStateData.available) {
			confirmationWindow = UI.RequestConfirmation(promptData, confirmationMenu);
			confirmationWindow.OnChoiceMade += OnConfirm;
			awaitingConfirmation = true;
		} else {
			Debug.Log("No Hocksters available");
		}
	}

	private void OnConfirm(bool _choice) {
		awaitingConfirmation = false;
		confirmationWindow.OnChoiceMade -= OnConfirm;
		if (_choice) {
			availableHocksters--;
			UpdateHockstersAvailable();
			if (availableHocksters < 1) {
				hocksterCallButton.SetAvailable(false);
			}
		//Remove sold items from Inventory
			foreach (var iItem in hocksterInventory) {
				Inventory.instance.Remove(iItem.item, iItem.quantity);
			}
		//Sell items
			Currency.instance.Cash += TallyValue(hocksterInventory);
			hocksterRect.ClearLineItems();
			hocksterInventory.Clear();
			estimatedValue.text = "0";
		} else {
			Debug.Log("Did not call Hockster");
		}
	}

	public void UpdateHockstersAvailable() {
		for (int i = 0; i < hocksterImages.Count; i++) {
			hocksterImages[i].SetActive(i < availableHocksters);
		}
	}
//Inventory Handling
	private void UpdateDisplay() {
		inventoryRect.Unpack(playerInventory, hocksterInventory, this);
		hocksterRect.Unpack(hocksterInventory, playerInventory, this);
	//Tally estimated value
		estimatedValue.text = string.Format("{0:N0}", TallyValue(hocksterInventory));
	}

	public decimal TallyValue(List<InventoryItem> listSource) {
		decimal tally = 0m;
		foreach (var item in listSource) {
			tally += (decimal)item.item.value * item.quantity;
		}
		return tally;
	}

	public void MoveItem(List<InventoryItem> listSource, List<InventoryItem> listDest, Item item, int quantity) {
//Remove an item from this list, move it to the other one
		Add(item, quantity, listDest);
		Remove(item, quantity, listSource);
		UpdateDisplay();
	}

    private void Add(Item item, int quantity, List<InventoryItem> listDest) {
//Check if the item is already in the inventory
        bool foundInDestList = false;
        foreach (var iItem in listDest) {
            if (iItem.item == item) {
        //Add to it's quantity
                iItem.quantity += quantity;
                foundInDestList = true;
            }
        }
        if (!foundInDestList) {
    //if there are none, add to the list instead
            listDest.Add(new InventoryItem(item, quantity));
        }
    }

    private void Remove(Item item, int quantity, List<InventoryItem> listSource) {
//Check if the item is already in the inventory
        foreach (var iItem in listSource) {
            if (iItem.item == item) {
        //Remove quantity from it's quantity
                iItem.quantity -= quantity; //Since Add and Remove are always called in tandem, this method trusts that quantity is "reasonable"
                if (iItem.quantity < 1) {
            //If you have none left, actually remove the item
                    listSource.Remove(iItem);
                }
                break;
            }
        }
    }

	public void DumpAllTrash(ButtonStateData _buttonStateData) {
//Move ALL items out of list
		foreach (var item in playerInventory) {
			Add(item.item, item.quantity, hocksterInventory);
		}
		playerInventory.Clear();
		UpdateDisplay();
	}

	public void RecoverAllTrash(ButtonStateData _buttonStateData) {
//Move ALL items out of list
		foreach (var item in hocksterInventory) {
			Add(item.item, item.quantity, playerInventory);
		}
		hocksterInventory.Clear();
		UpdateDisplay();
	}
}