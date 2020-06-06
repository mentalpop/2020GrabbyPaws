using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishListWindow : MonoBehaviour
{
    public GameObject gadgetPrefab;
    public Transform contentTransform;
    public ButtonGeneric closeButton;
    public ClickToClose clickToClose;

    private void Awake() {
        Unpack();
    }

    public void Unpack() {
    //Clear gadgets
        foreach (Transform child in contentTransform) {
            Destroy(child.gameObject);
        }
        int i = 0;
        foreach (var gadget in Inventory.instance.gadgetList.gadgets) {
            GameObject newGO = Instantiate(gadgetPrefab, contentTransform, false);
            WishListGadget wishList = newGO.GetComponent<WishListGadget>();
            wishList.Unpack(i++, this);
        }
    }

	private void OnEnable() {
		closeButton.OnClick += Close;
		clickToClose.OnClick += Close;
	}

	private void OnDisable() {
		closeButton.OnClick -= Close;
		clickToClose.OnClick -= Close;
	}
    
	private void Close() {
		gameObject.SetActive(false);
	}
}