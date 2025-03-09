using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class GamePadManager_MiniSoccer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

    [SerializeField] private RectTransform canvanRectTransform;
    [SerializeField] private RectTransform gamepadTransform;
    [SerializeField] private OnScreenStick onScreenStick;

    private Vector2 startPos;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();

        startPos = gamepadTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData) {
        onScreenStick.OnDrag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvanRectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
        
        gamepadTransform.anchoredPosition = localPoint;
        image.enabled = false;
        onScreenStick.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        onScreenStick.OnPointerUp(eventData);
        image.enabled = true;
        gamepadTransform.anchoredPosition = startPos;
    }


}