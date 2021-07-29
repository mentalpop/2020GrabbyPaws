using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StolenItemPrompt : MonoBehaviour
{
    public TextMeshProUGUI tmpItemName;
    public TextMeshProUGUI tmpValue;
    public TextMeshProUGUI tmpSplashMessage;
    public TextMeshProUGUI tmpQuantity;
    public GameObject quantityDisplay;
    public Transform cube;
    public BTween btween;
    public BTweenCanvasGroup canvasFadeOut;
    public BTweenRectAnchor rectFadeIn;
    public float secondsRemainOpen = 2f;

    private bool closing = false;
    private WaitForSeconds shortWait;
    //private Coroutine myCoroutine;
    private GameObject model;
    private Quaternion initialRotation;
    private StolenItemContainer stolenItemContainer;

    private void Awake() {
        shortWait = new WaitForSeconds(secondsRemainOpen);
    }

    private void OnEnable() {
        btween.OnEndTween += Btween_OnEndTween;
    }

    private void OnDisable() {
        btween.OnEndTween -= Btween_OnEndTween;
    }

    public void Unpack(InventoryItem inventoryItem, StolenItemContainer _stolenItemContainer) {
        stolenItemContainer = _stolenItemContainer;
        tmpItemName.text = inventoryItem.item.name;
        tmpValue.text = StaticMethods.ValueFormat(inventoryItem.item.value); //* inventoryItem.quantity
        tmpSplashMessage.text = inventoryItem.item.collectMessage; //BIG STEAL
        float _scale = UI.GetUIScale();
        transform.localScale = new Vector2(_scale, _scale);
    //Item Slot
        if (inventoryItem.quantity > 1) {
            tmpQuantity.text = inventoryItem.quantity.ToString();
        } else {
            quantityDisplay.SetActive(false);
        }
        if (inventoryItem.item.model != null) {
            model = Instantiate(inventoryItem.item.model, cube);
            //Debug.Log("cube transform: " + cube);
            model.transform.localPosition = inventoryItem.item.positionOffset;
            initialRotation = Quaternion.Euler(inventoryItem.item.rotationOffset);
            model.transform.rotation = initialRotation;
            model.transform.localScale = new Vector3(inventoryItem.item.itemScale, inventoryItem.item.itemScale, inventoryItem.item.itemScale);
            //model.layer = 5; //UI
            StaticMethods.SetLayerRecursively(model.transform, 5); //UI
        }
    //Effect
        btween.PlayFromZero();
        StartCoroutine(CloseAfterDelay());
        transform.localScale = Vector3.one; //Something changes the z-depth to zero, so this has to compensate
    }

    private IEnumerator CloseAfterDelay() {
        yield return shortWait;
        Close(); //This could be called early
    }

    public void Close() {
        if (!closing) {
            closing = true;
            canvasFadeOut.enabled = true; //Only enable for fade-out
            rectFadeIn.enabled = false; //Disable for fade-out
            btween.PlayFromEnd();
        }
    }

    private void Btween_OnEndTween(float value) {
        if (closing) {
            stolenItemContainer.RemoveThis(this);
            Destroy(gameObject);
        }
    }
}