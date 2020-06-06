using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonGeneric : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
	public ThreeStateButton threeStateButton;
        
    public delegate void ButtonEvent ();
	public event ButtonEvent OnClick = delegate { };

    public void OnPointerClick (PointerEventData evd) {
        OnClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (threeStateButton != null)
            threeStateButton.OnPress();
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (threeStateButton != null)
            threeStateButton.OnRelease();
    }
}