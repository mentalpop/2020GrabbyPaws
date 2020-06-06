using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotSecrets : MonoBehaviour
{
    public ButtonGeneric closeButton;
    public ClickToClose clickToClose;

	public GameObject lineItemPrefab;
	public Transform lineItemTransform;

	private void OnEnable() {
        clickToClose.OnClick += Close;
		closeButton.OnClick += Close;
		SpawnSecrets();
	}

	private void OnDisable() {
        clickToClose.OnClick -= Close;
		closeButton.OnClick -= Close;
	}

	private void Close() {
		gameObject.SetActive(false);
	}

	public void SpawnSecrets() {
		foreach(Transform child in lineItemTransform)
			Destroy(child.gameObject);
		List<NotSecretLineItem> lineItems = new List<NotSecretLineItem>();
		foreach (var secret in FlagRepository.instance.secretFlags.secrets) {
			//Debug.Log("secret.secret.ToString(): "+secret.secret.ToString());
			if (FlagRepository.ReadSecretKey(secret.secret.ToString()) != 0) { //If the secret has been discovered
				GameObject newGO = Instantiate(lineItemPrefab, lineItemTransform, false);
				NotSecretLineItem notSecretLineItem = newGO.GetComponent<NotSecretLineItem>();
				notSecretLineItem.Unpack(secret);
				lineItems.Add(notSecretLineItem);
			}
		}
	//Move Stricken items to the bottom of the list
		foreach (var item in lineItems) {
			if (item.isStricken) {
				item.gameObject.transform.SetAsLastSibling();
			}
		}
	}
}
