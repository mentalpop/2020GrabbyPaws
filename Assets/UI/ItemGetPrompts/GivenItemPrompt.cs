using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GivenItemPrompt : MonoBehaviour
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
    public BTweenScale scaleDown;

    private bool closing = false;
    //private Coroutine myCoroutine;
    private GameObject model;
    private Quaternion initialRotation;

    private void OnEnable() {
        btween.OnEndTween += Btween_OnEndTween;
    }

    private void OnDisable() {
        btween.OnEndTween -= Btween_OnEndTween;
    }

    public void Unpack(InventoryItem inventoryItem) {
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
        transform.localScale = Vector3.one; //Something changes the z-depth to zero, so this has to compensate
    }

    void Update() {
        //Close when user clicks button
        if (!UI.LockControls && (Input.GetButtonDown("Steal") || Input.GetMouseButtonDown(0))) {
            Close();
        }
    }

    public void Close() {
        if (!closing) {
            closing = true;
            canvasFadeOut.enabled = true; //Only enable for fade-out
            scaleDown.enabled = true; //Only enable for fade-out
            rectFadeIn.enabled = false; //Disable for fade-out
            btween.PlayFromEnd();
        }
    }

    private void Btween_OnEndTween(float value) {
        if (closing) {
            Destroy(gameObject);
        }
    }
}