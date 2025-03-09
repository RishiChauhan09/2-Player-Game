using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CustomToggle : MonoBehaviour, IPointerDownHandler {

    private Toggle toggle;
    [SerializeField] private RectTransform handle;
    [SerializeField] private GameObject background;

    [Header("Values")]
    [SerializeField] private float time = .25f;
    [SerializeField] private float xMin;
    [SerializeField] private float xMax;
    [SerializeField] private Color colorWhenOff;
    [SerializeField] private Color colorWhenOn;

    private void Awake() {
        toggle = GetComponent<Toggle>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(toggle.isOn) {
            handle.DOAnchorPosX(xMin, time).OnComplete(() => toggle.isOn = false);
            background.gameObject.GetComponent<Image>().color = colorWhenOff;
        } else {
            handle.DOAnchorPosX(xMax, time).OnComplete(() => toggle.isOn = true);
            background.gameObject.GetComponent<Image>().color = colorWhenOn;
        }
    }

    public void ChangeState(bool value) {
        if(value) {
            handle.DOAnchorPosX(xMin, time).OnComplete(() => toggle.isOn = false);
            background.gameObject.GetComponent<Image>().color = colorWhenOff;
        } else {
            handle.DOAnchorPosX(xMax, time).OnComplete(() => toggle.isOn = true);
            background.gameObject.GetComponent<Image>().color = colorWhenOn;
        }
        toggle.isOn = value;
    }

}