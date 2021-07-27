using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StolenItemPrompt : MonoBehaviour
{
    public TextMeshProUGUI tmpItemName;
    public TextMeshProUGUI tmpValue;
    public TextMeshProUGUI tmpSplashMessage;
    public BTween btween;
    //public CanvasGroup canvas;
    public float secondsRemainOpen = 2f;

    private bool closing = false;
    private WaitForSeconds shortWait;
    //private Coroutine myCoroutine;

    private void Awake() {
        shortWait = new WaitForSeconds(secondsRemainOpen);
    }

    private void OnEnable() {
        btween.OnEndTween += Btween_OnEndTween;
    }

    private void OnDisable() {
        btween.OnEndTween -= Btween_OnEndTween;
    }


    public void Unpack(InventoryItem inventoryItem) {
        tmpItemName.text = inventoryItem.item.name;
        tmpValue.text = UI.ValueFormat(inventoryItem.item.value); //* inventoryItem.quantity
        tmpSplashMessage.text = inventoryItem.item.collectMessage; //BIG STEAL
        float _scale = UI.GetUIScale();
        transform.localScale = new Vector2(_scale, _scale);
        btween.PlayFromZero();
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay() {
        yield return shortWait;
        Close(); //This could be called early
    }

    public void Close() {
        if (!closing) {
            closing = true;
            btween.PlayFromEnd();
        }
    }

    private void Btween_OnEndTween(float value) {
        if (closing) {
            Destroy(gameObject);
        }
    }
}