using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotSecrets : MonoBehaviour
{
	public ListController listController;
	public MenuHub menuHub;
	public MenuNode listMenuNode;
    public ButtonGeneric closeButton;
    public ClickToClose clickToClose;

	public GameObject lineItemPrefab;
	public Transform lineItemTransform;

	private List<NotSecretLineItem> lineItems = new List<NotSecretLineItem>();

	private void OnEnable() {
        clickToClose.OnClick += Close;
		closeButton.OnClick += Close;
        menuHub.OnMenuClose += MenuHub_OnMenuClose;
		SpawnSecrets();
	}

    private void OnDisable() {
        clickToClose.OnClick -= Close;
		closeButton.OnClick -= Close;
        menuHub.OnMenuClose -= MenuHub_OnMenuClose;
	}

    private void MenuHub_OnMenuClose() {
        Close();
    }

	private void Close() {
		gameObject.SetActive(false);
	}

	public void SpawnSecrets() {
		foreach(Transform child in lineItemTransform)
			Destroy(child.gameObject); //These children have only been flagged for deletion, 
										//but won't be removed from the heirarchy until end of frame,
										//this will confuse the foreach (Transform child in lineItemTransform) call
										//when trying to make a list of ListElements that should ACTUALLY exist
		if (lineItems.Count > 0)
			lineItems.Clear();
		foreach (var secret in FlagRepository.instance.secretFlags.secrets) {
			//Debug.Log("secret.secret.ToString(): "+secret.secret.ToString());
			if (FlagRepository.ReadSecretKey(secret.secret.ToString()) != 0) { //If the secret has been discovered
				GameObject newGO = Instantiate(lineItemPrefab, lineItemTransform, false);
				NotSecretLineItem notSecretLineItem = newGO.GetComponent<NotSecretLineItem>();
				notSecretLineItem.Unpack(secret, this);
				lineItems.Add(notSecretLineItem);
			}
		}
		SortListElementsUnderTransform();
		menuHub.menuOnEnable = listMenuNode;
		MenuNavigator.Instance.MenuFocus(listMenuNode);
	}

    public void SortListElementsUnderTransform() {
//Move Stricken items to the bottom of the list
		foreach (var item in lineItems) {
			if (item.isStricken) {
				item.gameObject.transform.SetAsLastSibling();
			}
		}
//Compile the list of elements based on their transform order
        List<ListElement> _elements = new List<ListElement>();
        foreach (Transform child in lineItemTransform) {
			NotSecretLineItem notSecretLineItem = child.GetComponent<NotSecretLineItem>();
			if (lineItems.Contains(notSecretLineItem)) { //This is the most reliable way to ensure that it wasn't an object that was flagged for deletion
														//This is NOT efficient, but Unity has no built-in way of checking this
				ListElement liEl = child.GetComponent<ListElement>();
				_elements.Add(liEl);
            }
        }
		//Debug.Log("SortListElementsUnderTransform _elements.Count: "+_elements.Count);
        listController.Elements = _elements;
    }
}
