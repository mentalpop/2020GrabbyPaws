using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishListWindow : MonoBehaviour
{
	public ListController listController;
	public MenuHub menuHub;
	public MenuNode listMenuNode;
	//public MenuNode confirmationMenu;
    public GameObject gadgetPrefab;
    public Transform contentTransform;
    public ButtonGeneric closeButton;
    public ClickToClose clickToClose;

    /*
    private void Awake() {
        Unpack();
    }
    //*/

    public void Unpack() {
    //Clear gadgets
        foreach (Transform child in contentTransform) {
            Destroy(child.gameObject);
        }
        int i = 0;
        List<ListElement> _elements = new List<ListElement>(); //Add each "Gadget" to the list of List Elements
        foreach (var gadget in Inventory.instance.gadgetList.gadgets) {
            GameObject newGO = Instantiate(gadgetPrefab, contentTransform, false);
            //ListElement liEl = newGO.GetComponent<ListElement>();
			_elements.Add(newGO.GetComponent<ListElement>());
            WishListGadget wishList = newGO.GetComponent<WishListGadget>();
            wishList.Unpack(i++, this, listMenuNode);
        }
        listController.Elements = _elements;
		menuHub.menuOnEnable = listMenuNode;
		MenuNavigator.Instance.MenuFocus(listMenuNode);
    }

	private void OnEnable() {
		closeButton.OnClick += Close;
		clickToClose.OnClick += Close;
        menuHub.OnMenuClose += MenuHub_OnMenuClose;
        Unpack();
	}

	private void OnDisable() {
		closeButton.OnClick -= Close;
		clickToClose.OnClick -= Close;
        menuHub.OnMenuClose -= MenuHub_OnMenuClose;
	}

    private void MenuHub_OnMenuClose() {
        Close();
    }
    
	private void Close() {
		gameObject.SetActive(false);
	}
}